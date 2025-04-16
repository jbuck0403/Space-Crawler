using UnityEngine;

[CreateAssetMenu(fileName = "ShotgunWeapon", menuName = "SpaceShooter/Weapon Types/Shotgun")]
public class ShotgunWeapon : BaseWeaponSO
{
    [SerializeField]
    private int numFragments = 8;

    [SerializeField]
    private float fuseTime = 2f;

    [Header("Grenade Ability Settings")]
    [SerializeField]
    private DamageProfile grenadeProjectileDamageProfile;

    [SerializeField]
    private FireConfig grenadeProjectileFireConfig;

    [SerializeField]
    private DamageProfile grenadeShrapnelDamageProfile;

    [SerializeField]
    private FireConfig grenadeShrapnelFireConfig;

    public override bool FireWeapon(
        Transform firePoint,
        Vector2 direction,
        Transform source,
        GameObject sourceObject,
        IProjectileDataProvider provider
    )
    {
        bool fired1 = base.FireWeapon(firePoint, direction, source, sourceObject, provider);
        bool fired2 = base.FireWeapon(firePoint, direction, source, sourceObject, provider);
        bool fired3 = base.FireWeapon(firePoint, direction, source, sourceObject, provider);

        UpdateNextFireTime(sourceObject);

        return fired1 && fired2 && fired3;
    }

    protected override void UniqueAbility(IWeaponAbilityDataProvider provider)
    {
        Transform firePoint = provider.GetFirePoint();
        Transform source = provider.GetWeaponOwnerTransform();

        Vector2 direction = MovementUtils.GetTargetDirection(
            firePoint.position,
            provider.GetAbilityTarget()
        );

        object[] parameters = new object[]
        {
            numFragments,
            grenadeShrapnelDamageProfile,
            source,
            fuseTime,
            grenadeShrapnelFireConfig
        };

        Projectile grenadeProjectile =
            ProjectileSpawner.SpawnProjectileWithBehavior<GrenadeProjectileBehavior>(
                firePoint,
                grenadeProjectileDamageProfile,
                source,
                parameters
            );

        // launch the grenade
        if (grenadeProjectile != null)
        {
            ProjectileSpawner.ApplyVelocity(
                grenadeProjectile.gameObject,
                direction,
                grenadeProjectileFireConfig,
                velocityModifier
            );

            UpdateNextAbilityTime();
        }
    }
}
