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
    public event Action<BaseWeaponSO> OnWeaponChanged;
    public Transform FirePoint => firePoint;
    public List<BaseWeaponSO> WeaponInstances => weaponInstances;

    public FloatEvent OnNextFireTime;
    public FloatEvent OnChargingWeapon;
    public FloatEvent OnWeaponAbilityCooldown;
    public OnHitFXEvent OnHitFX;
    public MuzzleFlareFXEvent OnMuzzleFlareFX;
    public VoidEvent OnFireWeapon;

    public Dictionary<ModifierType, List<(object Source, Delegate Modifier)>> modifiers =
        new Dictionary<ModifierType, List<(object Source, Delegate Modifier)>>();
    public Dictionary<ModifierType, List<(object Source, Delegate Modifier)>> Modifiers =>
        modifiers;

    private List<WeaponType> unlockedWeaponTypes = new List<WeaponType>();

    private void Awake()
    {
        Debug.Log($"!!!HANDLER: Event={OnNextFireTime != null}");
        LoadUnlockedWeapons();
        InitializeWeapons();
        InitializeListeners();
    }

    private void LoadUnlockedWeapons()
    {
        unlockedWeaponTypes.Add(WeaponType.pistol);

        GameData gameData = GameData.LoadGameData();
        if (gameData != null)
        {
            foreach (WeaponType weaponType in gameData.unlockedWeaponTypes)
            {
                if (unlockedWeaponTypes.Contains(weaponType))
                    return;

                unlockedWeaponTypes.Add(weaponType);
            }
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
    }

    public bool CanFire()
    {
        return currentWeapon.CanFire();
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
        if (index < 0)
        {
            return false;
        }

        currentProjectileIndex = index;
        currentWeapon.SetProjectileType(projectileTypes[index]);

        return true;
    }

    public bool SwitchToNextProjectile()
    {
        int nextIndex = (currentProjectileIndex + 1) % projectileTypes.Count;
        return SwitchToProjectile(nextIndex);
    }

    public bool SwitchToPreviousProjectile()
    {
        int prevIndex =
            (currentProjectileIndex - 1 + projectileTypes.Count) % projectileTypes.Count;
        return SwitchToProjectile(prevIndex);
    }

    public bool SwitchToWeapon(int index)
    {
        if (index < weaponInstances.Count)
            print($"Swapping to {weaponInstances[index].name}");

        if (index < 0 || index >= weaponInstances.Count)
        {
            Debug.LogWarning($"Invalid weapon index: {index}");
            return false;
        }

        currentWeaponIndex = index;
        currentWeapon = weaponInstances[index];

        OnWeaponChanged?.Invoke(currentWeapon);
        OnWeaponSwitched.Raise(gameObject);

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
            // if (isFiring)
            //     OnFireWeapon.Raise(gameObject);
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
        return weaponInstances.Contains(baseWeaponSO);
    }
}

public enum WeaponType
{
    pistol,
    shotgun,
    sniper,
}
