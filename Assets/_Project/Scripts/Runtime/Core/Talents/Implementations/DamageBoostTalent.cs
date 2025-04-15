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
        // Return list with single modifier
        return new List<TalentModifierData>
        {
            new TalentModifierData(
                ModifierType.WEAPON_DAMAGE_MODIFIER,
                ModifyDamageDelegate(),
                GetModifiable(gameObject)
            )
        };
    }

    private Delegate ModifyDamageDelegate()
    {
        // Calculate multiplier based on points (1 + percentage increase)
        // 1 point = 1.05x, 2 points = 1.10x, etc.
        float damageMultiplier = 1f + (damageIncreasePerPoint * pointsDesignated);

        ModifierHelper.FloatInFloatOutModifier fn = (damage) => damage * damageMultiplier;
        return fn;
    }

    protected override IModifiable GetModifiable(GameObject gameObject)
    {
        // Weapon handler is the component that handles weapons and damage
        var weaponHandler = gameObject.GetComponent<WeaponHandler>();
        return weaponHandler as IModifiable;
    }
}
