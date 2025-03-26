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
        Transform firePoint,
        Vector2 direction,
        float speed,
        DamageProfile damageProfile,
        Transform source
    )
    {
        // Get the base projectile pool
        var pool = Object.FindObjectOfType<ProjectilePool>();
        if (pool == null)
        {
            Debug.LogError($"No projectile pool found");
            return null;
        }

        // Get projectile from pool and initialize it
        GameObject projectileObject = pool.GetProjectile(firePoint);

        // Get or add the component
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        if (projectile == null)
        {
            Debug.LogError($"Pool returned object without required Projectile component");
            pool.ReturnToPool(projectileObject);
            return null;
        }

        // Set damage data
        DamageData damageData = damageProfile.CreateDamageData(source);

        // Initialize the projectile
        projectile.Initialize(pool, damageData);

        // Apply velocity
        ApplyVelocity(projectileObject, direction, speed);

        return projectile;
    }

    /// <summary>
    /// Spawns a projectile with a specific behavior
    /// </summary>
    public static Projectile SpawnProjectileWithBehavior<T>(
        Transform firePoint,
        Vector2 direction,
        float speed,
        DamageProfile damageProfile,
        Transform source,
        params object[] behaviorParams
    )
        where T : MonoBehaviour, IProjectileBehavior
    {
        // Spawn the base projectile
        Projectile projectile = SpawnProjectile(firePoint, direction, speed, damageProfile, source);
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

        return projectile;
    }

    /// <summary>
    /// Spawns a homing projectile that tracks a target
    /// </summary>
    public static Projectile SpawnHomingProjectile(
        Transform firePoint,
        Vector2 direction,
        float speed,
        DamageProfile damageProfile,
        Transform source,
        Transform target,
        float turnSpeed,
        MovementConfig config
    )
    {
        // Spawn a projectile with the homing behavior and pass the target as a parameter
        Projectile projectile = SpawnProjectileWithBehavior<HomingProjectileBehavior>(
            firePoint,
            direction,
            speed,
            damageProfile,
            source,
            target,
            turnSpeed,
            config
        );

        return projectile;
    }

    private static void ApplyVelocity(GameObject projectileObject, Vector2 direction, float speed)
    {
        Rigidbody2D rb = projectileObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction.normalized * speed;
        }
        else
        {
            Debug.LogWarning("Projectile does not have a Rigidbody component!");
        }
    }
}
