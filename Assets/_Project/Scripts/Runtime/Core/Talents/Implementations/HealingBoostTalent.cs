using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Talent that increases healing received by a percentage
/// </summary>
[CreateAssetMenu(menuName = "SpaceShooter/Talents/Healing Boost")]
public class HealingBoostTalent : BaseTalent
{
    [SerializeField]
    private float healingMultiplier = 1.25f;

    protected override List<TalentModifierData> GetModifierData(GameObject gameObject)
    {
        // Return list with single modifier
        return new List<TalentModifierData>
        {
            new TalentModifierData(
                ModifierType.BEFORE_HEALING,
                ModifyHealingAmountDelegate(),
                GetModifiable(gameObject)
            )
        };
    }

    private Delegate ModifyHealingAmountDelegate()
    {
        HealthSystem.HealingModifier fn = (amount) => amount * healingMultiplier;
        return fn;
    }

    protected override IModifiable GetModifiable(GameObject gameObject)
    {
        return gameObject.GetComponent<HealthSystem>();
    }
}
