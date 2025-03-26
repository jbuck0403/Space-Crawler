using UnityEngine;

public class StatusEffectFactory
{
    private static BaseStatusEffect CreateDoTEffect(
        StatusEffectData data,
        GameObject target,
        DamageData damageData
    )
    {
        return data.EffectType switch
        {
            StatusEffect.Burning => new BurningEffect(data, target, damageData),
            _ => null
        };
    }

    public static BaseStatusEffect CreateStatusEffect(
        StatusEffectData data,
        GameObject target,
        DamageData? damageData = null
    )
    {
        if (damageData.HasValue)
        {
            return CreateDoTEffect(data, target, damageData.Value);
        }

        // otherwise create a regular effect based on type
        switch (data.EffectType)
        {
            // stun, etc. TBD
            default:
                return null;
        }
    }
}
