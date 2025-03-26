using System;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Projectile behavior that homes in on a target using the provided movement configuration.
/// </summary>
public class HomingProjectileBehavior : BaseProjectileBehavior
{
    protected Transform target;
    protected MovementConfig movementConfig;
    protected MovementHandler movementHandler;
    protected float currentSpeed;
    protected Rigidbody2D projectileRB;

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

    /// <summary>
    /// Type-safe initialization method that can be called directly
    /// </summary>
    public void Initialize(Projectile projectile, Transform target, MovementConfig movementConfig)
    {
        this.projectile = projectile;
        this.target = target;
        this.movementConfig = movementConfig;
        InitializeComponents();
    }

    protected override void InitializeFromParams(object[] parameters)
    {
        if (
            parameters.Length >= 2
            && parameters[0] is Transform target
            && parameters[1] is MovementConfig movementConfig
        )
        {
            this.target = target;
            this.movementConfig = movementConfig;
            InitializeComponents();
        }
        else
        {
            Debug.LogError("HomingProjectileBehavior initialized with incorrect parameters");
        }
    }

    private void InitializeComponents()
    {
        projectileRB = projectile.gameObject.GetComponent<Rigidbody2D>();
        currentSpeed = projectileRB.velocity.magnitude; // Maintain initial speed

        if (movementConfig != null)
        {
            movementHandler = new MovementHandler(movementConfig);

            // initialize movement handler with current velocity
            movementHandler.ApplyVelocity(projectileRB.velocity);
        }
    }

    public override void Cleanup()
    {
        target = null;
        base.Cleanup();
    }
}
