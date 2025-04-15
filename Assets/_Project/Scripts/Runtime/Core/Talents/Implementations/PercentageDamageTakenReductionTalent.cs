using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Talent that reduces damage taken by 5% per point
/// </summary>
[CreateAssetMenu(menuName = "SpaceShooter/Talents/Defense Boost")]
public class PercentageDamageTakenReductionTalent : BaseTalent
{
    [SerializeField]
    private float damageReductionPerPoint = 0.05f; // 5% reduction per point

    protected override List<TalentModifierData> GetModifierData(GameObject gameObject)
    {
        // Return list with single modifier
        return new List<TalentModifierData>
        {
            new TalentModifierData(
                ModifierType.AFTER_DEFENSE_CALCULATION,
                ModifyIncomingDamageDelegate(),
                GetModifiable(gameObject)
            )
        };
    }

    private Delegate ModifyIncomingDamageDelegate()
    {
        // Calculate total damage reduction based on points invested
        float totalDamageReduction = damageReductionPerPoint * pointsDesignated;

        ModifierHelper.FloatInFloatOutModifier fn = (incomingDamage) =>
        {
            // Reduce incoming damage by percentage
            // Note: We use (1 - damageReduction) as a multiplier since less damage is better
            return incomingDamage * (1f - totalDamageReduction);
        };

        return fn;
    }

    protected override IModifiable GetModifiable(GameObject gameObject)
    {
        // DamageHandler is the component that processes incoming damage
        var damageHandler = gameObject.GetComponent<BaseDefenseHandler>();
        return damageHandler;
    }
}
