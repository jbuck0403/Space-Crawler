using System;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class WeaponHandler : MonoBehaviour, IProjectileDataProvider
{
    [Header("Weapon Configuration")]
    [SerializeField]
    private List<BaseWeaponSO> weaponDefinitions = new List<BaseWeaponSO>();

    [SerializeField]
    private Transform firePoint;

    [SerializeField]
    private int startingWeaponIndex = 0;

    [Header("Events")]
    [SerializeField]
    private VoidEvent OnWeaponSwitched;

    private List<BaseWeaponSO> weaponInstances = new List<BaseWeaponSO>();
    private int currentWeaponIndex;
    private BaseWeaponSO currentWeapon;
    private bool isFiring = false;
    private Transform currentTarget;

    public BaseWeaponSO CurrentWeapon => currentWeapon;
    public event Action<BaseWeaponSO> OnWeaponChanged;

    private void Awake()
    {
        InitializeWeapons();
    }

    private void Update()
    {
        // Handle auto firing for the current weapon if enabled
        if (isFiring && currentWeapon != null)
        {
            // Get the firing direction (up vector in 2D top-down)
            Vector2 direction = transform.up;

            // Update the weapon's firing state
            currentWeapon.UpdateFiring(firePoint, direction, transform, gameObject, this);
        }
    }

    public void InitializeWeapon(BaseWeaponSO weapon)
    {
        if (weapon == null)
            return;

        BaseWeaponSO instance = weapon.Initialize(this);
        weaponInstances.Add(instance);
    }

    private void InitializeWeapons()
    {
        if (weaponDefinitions.Count == 0)
        {
            Debug.LogWarning("No weapons available in WeaponHandler.");
            return;
        }

        // Initialize all weapons
        foreach (var weaponDef in weaponDefinitions)
        {
            InitializeWeapon(weaponDef);
        }

        // Use weaponInstances for actual weapon references
        currentWeaponIndex = Mathf.Clamp(startingWeaponIndex, 0, weaponInstances.Count - 1);
        SwitchToWeapon(currentWeaponIndex);
    }

    public void SwitchToWeapon(int index)
    {
        if (index < 0 || index >= weaponInstances.Count)
        {
            Debug.LogWarning($"Invalid weapon index: {index}");
            return;
        }

        currentWeaponIndex = index;
        currentWeapon = weaponInstances[index];

        // notify listeners
        OnWeaponChanged?.Invoke(currentWeapon);
        OnWeaponSwitched.Raise(gameObject);
    }

    public void SwitchToNextWeapon()
    {
        int nextIndex = (currentWeaponIndex + 1) % weaponInstances.Count;
        SwitchToWeapon(nextIndex);
    }

    public void SwitchToPreviousWeapon()
    {
        int prevIndex = (currentWeaponIndex - 1 + weaponInstances.Count) % weaponInstances.Count;
        SwitchToWeapon(prevIndex);
    }

    public void StartFiring()
    {
        isFiring = true;

        // If not using auto-fire, immediately fire once
        if (currentWeapon != null)
        {
            Vector2 direction = transform.up;
            currentWeapon.FireWeapon(firePoint, direction, transform, gameObject, this);
        }
    }

    public void StopFiring()
    {
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

    // Manual firing method (can be called from other scripts)
    public bool FireWeapon(Vector2? direction = null)
    {
        if (currentWeapon == null)
            return false;

        Vector2 fireDirection = direction ?? transform.up;
        return currentWeapon.FireWeapon(firePoint, fireDirection, transform, gameObject, this);
    }

#if ENABLE_INPUT_SYSTEM
    // Input System integration
    public void OnFire(InputValue value)
    {
        if (value.isPressed)
        {
            StartFiring();
        }
        else
        {
            StopFiring();
        }
    }

    public void OnSwitchWeapon(InputValue value)
    {
        float direction = value.Get<float>();

        if (direction > 0)
        {
            SwitchToNextWeapon();
        }
        else if (direction < 0)
        {
            SwitchToPreviousWeapon();
        }
    }
#endif
}
