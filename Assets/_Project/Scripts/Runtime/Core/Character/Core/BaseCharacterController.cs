using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [RequireComponent(typeof(BaseWeapon))]
public abstract class BaseCharacterController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    protected MovementConfig movementConfig;

    protected BaseWeapon weapon;

    protected bool shooting = false;

    protected bool shootingDisabledExternally = false;

    protected virtual void Awake()
    {
        weapon = GetComponent<BaseWeapon>();
    }

    protected virtual void Start()
    {
        InitializeWeapon();
    }

    protected virtual void InitializeWeapon()
    {
        if (
            weapon != null
            && weapon.weaponConfig != null
            && weapon.weaponConfig.firingStrategies != null
            && weapon.weaponConfig.firingStrategies.Count > 0
        )
        {
            weapon.SetStrategy(weapon.weaponConfig.firingStrategies[0]);
        }
    }

    public void EnableShooting(bool shooting, bool external = false)
    {
        if (weapon != null)
        {
            this.shooting = shooting;
            shootingDisabledExternally = external;
        }
    }

    protected virtual void HandleShooting(Transform target = null)
    {
        if (weapon != null)
        {
            if (shooting)
            {
                if (!weapon.GetFiring())
                {
                    FireWeapon(target);
                }
            }
            else
            {
                if (weapon.GetFiring())
                {
                    StopFiringWeapon();
                }
            }
        }
    }

    protected virtual void FireWeapon(Transform target = null)
    {
        if (weapon != null)
        {
            weapon.SetCanFire(true);
        }
    }

    protected virtual void StopFiringWeapon()
    {
        if (weapon != null)
            weapon.SetCanFire(false);
    }

    public MovementConfig GetMovementConfig()
    {
        return movementConfig;
    }

    public abstract MovementHandler GetMovementHandler();
}
