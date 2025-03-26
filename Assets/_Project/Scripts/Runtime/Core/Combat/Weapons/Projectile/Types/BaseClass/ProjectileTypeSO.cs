using UnityEngine;

/// <summary>
/// Base ScriptableObject class for defining different types of projectiles.
/// Handles common spawning logic while allowing specific types to configure their behavior.
/// </summary>
public abstract class ProjectileTypeSO : ScriptableObject
{
    [Header("Damage Configuration")]
    [SerializeField]
    protected DamageProfile damageProfile;

    /// <summary>
    /// Spawns a projectile with the given parameters and configures it using the data provider
    /// </summary>
    public virtual Projectile SpawnProjectile(
        Transform firePoint,
        Vector2 direction,
        float speed,
        Transform source,
        IProjectileDataProvider dataProvider
    )
    {
        // Use ProjectileSpawner for core spawning functionality
        Projectile projectile = ProjectileSpawner.SpawnProjectile(
            firePoint,
            direction,
            speed,
            damageProfile,
            source
        );

        if (projectile != null)
        {
            // Let derived classes configure the projectile with their specific behavior
            ConfigureProjectile(projectile, dataProvider);
        }

        return projectile;
    }

    /// <summary>
    /// Override this method to configure the specific behavior of this projectile type
    /// </summary>
    protected abstract void ConfigureProjectile(
        Projectile projectile,
        IProjectileDataProvider dataProvider
    );
}
