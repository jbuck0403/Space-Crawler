using System;
using UnityEngine;

[Serializable]
public class DefenseData
{
    [SerializeField, Range(0f, 1f)]
    private float physicalResistance = 0f;

    [SerializeField, Range(0f, 100f)]
    private float critResistance = 0f;

    [SerializeField]
    private ElementalResistanceData elementalResistances = new ElementalResistanceData();

    // properties with public getters
    public float PhysicalResistance => physicalResistance;
    public float CritResistance => critResistance;
    public ElementalResistanceData ElementalResistances => elementalResistances;

    // default constructor with sensible defaults
    public DefenseData()
    {
        // default values already set in field initialization
    }

    // constructor with parameters
    public DefenseData(
        float physicalResistance,
        float critResistance,
        ElementalResistanceData elementalResistances = null
    )
    {
        this.physicalResistance = Mathf.Clamp01(physicalResistance);
        this.critResistance = Mathf.Clamp(critResistance, 0f, 100f);
        this.elementalResistances = elementalResistances ?? new ElementalResistanceData();
    }

    // methods to modify values
    public void SetPhysicalResistance(float value)
    {
        physicalResistance = Mathf.Clamp01(value);
    }

    public void ModifyPhysicalResistance(float amount)
    {
        physicalResistance = Mathf.Clamp01(physicalResistance + amount);
    }

    public void SetCritResistance(float value)
    {
        critResistance = Mathf.Clamp(value, 0f, 100f);
    }

    public void ModifyCritResistance(float amount)
    {
        critResistance = Mathf.Clamp(critResistance + amount, 0f, 100f);
    }

    // Direct methods for elemental resistances
    public float GetElementalResistance(DamageType type)
    {
        return elementalResistances.GetResistance(type);
    }

    public void SetElementalResistance(DamageType type, float value)
    {
        elementalResistances.SetResistance(type, value);
    }

    public void ModifyElementalResistance(DamageType type, float amount)
    {
        elementalResistances.ModifyResistance(type, amount);
    }

    public void SetDefaultElementalResistance(float value)
    {
        elementalResistances.SetDefaultResistance(value);
    }

    public float GetDefaultElementalResistance()
    {
        return elementalResistances.GetDefaultResistance();
    }
}
