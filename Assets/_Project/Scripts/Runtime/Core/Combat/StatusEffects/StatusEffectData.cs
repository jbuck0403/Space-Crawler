using UnityEngine;

[CreateAssetMenu(fileName = "New Status Effect", menuName = "Combat/Status Effect")]
public class StatusEffectData : ScriptableObject
{
    [Header("Basic Properties")]
    [SerializeField]
    private StatusEffect effectType;

    [Header("Effect Settings")]
    [SerializeField]
    private float duration = 5f;

    [SerializeField]
    private float tickRate = 1f; // how often the effect triggers (for periodic effects)

    [SerializeField]
    private bool isStackable;

    [SerializeField]
    private int maxStacks = 1;

    [Header("UI Properties")]
    [SerializeField]
    private string description;

    [SerializeField]
    private Sprite icon;

    // Properties

    public StatusEffect EffectType => effectType;
    public string Description => description;
    public Sprite Icon => icon;
    public float Duration => duration;
    public float TickRate => tickRate;
    public bool IsStackable => isStackable;
    public int MaxStacks => maxStacks;

    public void ApplyStatusEffect(GameObject target)
    {
        var receiver = target.GetComponent<IStatusEffectReceiver>();

        if (receiver != null)
        {
            receiver.ApplyStatusEffect(this);
        }
    }
}
