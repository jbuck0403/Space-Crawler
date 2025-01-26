using UnityEngine;

[CreateAssetMenu(fileName = "DefenseProfile", menuName = "Game/Defense Profile")]
public class DefenseProfile : ScriptableObject
{
    [SerializeField]
    private float physicalResistance;

    [SerializeField]
    private float critResistance;

    [SerializeField]
    private DefenseHandlerType handlerType;

    public IDefenseHandler CreateDefenseHandler()
    {
        if (physicalResistance < 0 || physicalResistance > 1)
        {
            Debug.LogError(
                $"Physical resistance in {name} must be between 0 and 1. Current value: {physicalResistance}"
            );
        }
        if (critResistance < 0 || critResistance > 100)
        {
            Debug.LogError(
                $"Crit resistance in {name} must be between 0 and 100. Current value: {critResistance}"
            );
        }

        var data = new DefenseData(physicalResistance, critResistance);

        return handlerType switch
        {
            DefenseHandlerType.Default => new DefaultDefenseHandler(data),
            _ => new DefaultDefenseHandler(data)
        };
    }
}
