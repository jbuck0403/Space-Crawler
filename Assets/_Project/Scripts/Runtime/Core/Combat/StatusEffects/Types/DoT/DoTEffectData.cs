using UnityEngine;

[CreateAssetMenu(fileName = "New DoT Effect", menuName = "Combat/Status Effects/DoT Effect")]
public class DoTEffectData : StatusEffectData
{
    [Header("Damage Settings")]
    [SerializeField]
    private float baseDamage = 1f;

    [SerializeField]
    private float critMultiplier = 1.5f;

    [SerializeField]
    private float critChance = 0.1f;

    [SerializeField]
    private DamageType damageType = DamageType.Fire;

    public float BaseDamage => baseDamage;
    public float CritMultiplier => critMultiplier;
    public float CritChance => critChance;
    public DamageType DamageType => damageType;
}
