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

    public DamageData CreateDamageData(Transform source)
    {
        return new DamageData(baseDamage, source, critMultiplier, critChance, damageType);
    }
}
