// FireConfig.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "SpaceShooter/Weapon Config")]
public class WeaponConfig : ScriptableObject
{
    [SerializeField]
    public FireConfig fireConfig;

    [SerializeField]
    public List<BaseFiringStrategy> firingStrategies;

    [SerializeField]
    public ProjectileTypeSO projectileType;

    private void OnValidate()
    {
        // Ensure we always have a projectile type
        if (projectileType == null)
        {
            Debug.LogWarning(
                $"No projectile type assigned to {name}. Please assign a BasicProjectileTypeSO."
            );
        }
    }

    // Method that handles firing the appropriate projectile type
    public Projectile FireProjectile(
        Transform firePoint,
        Vector2 direction,
        Transform source,
        IProjectileDataProvider dataProvider
    )
    {
        if (projectileType != null)
        {
            return projectileType.SpawnProjectile(
                firePoint,
                direction,
                fireConfig.projectileSpeed,
                source,
                dataProvider
            );
        }

        Debug.LogError(
            $"No projectile type assigned to {name}. Please assign a BasicProjectileTypeSO."
        );
        return null;
    }
}
