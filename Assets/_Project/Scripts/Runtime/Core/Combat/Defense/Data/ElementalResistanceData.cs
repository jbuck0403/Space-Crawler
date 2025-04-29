using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ElementalResistanceData
{
    [SerializeField]
    private List<ElementalResistance> resistances = new List<ElementalResistance>();

    private float defaultElementalResistance;

    private bool initialized = false;

    public ElementalResistanceData(float elementalResistance)
    {
        defaultElementalResistance = elementalResistance;
        PopulateWithAllElementalTypes();
        initialized = true;
    }

    // helper to check if a damage type is elemental
    public static bool IsElemental(DamageType type)
    {
        if (!Enum.IsDefined(typeof(DamageType), type))
        {
            return false;
        }

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

        return defaultElementalResistance;
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

    public bool HasResistance(DamageType type)
    {
        if (type != DamageType.Physical && type != DamageType.True)
        {
            foreach (ElementalResistance resistance in resistances)
            {
                if (resistance.Type == type)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void SetDefaultResistance(float value)
    {
        defaultElementalResistance = Mathf.Clamp01(value);
    }

    public float GetDefaultResistance()
    {
        return defaultElementalResistance;
    }

    public void PopulateWithAllElementalTypes()
    {
        RemoveNonElemental();

        if (!initialized)
        {
            foreach (DamageType damageType in Enum.GetValues(typeof(DamageType)))
            {
                if (damageType == DamageType.Physical || damageType == DamageType.True)
                    continue;

                if (!HasResistance(damageType))
                {
                    SetResistance(damageType, defaultElementalResistance);
                }
            }
        }
    }

    public void RemoveNonElemental()
    {
        resistances.RemoveAll(pair => !IsElemental(pair.Type));
    }

    public void MakeResistancesMeetDefault()
    {
        for (int i = 0; i < resistances.Count; i++)
        {
            ElementalResistance pair = resistances[i];
            if (pair.Value < defaultElementalResistance || pair.Value > defaultElementalResistance)
            {
                pair.Value = defaultElementalResistance;
                resistances[i] = pair;
            }
        }
    }
}

[Serializable]
public struct ElementalResistance
{
    public DamageType Type;

    [Range(0f, 1f)]
    public float Value;
}
