using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ElementalResistanceData
{
    [SerializeField]
    private List<ElementalResistance> resistances = new List<ElementalResistance>();

    [SerializeField, Range(0f, 1f)]
    private float defaultResistance = 0f;

    // helper to check if a damage type is elemental
    public static bool IsElemental(DamageType type)
    {
        return type != DamageType.Physical && type != DamageType.True;
    }

    // get resistance for a specific elemental damage type
    public float GetResistance(DamageType type)
    {
        if (!IsElemental(type))
            return 0f;

        foreach (var resistance in resistances)
        {
            if (resistance.Type == type)
                return resistance.Value;
        }

        return defaultResistance;
    }

    public void SetResistance(DamageType type, float value)
    {
        if (!IsElemental(type))
            return;

        value = Mathf.Clamp01(value);

        for (int i = 0; i < resistances.Count; i++)
        {
            if (resistances[i].Type == type)
            {
                ElementalResistance resistance = resistances[i];
                resistance.Value = value;
                resistances[i] = resistance;
                return;
            }
        }

        // if not found, add new resistance
        resistances.Add(new ElementalResistance { Type = type, Value = value });
    }

    public void ModifyResistance(DamageType type, float amount)
    {
        if (!IsElemental(type))
            return;

        float currentValue = GetResistance(type);
        SetResistance(type, currentValue + amount);
    }

    public void SetDefaultResistance(float value)
    {
        defaultResistance = Mathf.Clamp01(value);
    }

    public float GetDefaultResistance()
    {
        return defaultResistance;
    }
}

// helper struct to pair damage type with resistance value
[Serializable]
public struct ElementalResistance
{
    public DamageType Type;

    [Range(0f, 1f)]
    public float Value;
}
