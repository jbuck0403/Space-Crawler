using UnityEngine;

public class StatusEffectFactory
{
    private static BaseStatusEffect CreateDoTEffect(
        DoTEffectData dotData,
        GameObject target,
        DamageData damageData
    )
    {
        return dotData.EffectType switch
        {
            StatusEffect.Burning => new BurningEffect(dotData, target, damageData),
            // StatusEffect.Poisoned => new PoisonEffect(dotData, target, damageData),
            _ => null
        };
    }

    public static BaseStatusEffect CreateStatusEffect(StatusEffectData data, GameObject target)
    {
        if (data is DoTEffectData dotData)
        {
            var damageData = new DamageData(
                dotData.BaseDamage,
                target.transform,
                dotData.CritMultiplier,
                dotData.CritChance,
                dotData.DamageType
            );
            return CreateDoTEffect(dotData, target, damageData);
        }

        // handle non-DoT effects here when we add them
        return null;
    }
}
