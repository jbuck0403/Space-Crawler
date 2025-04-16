using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Talent that increases damage dealt by 5% per point
/// </summary>
[CreateAssetMenu(menuName = "SpaceShooter/Talents/Damage Boost")]
public class DamageBoostTalent : BaseTalent
{
    [SerializeField]
    private float damageIncreasePerPoint = 0.05f; // 5% increase per point

    protected override List<TalentModifierData> GetModifierData(GameObject gameObject)
    {
        List<TalentModifierData> modifierDataList = new List<TalentModifierData>();

        Delegate modifier = ModifyDamageDelegate();

        foreach (IModifiable weapon in GetModifiable(gameObject))
        {
            modifierDataList.Add(
                new TalentModifierData(ModifierType.WEAPON_DAMAGE_MODIFIER, modifier, weapon)
            );
        }

        return modifierDataList;
    }

    private Delegate ModifyDamageDelegate()
    {
        // Calculate multiplier based on points (1 + percentage increase)
        // 1 point = 1.05x, 2 points = 1.10x, etc.
        float damageMultiplier = 1f + (damageIncreasePerPoint * pointsDesignated);

        ModifierHelper.FloatInFloatOutModifier fn = (damage) =>
        {
            Debug.Log("***WORKING");
            return damage * damageMultiplier;
        };
        return fn;
    }

    protected override List<IModifiable> GetModifiable(GameObject gameObject)
    {
        WeaponHandler weaponHandler = gameObject.GetComponent<WeaponHandler>();
        List<IModifiable> weapons = new List<IModifiable>();
        foreach (var weapon in weaponHandler.WeaponInstances)
        {
            weapons.Add(weapon);
        }
        return weapons;
    }
}
