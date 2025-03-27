using UnityEngine;

public class StatusEffectFactory
{
    private static BaseStatusEffect CreateDoTEffect(
        DoTEffectData data,
        GameObject target,
        Transform source
    )
    {
        Debug.Log($"#StatusEffect# Creating DoT Effect of type {data.EffectType}");
        return data.EffectType switch
        {
            StatusEffect.Burning => new BurningEffect(data, target, source),
            _ => null
        };
    }

    public static BaseStatusEffect CreateStatusEffect(
        StatusEffectData data,
        GameObject target,
        Transform source
    )
    {
        Debug.Log($"#StatusEffect# Creating Status Effect of type {data.GetType().Name}");
        if (data is DoTEffectData dotData)
        {
            Debug.Log("#StatusEffect# Data is DoTEffectData");
            return CreateDoTEffect(dotData, target, source);
        }
        else
        {
            Debug.Log("#StatusEffect# Data is not DoTEffectData");
            // create regular effect based on type
            switch (data.EffectType)
            {
                // stun, etc. TBD
                default:
                    return null;
            }
        }
    }
}
