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

    protected override List<TalentModifierData> GetModifierData(GameObject owner)
    {
        WeaponHandler weaponHandler = owner.GetComponent<WeaponHandler>();
        if (weaponHandler == null)
            return new List<TalentModifierData>();

        // Create ONE delegate instance to reuse
        var attackSpeedDelegate = ModifyFireRateDelegate();

        var modifierData = new List<TalentModifierData>
        {
            new TalentModifierData(ModifierType.AUTO_FIRE_RATE_MODIFIER, attackSpeedDelegate, null) // The modifiable is ignored in BaseTalent.OnActivate
        };

        return modifierData;
    }

    private Delegate ModifyFireRateDelegate()
    {
        Debug.Log("***ATTACK SPEED DELEGATE RUN");
        // Calculate total reduction based on points invested
        float totalReduction = fireRateReductionPerPoint * pointsDesignated;

        ModifierHelper.FloatInFloatOutModifier fn = (currentFireRate) =>
        {
            // Reduce the fire rate by flat amount, but ensure it doesn't go below a minimum
            float newFireRate = Mathf.Max(0.1f, currentFireRate - totalReduction);
            Debug.Log($"***MODIFIED ATTACK SPEED: {newFireRate}");

            return newFireRate;
        };

        return fn;
    }

    protected override List<IModifiable> GetModifiable(GameObject gameObject)
    {
        // Weapon handler is the component that handles firing weapons
        WeaponHandler weaponHandler = gameObject.GetComponent<WeaponHandler>();

        List<IModifiable> modifiables = new List<IModifiable>();

        foreach (var weapon in weaponHandler.WeaponInstances)
        {
            modifiables.Add(weapon);
        }

        return modifiables;
    }
}
