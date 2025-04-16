using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "SpaceShooter/Weapons/Base Weapon")]
public abstract class BaseWeaponSO : ScriptableObject, IModifiable
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

    public Dictionary<ModifierType, List<(object Source, Delegate Modifier)>> modifiers =
        new Dictionary<ModifierType, List<(object Source, Delegate Modifier)>>();
    public Dictionary<ModifierType, List<(object Source, Delegate Modifier)>> Modifiers =>
        modifiers;

    /// <summary>
    /// Initialize the weapon with necessary references and return an instance
    /// </summary>
    public virtual BaseWeaponSO Initialize(WeaponHandler weaponHandler)
    {
        BaseWeaponSO instance = Instantiate(this);

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
        // TBI Skill Point Delegate: BEFORE_PROJECTILE_CREATION
        Projectile projectile = GetProjectile(firePoint, source, provider);

        float newDamageModifier = damageModifier;
        foreach (
            var modifier in ModifierHelper.GetModifiers<ModifierHelper.FloatInFloatOutModifier>(
                this,
                ModifierType.WEAPON_DAMAGE_MODIFIER
            )
        )
        {
            newDamageModifier = modifier(newDamageModifier);
        }

        if (newDamageModifier != 1f)
        {
            // TBI Skill Point Delegate: WEAPON_DAMAGE_MODIFIER
            DamageData damageData = projectile.damageData;
            float newDamage = damageData.Amount * newDamageModifier;

            projectile.damageData = projectileType.damageProfile.CreateDamageData(
                source,
                damage: newDamage
            );
        }

        if (projectile != null)
        {
            Vector2 modifiedDirection = ProjectileSpawner.ApplyAccuracySpread(
                fireConfig,
                direction
            );
            // TBI Skill Point Delegate: PROJECTILE_VELOCITY_MODIFIER
            ProjectileSpawner.ApplyVelocity(
                projectile.gameObject,
                modifiedDirection,
                fireConfig,
                velocityModifier
            );

            // Raise event on the source object
            OnFireWeapon.Raise(sourceObject);
            // TBI Skill Point Delegate: AFTER_WEAPON_FIRE

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
        float modifiedFireRate = fireConfig.fireRate;

        // Add this debug info
        Debug.Log(
            $"*** Weapon {name} has {(modifiers.ContainsKey(ModifierType.AUTO_FIRE_RATE_MODIFIER) ? modifiers[ModifierType.AUTO_FIRE_RATE_MODIFIER].Count : 0)} fire rate modifiers"
        );

        // Print memory address to verify we're using the right instance
        Debug.Log($"*** Weapon dict memory location: {modifiers.GetHashCode()}");

        foreach (
            var modifier in ModifierHelper.GetModifiers<ModifierHelper.FloatInFloatOutModifier>(
                this,
                ModifierType.AUTO_FIRE_RATE_MODIFIER
            )
        )
        {
            modifiedFireRate = modifier(modifiedFireRate);
        }

        nextFireTime = Time.time + modifiedFireRate;
    }

    public bool CanActivateAbility()
    {
        // TBI Skill Point Delegate: ABILITY_COOLDOWN_CHECK
        return Time.time >= nextAbilityTime;
    }

    protected void UpdateNextAbilityTime()
    {
        // TBI Skill Point Delegate: ABILITY_COOLDOWN_MODIFIER
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
        {
            // TBI Skill Point Delegate: AUTO_FIRE_RATE_MODIFIER
            return FireWeapon(firePoint, direction, source, sourceObject, provider);
        }

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
        // TBI Skill Point Delegate: PRE_WEAPON_FIRE
        return FireProjectile(firePoint, direction, source, sourceObject, provider);
    }

    // /// <summary>
    // /// Apply velocity to a projectile in the specified direction
    // /// </summary>
    // protected virtual void ApplyVelocity(
    //     GameObject projectile,
    //     Vector2 direction,
    //     float velocityOverload = 1f
    // )
    // {
    //     if (projectile == null)
    //         return;

    //     Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
    //     if (rb != null)
    //     {
    //         float projectileSpeed =
    //             velocityOverload == 1f
    //                 ? fireConfig.projectileSpeed * velocityModifier
    //                 : velocityOverload * velocityModifier;

    //         rb.velocity = projectileSpeed * direction.normalized;
    //     }
    //     else
    //     {
    //         Debug.LogWarning(
    //             $"Projectile {projectile.name} has no Rigidbody2D component to apply velocity to"
    //         );
    //     }
    // }

    // /// <summary>
    // /// Apply accuracy spread to the direction vector
    // /// </summary>
    // protected Vector2 ApplyAccuracySpread(Vector2 baseDirection, float spreadDegrees)
    // {
    //     if (fireConfig.accuracy >= 1f)
    //         return baseDirection;

    //     float maxSpreadRadians =
    //         ConvertSpreadDegreesToRadians(spreadDegrees) * (1f - fireConfig.accuracy);

    //     // Generate a random angle within our spread cone
    //     float randomSpread = UnityEngine.Random.Range(
    //         -maxSpreadRadians / 2f,
    //         maxSpreadRadians / 2f
    //     );

    //     // Rotate our base direction by the spread amount
    //     float cos = Mathf.Cos(randomSpread);
    //     float sin = Mathf.Sin(randomSpread);
    //     return new Vector2(
    //         baseDirection.x * cos - baseDirection.y * sin,
    //         baseDirection.x * sin + baseDirection.y * cos
    //     );
    // }

    // private float ConvertSpreadDegreesToRadians(float spreadDegrees)
    // {
    //     // Clamp the input to valid range
    //     spreadDegrees = Mathf.Clamp(spreadDegrees, 0f, 180f);

    //     // 180 degrees = PI radians
    //     return spreadDegrees * Mathf.PI / 180f;
    // }

    public void UseUniqueAbility(IWeaponAbilityDataProvider provider)
    {
        // TBI Skill Point Delegate: BEFORE_WEAPON_ABILITY
        UniqueAbility(provider);
        // TBI Skill Point Delegate: AFTER_WEAPON_ABILITY
    }

    protected abstract void UniqueAbility(IWeaponAbilityDataProvider provider);
}
