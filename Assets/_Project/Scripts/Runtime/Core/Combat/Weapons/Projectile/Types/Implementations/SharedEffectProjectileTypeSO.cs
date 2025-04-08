using UnityEngine;

/// <summary>
/// Projectile type that applies status effects to both the target that's hit AND the character that shot it.
/// Can use either damage profiles or direct status effects.
/// </summary>
[CreateAssetMenu(
    fileName = "SharedEffectProjectileType",
    menuName = "SpaceShooter/Projectile Types/SharedEffect"
)]
public class SharedEffectProjectileTypeSO : ProjectileTypeSO
{
    [Header("Shooter Effect Configuration")]
    [Tooltip(
        "Optional separate damage profile to apply to the shooter. If null, no damage profile effects will be applied."
    )]
    [SerializeField]
    private DamageProfile shooterDamageProfile;

    [Tooltip("Direct status effects to apply to the shooter, independent of damage profile")]
    [SerializeField]
    private StatusEffectData[] shooterStatusEffects;

    public override Projectile SpawnProjectile(
        Transform firePoint,
        Transform source,
        IProjectileDataProvider dataProvider
    )
    {
        Projectile projectile = base.SpawnProjectile(firePoint, source, dataProvider);

        if (source != null)
        {
            // Apply damage profile effects if configured
            if (shooterDamageProfile != null)
            {
                DamageData shooterDamageData = shooterDamageProfile.CreateDamageData(source);
                shooterDamageData.ApplyAllStatusEffects(source.gameObject);
            }

            // Apply direct status effects if any
            if (shooterStatusEffects != null && shooterStatusEffects.Length > 0)
            {
                foreach (var effect in shooterStatusEffects)
                {
                    if (effect != null)
                    {
                        effect.SetSource(source);
                        effect.ApplyStatusEffect(source.gameObject);
                    }
                }
            }
        }

        return projectile;
    }

    protected override void ConfigureProjectile(
        Projectile projectile,
        IProjectileDataProvider dataProvider
    ) { }
}
