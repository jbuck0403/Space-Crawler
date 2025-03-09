using UnityEngine;

public class BaseDefenseHandler : MonoBehaviour, IDefenseHandler
{
    [SerializeField]
    protected DefenseProfile defenseProfile;
    protected DefenseData defenseData;

    private void Awake()
    {
        if (defenseProfile == null)
        {
            Debug.LogError($"DefenseProfile is null on {gameObject.name}");
            return;
        }

        defenseData = defenseProfile.CreateDefenseData();
    }

    public virtual void SetNewDefenseData(DefenseData defenseData)
    {
        this.defenseData = defenseData;
    }

    public virtual float HandleDefense(float incomingDamage, DamageData damageData)
    {
        float finalDamage = CalculateDamageAfterDefense(incomingDamage, damageData.Type);
        return -finalDamage; // negative because it's damage
    }

    protected virtual float CalculateDamageAfterDefense(float incomingDamage, DamageType type)
    {
        float damageReduction = 0f;

        if (defenseData != null)
        {
            if (type == DamageType.Physical)
            {
                damageReduction = defenseData.PhysicalResistance;
            }
            else if (type == DamageType.True)
            {
                damageReduction = 0f; // true damage ignores resistance
            }
            else
            {
                damageReduction = defenseData.GetElementalResistance(type);
            }
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
