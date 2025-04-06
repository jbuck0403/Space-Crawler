using UnityEngine;

public class GrenadeProjectileBehavior : BaseProjectileBehavior
{
    private int numProjectiles = 5;
    private Vector2[] directions;
    private bool exploded = false;
    private DamageProfile damageProfile;
    private Transform source;
    private float fuseTime;

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
            this.fuseTime = fuseTime;
        }
    }

    private void Update()
    {
        // count down until explosion time
    }

    private void Explode()
    {
        if (!exploded)
        {
            for (int i = 0; i < numProjectiles; i++)
            {
                ProjectileSpawner.SpawnProjectile(transform, damageProfile, source);
            }

            exploded = true;
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
}
