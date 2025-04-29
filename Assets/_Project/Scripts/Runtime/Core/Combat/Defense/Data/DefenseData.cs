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
    private float defaultElementalResistance = 0f;

    [SerializeField]
    private ElementalResistanceData elementalResistanceData;

    public float PhysicalResistance => physicalResistance;
    public float CritResistance => critResistance;
    public float DefaultElementalResistance => defaultElementalResistance;

    public DefenseData() { }

    // constructor with parameters
    public DefenseData(
        float physicalResistance,
        float critResistance,
        float defaultElementalResistance,
        ElementalResistanceData elementalResistanceData
    )
    {
        this.physicalResistance = Mathf.Clamp01(physicalResistance);
        this.critResistance = Mathf.Clamp(critResistance, 0f, 100f);
        this.defaultElementalResistance = defaultElementalResistance;
        this.elementalResistanceData = elementalResistanceData;
    }

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

    public float GetElementalResistance(DamageType type)
    {
        return elementalResistanceData.GetResistance(type);
    }

    public void SetElementalResistance(DamageType type, float value)
    {
        elementalResistanceData.SetResistance(type, value);
    }

    public void ModifyElementalResistance(DamageType type, float value)
    {
        elementalResistanceData.ModifyResistance(type, value);
    }
}

public struct ElementalResistancePair
{
    public DamageType type;
    public float resistance;

    public ElementalResistancePair(DamageType type, float resistance)
    {
        this.type = type;
        this.resistance = resistance;
    }
}
