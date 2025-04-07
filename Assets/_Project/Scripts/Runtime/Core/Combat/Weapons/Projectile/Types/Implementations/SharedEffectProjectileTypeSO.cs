using UnityEngine;

/// <summary>
/// Projectile type that applies status effects to both the target that's hit AND the character that shot it.
/// Can use either the same damage profile for both or a separate one for the shooter.
/// </summary>
[CreateAssetMenu(
    fileName = "SharedEffectProjectileType",
    menuName = "SpaceShooter/Projectile Types/SharedEffect"
)]
public class SharedEffectProjectileTypeSO : ProjectileTypeSO
{
    [Header("Shooter Effect Configuration")]
    [Tooltip(
        "Optional separate damage profile to apply to the shooter. If null, the projectile's damage profile will be used."
    )]
    [SerializeField]
    private DamageProfile shooterDamageProfile;

    public override Projectile SpawnProjectile(
        Transform firePoint,
        Transform source,
        IProjectileDataProvider dataProvider
    )
    {
        Projectile projectile = base.SpawnProjectile(firePoint, source, dataProvider);

        if (source != null)
        {
            DamageProfile profileToUse =
                shooterDamageProfile != null ? shooterDamageProfile : damageProfile;

            DamageData shooterDamageData = profileToUse.CreateDamageData(source);

            shooterDamageData.ApplyAllStatusEffects(source.gameObject);
        }

        return projectile;
    }

    protected override void ConfigureProjectile(
        Projectile projectile,
        IProjectileDataProvider dataProvider
    ) { }
}
