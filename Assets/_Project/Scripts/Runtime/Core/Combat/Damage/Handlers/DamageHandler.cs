using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(IDefenseHandler))]
public class DamageHandler : MonoBehaviour
{
    private IDefenseHandler defenseHandler;
    private HealthSystem healthSystem;
    private ShieldHandler shieldHandler;

    [SerializeField]
    private VoidEvent onCriticalHit;

    [SerializeField]
    private DamageTakenEvent OnDamageTaken;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        defenseHandler = GetComponent<IDefenseHandler>();
        shieldHandler = GetComponent<ShieldHandler>();
    }

    public void SetNewDefenseHandler(IDefenseHandler defenseHandler)
    {
        this.defenseHandler = defenseHandler;
    }

    public void HandleDamage(DamageData rawDamageData)
    {
        // TBI Skill Point Delegate: BEFORE_DAMAGE_HANDLER
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

        (float preMitigationDamage, bool isCrit) = CalculatePreMitigationDamage(rawDamageData);
        print($"PreMitigation Damage: {preMitigationDamage}");

        // TBI Skill Point Delegate: BEFORE_DEFENSE_CALCULATION
        float finalDamage = defenseHandler.HandleDefense(preMitigationDamage, rawDamageData);
        // TBI Skill Point Delegate: AFTER_DEFENSE_CALCULATION
        print($"Final Damage: {finalDamage}");

        DamageTakenEventData damageTakenEventData = new DamageTakenEventData(
            finalDamage,
            rawDamageData.Type,
            isCrit
        );

        float overflowDamage = finalDamage;
        bool damageDealt = false;

        if (shieldHandler != null && !rawDamageData.PenetratesShield && shieldHandler.HasShield)
        {
            overflowDamage = shieldHandler.Damage(finalDamage, rawDamageData.Type);
            float absorbedByShield = Mathf.Abs(finalDamage) - Mathf.Abs(overflowDamage);

            if (absorbedByShield > 0)
                damageDealt = true;
            print($"Shield took {absorbedByShield} points of damage");
        }

        if (overflowDamage < 0)
        {
            print($"Overflow damage of {overflowDamage}");
            // TBI Skill Point Delegate: BEFORE_HEALTH_DAMAGE
            healthSystem.Damage(overflowDamage);
            damageDealt = true;
        }

        if (damageDealt)
        {
            if (shieldHandler != null)
            {
                shieldHandler.SetRecharging(false);
                shieldHandler.HandleDelayedShieldRecharge();
            }

            OnDamageTaken.Raise(gameObject, damageTakenEventData);
            // TBI Skill Point Delegate: AFTER_DAMAGE_DEALT
        }
    }

    public void SetDefenseHandler(IDefenseHandler defenseHandler)
    {
        this.defenseHandler = defenseHandler;
    }

    private (float, bool) CalculatePreMitigationDamage(DamageData rawDamageData)
    {
        float finalDamage = rawDamageData.Amount;
        // TBI Skill Point Delegate: BEFORE_CRIT_CALCULATION
        float critMultiplier = rawDamageData.CritMultiplier;
        float critChance = Mathf.Clamp(
            rawDamageData.CritChance - defenseHandler.GetCritResistance(),
            0,
            100
        );
        bool criticalHit;

        (finalDamage, criticalHit) = ApplyCriticalHit(finalDamage, critMultiplier, critChance);
        // TBI Skill Point Delegate: AFTER_CRIT_CALCULATION

        return (finalDamage, criticalHit);
    }

    private (float, bool) ApplyCriticalHit(
        float finalDamage,
        float critMultiplier,
        float critChance
    )
    {
        bool isCrit = RandomUtils.Chance(critChance);
        print($"Crit: {isCrit} Crit%: {critChance}");

        if (isCrit)
        {
            onCriticalHit.Raise(gameObject);
            // TBI Skill Point Delegate: ON_CRITICAL_HIT
        }

        return (finalDamage * (isCrit ? critMultiplier : 1), isCrit);
    }
}
