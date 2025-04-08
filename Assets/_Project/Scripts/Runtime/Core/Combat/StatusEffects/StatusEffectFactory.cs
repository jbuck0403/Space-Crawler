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
        if (data is DoTEffectData dotData) // DoT specific handling
        {
            return CreateDoTEffect(dotData, target, source);
        }
        else // normal status effect handling
        {
            return data.EffectType switch
            {
                StatusEffect.Freezing => new FreezingEffect(data, target, source),
                _ => null
            };
        }
    }
}
