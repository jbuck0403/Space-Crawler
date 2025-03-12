using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseWeapon))]
public abstract class BaseCharacterController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    protected MovementConfig movementConfig;

    protected BaseWeapon weapon;

    protected virtual void Awake()
    {
        weapon = GetComponent<BaseWeapon>();
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

    protected virtual void FireWeapon()
    {
        weapon.SetCanFire(true);
    }

    protected virtual void StopFiringWeapon()
    {
        weapon.SetCanFire(false);
    }
}
