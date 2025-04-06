using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "SpaceShooter/Weapons/Base Weapon")]
public abstract class BaseWeaponSO : ScriptableObject
{
    [Header("Weapon Configuration")]
    [SerializeField]
    protected FireConfig fireConfig;

    [SerializeField]
    protected ProjectileTypeSO projectileType;

    [SerializeField]
    protected VoidEvent OnFireWeapon;

    [Header("Weapon Properties")]
    [SerializeField]
    protected float damageModifier = 1f;

    [SerializeField]
    protected float velocityModifier = 1f;

    [SerializeField]
    protected float abilityCooldown = 5f;

    protected bool isInitialized = false;
    protected float nextFireTime = 0f;
    protected float nextAbilityTime = 0f;

    // Properties
    public FireConfig FireConfig => fireConfig;
    public ProjectileTypeSO ProjectileType => projectileType;

    /// <summary>
    /// Initialize the weapon with necessary references and return an instance
    /// </summary>
    public virtual BaseWeaponSO Initialize(WeaponHandler weaponHandler)
    {
        BaseWeaponSO instance = Instantiate(this);

        instance.fireConfig.accuracy *= velocityModifier;
        instance.nextFireTime = 0f;
        instance.isInitialized = true;

        return instance;
    }

    /// <summary>
    /// Get a projectile from the pool using the weapon's projectile type
    /// </summary>
    protected virtual Projectile GetProjectile(
        Transform firePoint,
        Transform source,
        IProjectileDataProvider provider
    )
    {
        if (projectileType == null)
        {
            Debug.LogError($"No projectile type assigned to {name}");
            return null;
        }

        // Get the projectile from the ProjectileSpawner
        Projectile projectile = projectileType.SpawnProjectile(firePoint, source, provider);

        if (projectile != null)
        {
            return projectile;
        }

        return null;
    }

    /// <summary>
    /// Fire a single projectile
    /// </summary>
    protected virtual bool FireProjectile(
        Transform firePoint,
        Vector2 direction,
        Transform source,
        GameObject sourceObject,
        IProjectileDataProvider provider
    )
    {
        Projectile projectile = GetProjectile(firePoint, source, provider);

        if (damageModifier != 1f)
        {
            DamageData damageData = projectile.damageData;
            float newDamage = damageData.Amount * damageModifier;

            projectile.damageData = projectileType.damageProfile.CreateDamageData(
                source,
                damage: newDamage
            );
        }

        if (projectile != null)
        {
            Vector2 modifiedDirection = ApplyAccuracySpread(direction, fireConfig.spread);
            ApplyVelocity(projectile.gameObject, modifiedDirection);

            // Raise event on the source object
            OnFireWeapon.Raise(sourceObject);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Check if the weapon can fire based on fire rate
    /// </summary>
    public virtual bool CanFire()
    {
        return Time.time >= nextFireTime;
    }

    protected void UpdateNextFireTime()
    {
        nextFireTime = Time.time + fireConfig.fireRate;
    }

    public bool CanActivateAbility()
    {
        return Time.time >= nextAbilityTime;
    }

    protected void UpdateNextAbilityTime()
    {
        nextAbilityTime = Time.time + abilityCooldown;
    }

    /// <summary>
    /// Update the fire timer for auto weapons
    /// </summary>
    public virtual bool UpdateFireWeapon(
        Transform firePoint,
        Vector2 direction,
        Transform source,
        GameObject sourceObject,
        IProjectileDataProvider provider
    )
    {
        if (CanFire())
            return FireWeapon(firePoint, direction, source, sourceObject, provider);

        return false;
    }

    /// <summary>
    /// Implementation of IFireWeapon interface
    /// </summary>
    public virtual bool FireWeapon(
        Transform firePoint,
        Vector2 direction,
        Transform source,
        GameObject sourceObject,
        IProjectileDataProvider provider
    )
    {
        return FireProjectile(firePoint, direction, source, sourceObject, provider);
    }

    /// <summary>
    /// Apply velocity to a projectile in the specified direction
    /// </summary>
    protected virtual void ApplyVelocity(
        GameObject projectile,
        Vector2 direction,
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
    protected Vector2 ApplyAccuracySpread(Vector2 baseDirection, float spreadDegrees)
    {
        if (fireConfig.accuracy >= 1f)
            return baseDirection;

        float maxSpreadRadians =
            ConvertSpreadDegreesToRadians(spreadDegrees) * (1f - fireConfig.accuracy);

        // Generate a random angle within our spread cone
        float randomSpread = UnityEngine.Random.Range(
            -maxSpreadRadians / 2f,
            maxSpreadRadians / 2f
        );

        // Rotate our base direction by the spread amount
        float cos = Mathf.Cos(randomSpread);
        float sin = Mathf.Sin(randomSpread);
        return new Vector2(
            baseDirection.x * cos - baseDirection.y * sin,
            baseDirection.x * sin + baseDirection.y * cos
        );
    }

    private float ConvertSpreadDegreesToRadians(float spreadDegrees)
    {
        // Clamp the input to valid range
        spreadDegrees = Mathf.Clamp(spreadDegrees, 0f, 180f);

        // 180 degrees = PI radians
        return spreadDegrees * Mathf.PI / 180f;
    }

    public void UseUniqueAbility(IWeaponAbilityDataProvider provider)
    {
        UniqueAbility(provider);
    }

    protected abstract void UniqueAbility(IWeaponAbilityDataProvider provider);
}
