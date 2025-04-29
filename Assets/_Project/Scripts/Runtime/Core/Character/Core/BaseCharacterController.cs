using System;
using UnityEngine;

public abstract class BaseCharacterController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    protected MovementConfig movementConfig;

    [NonSerialized]
    public WeaponHandler weaponHandler;

    [SerializeField]
    public LayerMask obstacleLayers;

    protected bool shooting = false;

    protected bool shootingDisabledExternally = false;

    protected virtual void Awake()
    {
        weaponHandler = GetComponent<WeaponHandler>();
    }

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
