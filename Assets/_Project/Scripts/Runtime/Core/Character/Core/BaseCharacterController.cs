using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [RequireComponent(typeof(BaseWeapon))]
public abstract class BaseCharacterController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    protected MovementConfig movementConfig;

    protected WeaponHandler weaponHandler;

    protected bool shooting = false;

    protected bool shootingDisabledExternally = false;

    protected virtual void Awake()
    {
        weaponHandler = GetComponent<WeaponHandler>();
    }

    // protected virtual void Start()
    // {
    //     InitializeWeapon();
    // }

    // protected virtual void InitializeWeapon()
    // {
    //     if (
    //         weapon != null
    //         && weapon.weaponConfig != null
    //         && weapon.weaponConfig.firingStrategies != null
    //         && weapon.weaponConfig.firingStrategies.Count > 0
    //     )
    //     {
    //         weapon.SetStrategy(weapon.weaponConfig.firingStrategies[0]);
    //     }
    // }

    public void EnableShooting(bool enabled, bool external = false)
    {
        shooting = enabled;
        shootingDisabledExternally = external;
    }

    protected virtual void HandleShooting(Transform target = null)
    {
        if (weaponHandler != null)
        {
            if (shooting && !shootingDisabledExternally)
            {
                if (!weaponHandler.GetFiring() && weaponHandler.CanFire())
                {
                    FireWeapon(target);
                }
            }
            else
            {
                if (weaponHandler.GetFiring())
                {
                    StopFiringWeapon();
                }
            }
        }
    }

    protected virtual void FireWeapon(Transform target = null)
    {
        if (weaponHandler != null)
        {
            weaponHandler.SetTarget(target);
            weaponHandler.StartFiring();
        }
    }

    protected virtual void StopFiringWeapon()
    {
        if (weaponHandler != null)
            weaponHandler.StopFiring();
    }

    public MovementConfig GetMovementConfig()
    {
        return movementConfig;
    }

    public abstract MovementHandler GetMovementHandler();
}
