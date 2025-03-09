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

    public ElementalResistanceData CreateElementalResistanceData()
    {
        ElementalResistanceData data = new ElementalResistanceData(defaultElementalResistance);

        return data;
    }

    public DefenseData CreateDefenseData()
    {
        elementalResistanceData = CreateElementalResistanceData();

        // create a new DefenseData with our values
        DefenseData data = new DefenseData(
            physicalResistance,
            critResistance,
            defaultElementalResistance,
            elementalResistanceData
        );

        return data;
    }
}
