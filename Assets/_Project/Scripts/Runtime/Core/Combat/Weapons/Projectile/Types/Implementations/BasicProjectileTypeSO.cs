using UnityEngine;

/// <summary>
/// Basic projectile type that moves in a straight line.
/// Uses ProjectileSpawner's basic functionality with no additional behavior.
/// </summary>
[CreateAssetMenu(
    fileName = "BasicProjectileType",
    menuName = "SpaceShooter/Projectile Types/Basic"
)]
public class BasicProjectileTypeSO : ProjectileTypeSO
{
    protected override void ConfigureProjectile(
        Projectile projectile,
        IProjectileDataProvider dataProvider
    ) { }
}
