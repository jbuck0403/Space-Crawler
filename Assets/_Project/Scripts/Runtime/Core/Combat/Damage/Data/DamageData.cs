using UnityEngine;

public struct DamageData
{
    public float Amount { get; private set; }
    public Transform Source { get; private set; }
    public float CritMultiplier { get; private set; }
    public float CritChance { get; private set; }
    public DamageType Type { get; private set; }
    public StatusEffectData[] StatusEffectsToApply { get; private set; }

    public DamageData(
        float amount,
        Transform source,
        float critMultiplier,
        float critChance,
        DamageType damageType,
        StatusEffectData[] statusEffectsToApply = null
    )
    {
        Amount = amount;
        Source = source;
        CritMultiplier = critMultiplier;
        CritChance = critChance;
        Type = damageType;
        StatusEffectsToApply = statusEffectsToApply;
    }

    public void ApplyAllStatusEffects(GameObject target)
    {
        Debug.Log("#StatusEffect# Applying All Status Effects");
        if (StatusEffectsToApply == null || StatusEffectsToApply.Length == 0)
            return;

        Debug.Log("#StatusEffect# Status Effects to Apply Null Check Passed");
        foreach (var effect in StatusEffectsToApply)
        {
            if (effect != null)
            {
                Debug.Log("#StatusEffect# Applying Effect");
                effect.SetSource(Source);
                effect.ApplyStatusEffect(target);
            }
        }
    }
}
