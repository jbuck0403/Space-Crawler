using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "PistolWeapon", menuName = "SpaceShooter/Weapon Types/Pistol")]
public class PistolWeapon : BaseWeaponSO
{
    [SerializeField]
    private float dashForce = 20f;

    [SerializeField]
    private float dashDuration = 0.2f;

    public override bool FireWeapon(
        Transform firePoint,
        Vector2 direction,
        Transform source,
        GameObject sourceObject,
        IProjectileDataProvider provider
    )
    {
        bool fired = base.FireWeapon(firePoint, direction, source, sourceObject, provider);
        Debug.Log("!!!PISTOL FIRED - Updating fire time");
        UpdateNextFireTime(sourceObject);

        return fired;
    }

    protected override void UniqueAbility(IWeaponAbilityDataProvider provider)
    {
        MovementHandler movementHandler = provider.GetMovementHandler();

        Vector2 dashDirection = MovementUtils.GetTargetDirection(
            provider.GetWeaponOwnerTransform().position,
            provider.GetAbilityTarget()
        );

        provider.SetApplyingExternalMovement(true, dashDuration);

        movementHandler.ApplyVelocity(dashDirection * dashForce);

        UpdateNextAbilityTime();
    }
}
