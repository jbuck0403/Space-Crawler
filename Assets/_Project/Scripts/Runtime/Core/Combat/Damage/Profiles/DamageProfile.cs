using UnityEngine;

[CreateAssetMenu(fileName = "DamageProfile", menuName = "Combat/Damage Profile")]
public class DamageProfile : ScriptableObject
{
    [SerializeField]
    private float baseDamage = 10f;

    [SerializeField]
    private float critMultiplier = 1.5f;

    [SerializeField]
    [Range(0, 100)]
    private float critChance = 20f;

    [SerializeField]
    private DamageType damageType;

    [SerializeField]
    private bool penetratesShield = false;

    [SerializeField]
    private StatusEffectData[] statusEffectsToApply;

    public DamageData CreateDamageData(
        Transform source,
        float? damage = null,
        float? critMult = null,
        float? crit = null,
        DamageType? type = null,
        bool? pierceShield = null,
        StatusEffectData[] effects = null
    )
    {
        return new DamageData(
            damage ?? baseDamage,
            source,
            critMult ?? critMultiplier,
            crit ?? critChance,
            type ?? damageType,
            pierceShield ?? penetratesShield,
            effects ?? statusEffectsToApply
        );
    }
}
