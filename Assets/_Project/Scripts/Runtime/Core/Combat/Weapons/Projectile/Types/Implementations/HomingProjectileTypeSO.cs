using UnityEngine;

/// <summary>
/// Homing projectile type that tracks and follows a target.
/// Contains its own movement configuration for homing behavior.
/// </summary>
[CreateAssetMenu(
    fileName = "HomingProjectileType",
    menuName = "SpaceShooter/Projectile Types/Homing"
)]
public class HomingProjectileTypeSO : ProjectileTypeSO
{
    [Header("Homing Configuration")]
    [SerializeField]
    private MovementConfig movementConfig;

    protected override void ConfigureProjectile(
        Projectile projectile,
        IProjectileDataProvider dataProvider
    )
    {
        Transform target = dataProvider.GetTarget();

        if (target != null)
        {
            // add homing behavior with target and our movement config
            HomingProjectileBehavior behavior =
                projectile.gameObject.AddComponent<HomingProjectileBehavior>();
            behavior.Initialize(projectile, target, movementConfig);
        }
        else
        {
            Debug.LogWarning(
                "No target provided for homing projectile, skipping behavior addition"
            );
        }
    }
}
