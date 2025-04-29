using UnityEngine;

/// <summary>
/// Base ScriptableObject class for defining different types of projectiles.
/// Handles common spawning logic while allowing specific types to configure their behavior.
/// </summary>
public abstract class ProjectileTypeSO : ScriptableObject
{
    [SerializeField]
    public AmmoType ammoType;

    [Header("Damage Configuration")]
    [SerializeField]
    public DamageProfile damageProfile;

    [Header("VFX")]
    public ProjectileVFXPrefabs projectileVFXPrefabs;

    /// <summary>
    /// Spawns a projectile with the given parameters and configures it using the data provider
    /// </summary>
    public virtual Projectile SpawnProjectile(
        WeaponHandler weaponHandler,
        Transform firePoint,
        Transform source,
        IProjectileDataProvider dataProvider
    )
    {
        // Use ProjectileSpawner for core spawning functionality
        Projectile projectile = ProjectileSpawner.SpawnProjectile(
            weaponHandler,
            firePoint,
            damageProfile,
            source,
            projectileVFXPrefabs
        );

        if (projectile != null)
        {
            // Let derived classes configure the projectile with their specific behavior
            ConfigureProjectile(projectile, dataProvider);
            // WeaponVFXHandler.HandleMuzzleFlare(
            //     projectileVFXPrefabs.muzzleFlashPrefab,
            //     firePoint,
            //     source
            // );
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
