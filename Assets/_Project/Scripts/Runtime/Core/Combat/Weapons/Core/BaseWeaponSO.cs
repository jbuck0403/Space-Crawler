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

    protected WeaponHandler weaponHandler;

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

        instance.weaponHandler = weaponHandler;
        instance.nextFireTime = 0f;
        instance.isInitialized = true;

        return instance;
    }

    public virtual void NotifyWeaponStoppedFiring() { }

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
        Projectile projectile = projectileType.SpawnProjectile(
            weaponHandler,
            firePoint,
            source,
            provider
        );

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

            // OnFireWeapon.Raise(sourceObject);
            MuzzleFlareFXData fxData = new MuzzleFlareFXData(
                projectileType.projectileVFXPrefabs.muzzleFlashPrefab,
                source,
                firePoint
            );

            if (weaponHandler != null && weaponHandler.OnMuzzleFlareFX != null)
                weaponHandler.OnMuzzleFlareFX.Raise(sourceObject, fxData);
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

    protected float UpdateNextFireTime(GameObject source)
    {
        float modifiedFireRate = fireConfig.fireRate;

        // Apply modifiers to fire rate
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

        Debug.Log($"!!!WEAPON: Handler={weaponHandler != null}, FireRate={modifiedFireRate}");
        if (weaponHandler != null)
        {
            Debug.Log("!!!WEAPON: Raising event");
            weaponHandler.OnNextFireTime.Raise(source, modifiedFireRate);
        }

        return nextFireTime;
    }

    public bool CanActivateAbility()
    {
        // TBI Skill Point Delegate: ABILITY_COOLDOWN_CHECK
        return Time.time >= nextAbilityTime;
    }

    protected void UpdateNextAbilityTime(GameObject source)
    {
        // TBI Skill Point Delegate: ABILITY_COOLDOWN_MODIFIER
        nextAbilityTime = Time.time + abilityCooldown;

        if (weaponHandler != null)
            weaponHandler.OnWeaponAbilityCooldown.Raise(source, abilityCooldown);
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

    public void UseUniqueAbility(IWeaponAbilityDataProvider provider)
    {
        // TBI Skill Point Delegate: BEFORE_WEAPON_ABILITY
        UniqueAbility(provider);
        // TBI Skill Point Delegate: AFTER_WEAPON_ABILITY
        UpdateNextAbilityTime(provider.GetWeaponOwnerTransform().gameObject);
    }

    public void SetProjectileType(ProjectileTypeSO projectileTypeSO)
    {
        projectileType = projectileTypeSO;
    }

    protected abstract void UniqueAbility(IWeaponAbilityDataProvider provider);
}
