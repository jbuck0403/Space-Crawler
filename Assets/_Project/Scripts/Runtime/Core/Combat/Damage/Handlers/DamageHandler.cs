using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(IDefenseHandler))]
public class DamageHandler : MonoBehaviour
{
    private IDefenseHandler defenseHandler;
    private HealthSystem healthSystem;

    [SerializeField]
    private VoidEvent onCriticalHit;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        defenseHandler = GetComponent<IDefenseHandler>();
    }

    public void SetNewDefenseHandler(IDefenseHandler defenseHandler)
    {
        this.defenseHandler = defenseHandler;
    }

    public void HandleDamage(DamageData rawDamageData)
    {
        print(
            $"HandleDamage Raw:{rawDamageData.Amount} Crit% {rawDamageData.CritChance} CritX {rawDamageData.CritMultiplier}"
        );
        if (healthSystem == null)
        {
            Debug.LogError($"No HealthSystem found on {gameObject.name}");
            return;
        }

        if (defenseHandler == null)
        {
            Debug.LogError($"No DefenseHandler initialized on {gameObject.name}");
            return;
        }

        float preMitigationDamage = CalculatePreMitigationDamage(rawDamageData);
        print($"PreMitigation Damage: {preMitigationDamage}");

        float finalDamage = defenseHandler.HandleDefense(preMitigationDamage, rawDamageData);
        print($"Final Damage: {finalDamage}");
        healthSystem.ModifyHealth(finalDamage);
    }

    public void SetDefenseHandler(IDefenseHandler defenseHandler)
    {
        this.defenseHandler = defenseHandler;
    }

    private float CalculatePreMitigationDamage(DamageData rawDamageData)
    {
        float finalDamage = rawDamageData.Amount;
        float critMultiplier = rawDamageData.CritMultiplier;
        float critChance = Mathf.Clamp(
            rawDamageData.CritChance - defenseHandler.GetCritResistance(),
            0,
            100
        );

        finalDamage = ApplyCriticalHit(finalDamage, critMultiplier, critChance);

        return finalDamage;
    }

    private float ApplyCriticalHit(float finalDamage, float critMultiplier, float critChance)
    {
        bool isCrit = RandomUtils.Chance(critChance);
        print($"Crit: {isCrit} Crit%: {critChance}");

        if (isCrit)
        {
            onCriticalHit.Raise();
        }

        return finalDamage * (isCrit ? critMultiplier : 1);
    }
}
