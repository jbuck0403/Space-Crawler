using UnityEngine;

public class BaseDefenseHandler : IDefenseHandler
{
    protected DefenseData defenseData;

    public BaseDefenseHandler(DefenseData defenseData = null)
    {
        this.defenseData = defenseData ?? new DefenseData();
    }

    public virtual float HandleDefense(float incomingDamage, DamageData damageData)
    {
        float finalDamage = CalculateDamageAfterDefense(incomingDamage, damageData.Type);
        return -finalDamage; // negative because it's damage
    }

    protected virtual float CalculateDamageAfterDefense(float incomingDamage, DamageType type)
    {
        float damageReduction = 0f;

        if (type == DamageType.Physical)
        {
            damageReduction = defenseData.PhysicalResistance;
        }
        else if (type == DamageType.True)
        {
            damageReduction = 0f; // true damage ignores resistance
        }
        else if (ElementalResistanceData.IsElemental(type))
        {
            damageReduction = defenseData.GetElementalResistance(type);
        }

        return Mathf.Max(0, incomingDamage * (1 - damageReduction));
    }

    // interface implementation
    public float GetCritResistance() => defenseData.CritResistance;

    public float GetPhysicalResistance() => defenseData.PhysicalResistance;

    public void SetPhysicalResistance(float value)
    {
        defenseData.SetPhysicalResistance(value);
    }

    public void ModifyPhysicalResistance(float amount)
    {
        defenseData.ModifyPhysicalResistance(amount);
    }

    public float GetElementalResistance(DamageType type)
    {
        return defenseData.GetElementalResistance(type);
    }

    public void SetElementalResistance(DamageType type, float value)
    {
        defenseData.SetElementalResistance(type, value);
    }

    public void ModifyElementalResistance(DamageType type, float amount)
    {
        defenseData.ModifyElementalResistance(type, amount);
    }

    public DefenseData GetDefenseData()
    {
        return defenseData;
    }
}
