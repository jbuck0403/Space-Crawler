// using System;
using System;
using UnityEngine;

/// <summary>
/// Static utility class for spawning projectiles via appropriate pools
/// </summary>
public static class ProjectileSpawner
{
    /// <summary>
    /// Core projectile spawning method that interfaces with the appropriate pool
    /// </summary>
    public static Projectile SpawnProjectile(
        WeaponHandler weaponHandler,
        Transform firePoint,
        DamageProfile damageProfile,
        Transform source,
        ProjectileVFXPrefabs prefabs
    )
    {
        // ProjectilePool pool = ProjectilePool.Instance;
        // if (pool == null)
        // {
        //     Debug.LogError($"No projectile pool found");
        //     return null;
        // }

        // Get projectile from pool and initialize it
        // GameObject projectileObject = pool.GetProjectile(firePoint);

        // GameObject muzzleFlash = GameObject.Instantiate(
        //     prefabs.muzzleFlashPrefab,
        //     firePoint.position,
        //     firePoint.rotation
        // );

        // float timeUntilNextShot = source
        //     .GetComponent<WeaponHandler>()
        //     .CurrentWeapon.FireConfig.fireRate;
        // GameObject.Destroy(muzzleFlash, timeUntilNextShot);

        GameObject projectileObject = GameObject.Instantiate(
            prefabs.projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        Projectile projectile = projectileObject.GetComponent<Projectile>();

        // // Get or add the component
        // Projectile projectile = projectileObject.GetComponent<Projectile>();
        // if (projectile == null)
        // {
        //     Debug.LogError($"Pool returned object without required Projectile component");
        //     pool.ReturnToPool(projectileObject);
        //     return null;
        // }

        DamageData damageData = damageProfile.CreateDamageData(source);

        projectile.Initialize(
            weaponHandler,
            prefabs.targetHitPrefab,
            weaponHandler.OnHitFX,
            damageData
        );

        return projectile;
    }

    /// <summary>
    /// Spawns a projectile with a specific behavior
    /// </summary>
    public static Projectile SpawnProjectileWithBehavior<T>(
        WeaponHandler weaponHandler,
        Transform firePoint,
        DamageProfile damageProfile,
        Transform source,
        ProjectileVFXPrefabs prefabs,
        params object[] behaviorParams
    )
        where T : MonoBehaviour, IProjectileBehavior
    {
        // Spawn the base projectile
        Projectile projectile = SpawnProjectile(
            weaponHandler,
            firePoint,
            damageProfile,
            source,
            prefabs
        );
        if (projectile == null)
            return null;

        // Get or add the behavior component
        T behavior = projectile.GetComponent<T>();
        if (behavior == null)
        {
            behavior = projectile.gameObject.AddComponent<T>();
        }

        // Initialize the behavior with provided parameters
        behavior.Initialize(projectile, behaviorParams);

        projectile.CanonizeBehaviors();

        return projectile;
    }

    /// <summary>
    /// Spawns a homing projectile that tracks a target
    /// </summary>
    public static Projectile SpawnHomingProjectile(
        WeaponHandler weaponHandler,
        Transform firePoint,
        DamageProfile damageProfile,
        Transform source,
        ProjectileVFXPrefabs prefabs,
        Transform target,
        float turnSpeed,
        MovementConfig config
    )
    {
        // Spawn a projectile with the homing behavior and pass the target as a parameter
        Projectile projectile = SpawnProjectileWithBehavior<HomingProjectileBehavior>(
            weaponHandler,
            firePoint,
            damageProfile,
            source,
            prefabs,
            target,
            turnSpeed,
            config
        );

        return projectile;
    }

    public static Projectile SpawnGrenadeProjectile(
        WeaponHandler weaponHandler,
        Transform firePoint,
        DamageProfile damageProfile,
        Transform source,
        ProjectileVFXPrefabs prefabs,
        object[] parameters
    )
    {
        // Spawn a projectile with the Grenade behavior and pass the target as a parameter
        Projectile projectile = SpawnProjectileWithBehavior<GrenadeProjectileBehavior>(
            weaponHandler,
            firePoint,
            damageProfile,
            source,
            prefabs,
            parameters
        );

        return projectile;
    }

    /// <summary>
    /// Apply velocity to a projectile in the specified direction
    /// </summary>
    public static void ApplyVelocity(
        GameObject projectile,
        Vector2 direction,
        FireConfig fireConfig,
        float velocityModifier = 1f,
        float velocityOverload = 1f
    )
    {
        if (projectile == null)
            return;

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float projectileSpeed =
                velocityOverload == 1f
                    ? fireConfig.projectileSpeed * velocityModifier
                    : velocityOverload * velocityModifier;

            rb.velocity = projectileSpeed * direction.normalized;

            // update the projectile's rotation to match its travel direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            Debug.LogWarning(
                $"Projectile {projectile.name} has no Rigidbody2D component to apply velocity to"
            );
        }
    }

    /// <summary>
    /// Apply accuracy spread to the direction vector
    /// </summary>
    public static Vector2 ApplyAccuracySpread(FireConfig fireConfig, Vector2 baseDirection)
    {
        if (fireConfig.accuracy >= 1f)
            return baseDirection;

        float maxSpreadRadians =
            ConvertSpreadDegreesToRadians(fireConfig.spread) * (1f - fireConfig.accuracy);

        // float randomSpread = Random.Range(-maxSpreadRadians / 2f, maxSpreadRadians / 2f);
        float randomSpread = RandomUtils.Range(-maxSpreadRadians / 2f, maxSpreadRadians / 2f);

        float cos = Mathf.Cos(randomSpread);
        float sin = Mathf.Sin(randomSpread);
        return new Vector2(
            baseDirection.x * cos - baseDirection.y * sin,
            baseDirection.x * sin + baseDirection.y * cos
        );
    }

    private static float ConvertSpreadDegreesToRadians(float spreadDegrees)
    {
        // Clamp the input to valid range
        spreadDegrees = Mathf.Clamp(spreadDegrees, 0f, 180f);

        // 180 degrees = PI radians
        return spreadDegrees * Mathf.PI / 180f;
    }
}

[Serializable]
public class ProjectileVFXPrefabs
{
    public GameObject projectilePrefab;
    public GameObject muzzleFlashPrefab;
    public GameObject targetHitPrefab;
}
