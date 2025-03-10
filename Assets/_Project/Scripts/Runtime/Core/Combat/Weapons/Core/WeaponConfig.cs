// FireConfig.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "SpaceShooter/Weapon Config")]
public class WeaponConfig : ScriptableObject
{
    [SerializeField]
    public FireConfig fireConfig;

    [SerializeField]
    public DamageProfile damageProfile;

    [SerializeField]
    public List<BaseFiringStrategy> firingStrategies;
}
