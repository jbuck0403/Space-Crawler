using UnityEngine;

[CreateAssetMenu(fileName = "DamageProfile", menuName = "Combat/Damage Profile")]
public class DamageProfile : ScriptableObject
{
    [SerializeField]
    private float baseDamage = 10f;

    [SerializeField]
    private float critMultiplier = 1.5f;

    [SerializeField]
    private float critChance = 0.1f;

    [SerializeField]
    private DamageType damageType;

    public DamageData CreateDamageData(Transform source)
    {
        return new DamageData(baseDamage, source, critMultiplier, critChance, damageType);
    }
}
