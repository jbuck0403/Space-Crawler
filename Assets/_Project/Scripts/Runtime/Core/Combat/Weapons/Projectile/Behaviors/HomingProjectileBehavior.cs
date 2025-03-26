using UnityEngine;

/// <summary>
/// Projectile behavior that homes in on a target using the provided movement configuration.
/// </summary>
public class HomingProjectileBehavior : MonoBehaviour, IProjectileBehavior
{
    protected Projectile projectile;
    protected Transform target;
    protected MovementConfig movementConfig;
    protected MovementHandler movementHandler;
    protected float currentSpeed;

    protected Rigidbody2D projectileRB;

    public void Initialize(Projectile projectile, object[] behaviorParams)
    {
        this.projectile = projectile;

        // Extract parameters
        if (behaviorParams.Length >= 2)
        {
            target = behaviorParams[0] as Transform;
            movementConfig = behaviorParams[1] as MovementConfig;
            projectileRB = projectile.gameObject.GetComponent<Rigidbody2D>();
            currentSpeed = projectileRB.velocity.magnitude; // Maintain initial speed

            if (movementConfig != null)
            {
                movementHandler = new MovementHandler(movementConfig);
                // Initialize movement handler with current velocity
                movementHandler.ApplyVelocity(projectileRB.velocity);
            }
        }
        else
        {
            Debug.LogError("HomingProjectileBehavior initialized without required parameters");
        }
    }

    public void Cleanup()
    {
        target = null;
        Destroy(this);
    }

    protected void FixedUpdate()
    {
        if (projectile == null || movementHandler == null)
            return;

        if (target != null && target.gameObject.activeInHierarchy)
        {
            // Calculate direction to target
            Vector2 directionToTarget = (
                target.position - projectile.transform.position
            ).normalized;

            // Use MovementHandler to update velocity and rotation
            movementHandler.AddToVelocity(directionToTarget);
            movementHandler.ApplyRotation(
                projectile.transform,
                directionToTarget,
                Time.fixedDeltaTime
            );

            // Update rigidbody with movement handler's velocity
            projectileRB.velocity = movementHandler.GetCurrentVelocity();
        }
    }
}

public class HomingProjectileMovementConfig
{
    public MovementConfig config;
    public Transform target;
}
