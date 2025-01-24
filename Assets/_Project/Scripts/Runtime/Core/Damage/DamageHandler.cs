using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(DefenseHandler))]
public class DamageHandler : MonoBehaviour
{
    private DefenseHandler defenseHandler;
    private HealthSystem healthSystem;

    private void Awake()
    {
        defenseHandler = GetComponent<DefenseHandler>();
        healthSystem = GetComponent<HealthSystem>();
    }

    public void HandleDamage(DamageData rawDamageData)
    {
        float preMitigationDamage = CalculatePreMitigationDamage(rawDamageData);
        float finalDamage = defenseHandler.HandleDefense(preMitigationDamage, rawDamageData);

        healthSystem.ModifyHealth(-finalDamage);
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
