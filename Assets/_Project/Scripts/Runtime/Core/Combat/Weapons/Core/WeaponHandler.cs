using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class WeaponHandler : MonoBehaviour, IProjectileDataProvider, IModifiable
{
    [Header("Weapon Configuration")]
    [SerializeField]
    private List<BaseWeaponSO> weaponDefinitions = new List<BaseWeaponSO>();

    [SerializeField]
    private List<ProjectileTypeSO> projectileTypes = new List<ProjectileTypeSO>();

    [SerializeField]
    private Transform firePoint;

    [SerializeField]
    private int startingWeaponIndex = 0;

    [Header("Events")]
    [SerializeField]
    private VoidEvent OnWeaponSwitched;

    private List<BaseWeaponSO> weaponInstances = new List<BaseWeaponSO>();
    private int currentWeaponIndex;
    private int currentProjectileIndex;
    private BaseWeaponSO currentWeapon;
    private bool isFiring = false;
    private Transform currentTarget;

    public BaseWeaponSO CurrentWeapon => currentWeapon;

    public Transform FirePoint => firePoint;
    public List<BaseWeaponSO> WeaponInstances => weaponInstances;

    public FloatEvent OnNextFireTime;
    public FloatEvent OnChargingWeapon;
    public FloatEvent OnWeaponAbilityCooldown;
    public OnHitFXEvent OnHitFX;
    public MuzzleFlareFXEvent OnMuzzleFlareFX;
    public VoidEvent OnFireWeapon;
    public WeaponTypeEvent OnWeaponSwapped;
    public WeaponTypeEvent OnWeaponInitialized;

    public AmmoTypeEvent OnAmmoSwapped;
    public AmmoTypeEvent OnAmmoInitialized;

    public Dictionary<ModifierType, List<(object Source, Delegate Modifier)>> modifiers =
        new Dictionary<ModifierType, List<(object Source, Delegate Modifier)>>();
    public Dictionary<ModifierType, List<(object Source, Delegate Modifier)>> Modifiers =>
        modifiers;

    [SerializeField]
    private List<WeaponType> unlockedWeaponTypes = new List<WeaponType>();
    private List<AmmoType> unlockedAmmoTypes = new List<AmmoType>();

    // Track which ammo type each weapon is using
    private Dictionary<WeaponType, AmmoType> weaponAmmoSelections =
        new Dictionary<WeaponType, AmmoType>();

    private void Awake()
    {
        Debug.Log($"!!!HANDLER: Event={OnNextFireTime != null}");
        LoadWeaponsAndAmmo();
        InitializeWeapons();
        InitializeListeners();
    }

    private void LoadWeaponsAndAmmo()
    {
        unlockedWeaponTypes.Add(WeaponType.Pistol);
        unlockedAmmoTypes.Add(AmmoType.Basic);

        GameData gameData = GameData.LoadGameData();
        if (gameData != null)
        {
            UnlockLoadedWeapons(gameData);
            UnlockLoadedAmmo(gameData);
        }
    }

    private void UnlockLoadedWeapons(GameData gameData)
    {
        foreach (WeaponType weaponType in gameData.unlockedWeaponTypes)
        {
            if (!unlockedWeaponTypes.Contains(weaponType))
                unlockedWeaponTypes.Add(weaponType);
        }
    }

    private void UnlockLoadedAmmo(GameData gameData)
    {
        foreach (AmmoType ammoType in gameData.unlockedAmmoTypes)
        {
            if (!unlockedAmmoTypes.Contains(ammoType))
                unlockedAmmoTypes.Add(ammoType);
        }
    }

    private void InitializeListeners()
    {
        OnFireWeapon.AddListener(gameObject, () => AudioManager.PlayWeaponFire());
    }

    private void Update()
    {
        if (currentWeapon != null)
        {
            if (isFiring)
            {
                Vector2 direction = transform.up;

                currentWeapon.UpdateFireWeapon(firePoint, direction, transform, gameObject, this);
            }
            else if (!isFiring)
            {
                currentWeapon.NotifyWeaponStoppedFiring();
            }
        }
    }

    public void InitializeWeapon(BaseWeaponSO weapon)
    {
        if (weapon == null)
            return;

        BaseWeaponSO instance = weapon.Initialize(this);

        if (!weaponInstances.Contains(instance))
            weaponInstances.Add(instance);

        OnWeaponInitialized.Raise(gameObject, instance.weaponType);
    }

    public void InitializeAmmo(AmmoType ammoType)
    {
        if (unlockedAmmoTypes.Contains(ammoType))
            return;
        else
        {
            unlockedAmmoTypes.Add(ammoType);
            OnAmmoInitialized.Raise(gameObject, ammoType);
        }
    }

    public bool CanFire()
    {
        if (currentWeapon != null)
            return currentWeapon.CanFire();

        return false;
    }

    private void InitializeWeapons()
    {
        if (weaponDefinitions.Count == 0)
        {
            Debug.LogWarning("No weapons available in WeaponHandler.");
            return;
        }

        foreach (var weaponDef in weaponDefinitions)
        {
            if (unlockedWeaponTypes.Contains(weaponDef.weaponType))
            {
                InitializeWeapon(weaponDef);
            }
        }

        currentWeaponIndex = Mathf.Clamp(startingWeaponIndex, 0, weaponInstances.Count - 1);
        SwitchToWeapon(currentWeaponIndex);
    }

    public bool SwitchToProjectile(int index)
    {
        if (index < 0 || index >= projectileTypes.Count)
        {
            return false;
        }

        AmmoType ammoType = projectileTypes[index].ammoType;
        if (!HasAmmo(ammoType))
        {
            return false;
        }

        currentProjectileIndex = index;
        currentWeapon.SetProjectileType(projectileTypes[index]);

        // Track which ammo this weapon is using
        if (currentWeapon != null)
        {
            weaponAmmoSelections[currentWeapon.weaponType] = ammoType;
            // Notify UI that ammo has changed
            OnAmmoSwapped.Raise(gameObject, ammoType);
        }

        return true;
    }

    public bool SwitchToNextProjectile(bool reverse = false)
    {
        int startIndex = currentProjectileIndex;
        int nextIndex;

        do
        {
            nextIndex = (currentProjectileIndex + (reverse ? -1 : 1)) % projectileTypes.Count;
            if (nextIndex < 0)
                nextIndex += projectileTypes.Count;

            currentProjectileIndex = nextIndex;

            if (AmmoUnlocked(nextIndex))
            {
                SwitchToProjectile(nextIndex);
                return true;
            }
        } while (nextIndex != startIndex);

        return false;
    }

    public bool SwitchToPreviousProjectile()
    {
        return SwitchToNextProjectile(true);
    }

    public bool SwitchToWeapon(int index)
    {
        if (index < 0 || index >= weaponInstances.Count)
        {
            Debug.LogWarning($"Invalid weapon index: {index}");
            return false;
        }

        currentWeaponIndex = index;
        currentWeapon = weaponInstances[index];

        OnWeaponSwapped.Raise(gameObject, currentWeapon.weaponType);
        OnWeaponSwitched.Raise(gameObject);

        // Also raise the ammo swapped event with the current weapon's ammo
        AmmoType currentAmmoType = GetCurrentAmmoType();
        OnAmmoSwapped.Raise(gameObject, currentAmmoType);

        return true;
    }

    public bool SwitchToNextWeapon()
    {
        int nextIndex = (currentWeaponIndex + 1) % weaponInstances.Count;
        return SwitchToWeapon(nextIndex);
    }

    public bool SwitchToPreviousWeapon()
    {
        int prevIndex = (currentWeaponIndex - 1 + weaponInstances.Count) % weaponInstances.Count;
        return SwitchToWeapon(prevIndex);
    }

    public void StartFiring()
    {
        // TBI Skill Point Delegate: BEFORE_WEAPON_FIRING
        print("Started Firing");
        if (!isFiring && currentWeapon != null)
        {
            isFiring = FireWeapon();
        }
    }

    public void StopFiring()
    {
        // TBI Skill Point Delegate: ON_WEAPON_STOP_FIRING
        isFiring = false;
    }

    public bool GetFiring()
    {
        return isFiring;
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
    }

    // IProjectileDataProvider implementation
    public Transform GetTarget()
    {
        return currentTarget;
    }

    public float GetDistanceToTarget()
    {
        if (currentTarget == null)
            return 0f;

        return Vector2.Distance(transform.position, currentTarget.position);
    }

    public bool FireWeapon(Vector2? direction = null)
    {
        if (currentWeapon == null)
            return false;

        Vector2 fireDirection = direction ?? transform.up;
        return currentWeapon.FireWeapon(firePoint, fireDirection, transform, gameObject, this);
    }

    public void ActivateWeaponAbility(IWeaponAbilityDataProvider provider)
    {
        if (currentWeapon != null && currentWeapon.CanActivateAbility())
        {
            currentWeapon.UseUniqueAbility(provider);
        }
    }

    public void RaiseOnFireWeaponEvent()
    {
        if (OnFireWeapon != null && CanFire())
            OnFireWeapon.Raise(gameObject);
    }

    public bool HasWeapon(BaseWeaponSO baseWeaponSO)
    {
        foreach (var weapon in weaponInstances)
        {
            if (weapon.weaponType == baseWeaponSO.weaponType)
                return true;
        }
        return false;
    }

    public bool HasAmmo(AmmoType ammoType)
    {
        return unlockedAmmoTypes.Contains(ammoType);
    }

    private bool AmmoUnlocked(int index)
    {
        if (index >= projectileTypes.Count || index < 0)
            return false;

        return HasAmmo(projectileTypes[index].ammoType);
    }

    // Get the current ammo type for the active weapon
    public AmmoType GetCurrentAmmoType()
    {
        if (currentWeapon == null)
        {
            return AmmoType.Basic; // Default
        }

        // Get the saved ammo type for this weapon, or default to Basic
        if (weaponAmmoSelections.TryGetValue(currentWeapon.weaponType, out AmmoType ammoType))
        {
            return ammoType;
        }

        return AmmoType.Basic;
    }
}

public enum WeaponType
{
    Pistol,
    Shotgun,
    Sniper,
}

public enum AmmoType
{
    Basic,
    Burning,
    Freezing,
}
