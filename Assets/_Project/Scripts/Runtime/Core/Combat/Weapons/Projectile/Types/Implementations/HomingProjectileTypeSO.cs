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
        // Get target from data provider
        Transform target = dataProvider.GetTarget();

        if (target != null)
        {
            // Add homing behavior with target and our movement config
            var behavior = projectile.gameObject.AddComponent<HomingProjectileBehavior>();
            behavior.Initialize(projectile, new object[] { target, movementConfig });
        }
        else
        {
            Debug.LogWarning(
                "No target provided for homing projectile, skipping behavior addition"
            );
        }
    }
}
