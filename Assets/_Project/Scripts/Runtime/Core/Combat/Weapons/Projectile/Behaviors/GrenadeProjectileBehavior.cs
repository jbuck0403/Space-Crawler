using UnityEngine;

public class GrenadeProjectileBehavior : BaseProjectileBehavior
{
    private int numProjectiles = 5;
    private bool exploded = false;
    private DamageProfile damageProfile;
    private Transform source;
    private float fuseTime;
    private FireConfig fireConfig;

    private Projectile grenadeProjectile;
    private ProjectileTypeSO grenadeProjectileTypeSO;

    protected override void InitializeFromParams(object[] parameters)
    {
        if (parameters[0] is int numProjectiles)
            this.numProjectiles = numProjectiles;

        if (parameters[1] is DamageProfile damageProfile)
        {
            this.damageProfile = damageProfile;
        }

        if (parameters[2] is Transform source)
        {
            this.source = source;
        }

        if (parameters[3] is float fuseTime)
        {
            this.fuseTime = Time.time + fuseTime;
        }

        if (parameters[4] is FireConfig fireConfig)
        {
            this.fireConfig = fireConfig;
        }

        grenadeProjectile = GetComponent<Projectile>();
    }

    private void Update()
    {
        if (Time.time >= fuseTime)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (!exploded)
        {
            for (int i = 0; i < numProjectiles; i++)
            {
                Projectile projectile = ProjectileSpawner.SpawnProjectile(
                    transform,
                    damageProfile,
                    source,
                    grenadeProjectileTypeSO.projectileVFXPrefabs
                );
                Vector2[] projectileDirections = GetProjectileDirections();

                ProjectileSpawner.ApplyVelocity(
                    projectile.gameObject,
                    projectileDirections[i],
                    fireConfig
                );
            }

            exploded = true;

            if (grenadeProjectile != null)
                grenadeProjectile.DestroyProjectile();
        }
    }

    private Vector2[] GetProjectileDirections()
    {
        Vector2[] directions = new Vector2[numProjectiles];

        // Calculate angle between each projectile (in radians)
        float angleStep = 2f * Mathf.PI / numProjectiles;

        for (int i = 0; i < numProjectiles; i++)
        {
            // Calculate angle for this projectile
            float angle = i * angleStep;

            // Convert angle to direction vector
            // Using sin/cos gives us points on a circle
            directions[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        return directions;
    }

    public override void OnHit()
    {
        Explode();
    }
}
