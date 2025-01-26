using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
// [RequireComponent(typeof(DefenseHandler))]
public class DamageHandler : MonoBehaviour
{
    private IDefenseHandler defenseHandler;
    private HealthSystem healthSystem;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
    }

    public void Initialize(IDefenseHandler defenseHandler)
    {
        this.defenseHandler = defenseHandler;
    }

    public void HandleDamage(DamageData rawDamageData)
    {
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
        float finalDamage = defenseHandler.HandleDefense(preMitigationDamage, rawDamageData);
        healthSystem.ModifyHealth(finalDamage);
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
        return finalDamage * (RandomUtils.Chance(critChance) ? critMultiplier : 1);
    }
}
