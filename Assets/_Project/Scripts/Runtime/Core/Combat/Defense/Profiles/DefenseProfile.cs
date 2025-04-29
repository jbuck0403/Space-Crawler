using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefenseProfile", menuName = "Combat/Defense Profile")]
public class DefenseProfile : ScriptableObject
{
    [SerializeField, Range(0f, 1f)]
    private float physicalResistance = 0f;

    [SerializeField, Range(0f, 100f)]
    private float critResistance = 0f;

    [SerializeField, Range(0f, 1f)]
    private float defaultElementalResistance = 0f;

    [SerializeField]
    private ElementalResistanceData elementalResistanceData;

    private float prevDefaultElementalResistance;

    // populate resistance list based on defined elemental enum types
    void OnValidate()
    {
        UpdateElementalResistanceData();
    }

    public ElementalResistanceData CreateElementalResistanceData()
    {
        ElementalResistanceData data = new ElementalResistanceData(defaultElementalResistance);

        return data;
    }

    public DefenseData CreateDefenseData()
    {
        UpdateElementalResistanceData();

        DefenseData data = new DefenseData(
            physicalResistance,
            critResistance,
            defaultElementalResistance,
            elementalResistanceData
        );

        return data;
    }

    private void UpdateElementalResistanceData()
    {
        prevDefaultElementalResistance = elementalResistanceData.GetDefaultResistance();

        if (elementalResistanceData == null)
        {
            elementalResistanceData = CreateElementalResistanceData();
        }
        else
        {
            Debug.Log("Updating Resistance Data");
            elementalResistanceData.PopulateWithAllElementalTypes();
            elementalResistanceData.SetDefaultResistance(defaultElementalResistance);

            if (prevDefaultElementalResistance != defaultElementalResistance)
                elementalResistanceData.MakeResistancesMeetDefault();
        }
    }
}
