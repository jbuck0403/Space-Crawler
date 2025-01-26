using UnityEngine;

public class BaseDefenseHandler : IDefenseHandler
{
    protected DefenseData defenseData;

    public float GetCritResistance() => defenseData.CritResistance;

    public BaseDefenseHandler(DefenseData defenseData)
    {
        if (defenseData.PhysicalResistance < 0 || defenseData.PhysicalResistance > 1)
        {
            Debug.LogError(
                $"Physical resistance must be between 0 and 1. Current value: {defenseData.PhysicalResistance}"
            );
        }
        if (defenseData.CritResistance < 0 || defenseData.CritResistance > 100)
        {
            Debug.LogError(
                $"Crit resistance must be between 0 and 100. Current value: {defenseData.CritResistance}"
            );
        }
        this.defenseData = defenseData;
    }

    public virtual float HandleDefense(float incomingDamage, DamageData damageData)
    {
        float finalDamage = CalculateDamageAfterDefense(incomingDamage, damageData.Type);

        return -finalDamage;
    }

    protected virtual float CalculateDamageAfterDefense(float incomingDamage, DamageType type)
    {
        float damageReduction = 0f;

        switch (type)
        {
            case DamageType.Physical:
                damageReduction = defenseData.PhysicalResistance;
                break;

            case DamageType.True:
                damageReduction = 0;
                break;
        }

        return Mathf.Max(0, incomingDamage * (1 - damageReduction));
    }
}
