using UnityEngine;

public class StatusEffectFactory
{
    public static BaseStatusEffect CreateStatusEffect(StatusEffectData data, GameObject target)
    {
        // create damage data if this is a DoT effect
        DamageData? damageData = null;

        if (data is DoTEffectData dotData)
        {
            damageData = new DamageData(
                dotData.BaseDamage,
                target.transform,
                dotData.CritMultiplier,
                dotData.CritChance,
                dotData.DamageType
            );
        }

        // create the appropriate effect based on type
        return data.EffectType switch
        {
            StatusEffect.Burning when data is DoTEffectData && damageData.HasValue
                => new BurningEffect(data, target, damageData.Value),

            _ => null
        };
    }
}
