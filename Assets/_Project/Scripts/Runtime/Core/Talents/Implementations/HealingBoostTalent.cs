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
        List<TalentModifierData> modifierDataList = new List<TalentModifierData>();

        foreach (IModifiable modifiable in GetModifiable(gameObject))
        {
            modifierDataList.Add(
                new TalentModifierData(
                    ModifierType.AUTO_FIRE_RATE_MODIFIER,
                    ModifyHealingAmountDelegate(),
                    modifiable
                )
            );
        }

        return modifierDataList;
    }

    private Delegate ModifyHealingAmountDelegate()
    {
        // HealthSystem.HealingModifier fn = (amount) => amount * healingMultiplier;
        ModifierHelper.FloatInFloatOutModifier fn = (amount) => amount * healingMultiplier;
        return fn;
    }

    protected override List<IModifiable> GetModifiable(GameObject gameObject)
    {
        HealthSystem healthSystem = gameObject.GetComponent<HealthSystem>();
        return new List<IModifiable> { healthSystem };
    }
}
