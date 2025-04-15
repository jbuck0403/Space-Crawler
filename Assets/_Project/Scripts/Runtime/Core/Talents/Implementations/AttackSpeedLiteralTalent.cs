using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Talent that reduces the interval between shots by a flat 0.1 seconds per point
/// Most beneficial to fast-firing weapons like pistol
/// </summary>
[CreateAssetMenu(menuName = "SpaceShooter/Talents/Attack Speed Literal")]
public class AttackSpeedLiteralTalent : BaseTalent
{
    [SerializeField]
    private float fireRateReductionPerPoint = 0.1f; // Seconds to reduce per point

    protected override List<TalentModifierData> GetModifierData(GameObject gameObject)
    {
        // Return list with single modifier
        return new List<TalentModifierData>
        {
            new TalentModifierData(
                ModifierType.AUTO_FIRE_RATE_MODIFIER,
                ModifyFireRateDelegate(),
                GetModifiable(gameObject)
            )
        };
    }

    private Delegate ModifyFireRateDelegate()
    {
        // Calculate total reduction based on points invested
        float totalReduction = fireRateReductionPerPoint * pointsDesignated;

        ModifierHelper.FloatInFloatOutModifier fn = (currentFireRate) =>
        {
            // Reduce the fire rate by flat amount, but ensure it doesn't go below a minimum
            float newFireRate = Mathf.Max(0.1f, currentFireRate - totalReduction);
            return newFireRate;
        };

        return fn;
    }

    protected override IModifiable GetModifiable(GameObject gameObject)
    {
        // Weapon handler is the component that handles firing weapons
        var weaponHandler = gameObject.GetComponent<WeaponHandler>();
        return weaponHandler as IModifiable;
    }
}
