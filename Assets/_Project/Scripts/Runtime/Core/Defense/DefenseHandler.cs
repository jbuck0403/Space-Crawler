using UnityEngine;

public class DefenseHandler : MonoBehaviour
{
    private DefenseData defenseData;

    public float HandleDefense(float incomingDamage, DamageData damageData)
    {
        float finalDamage = CalculateDamageAfterDefense(incomingDamage, damageData.Type);

        return -finalDamage;
    }

    private float CalculateDamageAfterDefense(float incomingDamage, DamageType type)
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

    public void SetDefenseData(DefenseData newDefenseData)
    {
        defenseData = newDefenseData;
    }

    public float GetCritResistance()
    {
        return defenseData.CritResistance;
    }
}
