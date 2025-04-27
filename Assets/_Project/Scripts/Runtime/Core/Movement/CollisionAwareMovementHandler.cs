using UnityEngine;

/// <summary>
/// Extension of MovementHandler that adds collision detection capabilities
/// while maintaining full compatibility with the existing API.
/// </summary>
public class CollisionAwareMovementHandler : MovementHandler
{
    // Collision detection properties
    private LayerMask collisionLayers;
    private float skinWidth = 0.1f;
    private int raycastCount = 8;
    private float collisionRadius = 0.5f;
    private bool collisionDetectionEnabled = true;
    private float collisionBuffer = 0.1f;
    private Transform transform;

    private Collider2D attachedCollider;

    public CollisionAwareMovementHandler(MovementConfig config)
        : base(config) { }

    /// <summary>
    /// Initializes collision detection with the specified parameters
    /// </summary>
    public void InitializeCollisionDetection(
        LayerMask collisionLayers,
        Transform transform,
        float collisionRadius = 0.5f,
        Collider2D attachedCollider = null
    )
    {
        this.collisionLayers = collisionLayers;
        this.transform = transform;
        this.collisionRadius = collisionRadius;
        this.attachedCollider = attachedCollider;
        collisionDetectionEnabled = true;

        // Log initialization for debugging
        Debug.Log(
            $"CollisionAwareMovementHandler initialized with transform: {(transform != null ? transform.name : "null")}"
        );
    }

    /// <summary>
    /// Enables or disables collision detection
    /// </summary>
    public void SetCollisionDetection(bool enabled)
    {
        collisionDetectionEnabled = enabled;
    }

    /// <summary>
    /// Overrides the CalculateMovement method to prevent moving into obstacles
    /// </summary>
    public override Vector2 CalculateMovement(
        Vector2 targetDirection,
        Vector2 currentPosition,
        float deltaTime
    )
    {
        if (!collisionDetectionEnabled || collisionLayers == 0)
        {
            Debug.Log("###CalculateMovement: Collision detection disabled or no layers");
            return base.CalculateMovement(targetDirection, currentPosition, deltaTime);
        }

        Vector2 targetPosition = base.CalculateMovement(
            targetDirection,
            currentPosition,
            deltaTime
        );

        Debug.Log(
            $"###CalculateMovement: From {currentPosition} to target {targetPosition}, direction {targetDirection}"
        );

        if (targetPosition == currentPosition)
        {
            Debug.Log("###CalculateMovement: Target position is the same as current position");
            return currentPosition;
        }

        Vector2 movement = targetPosition - currentPosition;
        float movementDistance = movement.magnitude;
        Vector2 movementDirection = movement.normalized;

        Debug.Log(
            $"###CalculateMovement: Movement vector {movement}, distance {movementDistance}, direction {movementDirection}"
        );

        bool collisionDetected = false;
        float closestHitDistance = float.MaxValue;

        for (int i = 0; i < raycastCount; i++)
        {
            float angle = (i * 2 * Mathf.PI) / raycastCount;

            Vector2 offset = new Vector2(
                Mathf.Cos(angle) * collisionRadius,
                Mathf.Sin(angle) * collisionRadius
            );

            Vector2 rayOrigin = currentPosition + offset;

            RaycastHit2D hit = Physics2D.Raycast(
                rayOrigin,
                movementDirection,
                movementDistance + collisionBuffer,
                collisionLayers
            );

            hit = SkipTriggerColliders(hit, movementDirection, movementDistance, collisionBuffer);

            if (hit.collider != null)
            {
                collisionDetected = true;

                Debug.Log(
                    $"###CalculateMovement: Ray {i} hit {hit.collider.name} at distance {hit.distance}"
                );

                float adjustedDistance =
                    hit.distance
                    - (offset.magnitude * Vector2.Dot(movementDirection, offset.normalized));
                closestHitDistance = Mathf.Min(closestHitDistance, adjustedDistance);
            }
        }

        if (collisionDetected)
        {
            float safeDistance = Mathf.Max(
                0,
                closestHitDistance - collisionBuffer - collisionRadius
            );

            Debug.Log(
                $"###CalculateMovement: Collision detected, closest hit {closestHitDistance}, safe distance {safeDistance}"
            );

            ResetVelocity();

            Vector2 newPosition = currentPosition + (movementDirection * safeDistance);
            Debug.Log($"###CalculateMovement: RETURNING collision-adjusted position {newPosition}");
            return newPosition;
        }

        Debug.Log($"###CalculateMovement: RETURNING original target position {targetPosition}");
        return targetPosition;
    }

    /// <summary>
    /// Overrides the CalculateRotation method to prevent rotation into obstacles
    /// </summary>
    // public override float CalculateRotation(
    //     Vector2 targetDirection,
    //     float currentRotation,
    //     float rotationSpeed,
    //     float deltaTime
    // )
    // {
    //     if (!collisionDetectionEnabled || collisionLayers == 0 || targetDirection == Vector2.zero)
    //     {
    //         return base.CalculateRotation(
    //             targetDirection,
    //             currentRotation,
    //             rotationSpeed,
    //             deltaTime
    //         );
    //     }

    //     // If transform is not set, use base rotation
    //     if (transform == null)
    //     {
    //         return base.CalculateRotation(
    //             targetDirection,
    //             currentRotation,
    //             rotationSpeed,
    //             deltaTime
    //         );
    //     }

    //     float lookAheadDistance = 1.0f + GetCurrentVelocity().magnitude * 0.5f;

    //     Vector2 currentPosition = transform.position;

    //     if (currentPosition == Vector2.zero)
    //     {
    //         return base.CalculateRotation(
    //             targetDirection,
    //             currentRotation,
    //             rotationSpeed,
    //             deltaTime
    //         );
    //     }

    //     // Check for collisions in the direction of rotation
    //     bool collisionDetected = false;
    //     for (int i = 0; i < raycastCount; i++)
    //     {
    //         float angle = (i * 2 * Mathf.PI) / raycastCount;
    //         Vector2 offset = new Vector2(
    //             Mathf.Cos(angle) * collisionRadius,
    //             Mathf.Sin(angle) * collisionRadius
    //         );
    //         Vector2 rayOrigin = currentPosition + offset;

    //         RaycastHit2D hit = Physics2D.Raycast(
    //             rayOrigin,
    //             targetDirection,
    //             lookAheadDistance,
    //             collisionLayers
    //         );

    //         hit = SkipTriggerColliders(hit, targetDirection, lookAheadDistance, 0);

    //         if (hit.collider != null)
    //         {
    //             float adjustedDistance =
    //                 hit.distance
    //                 - (offset.magnitude * Vector2.Dot(targetDirection, offset.normalized));

    //             if (adjustedDistance < collisionRadius + collisionBuffer)
    //             {
    //                 collisionDetected = true;
    //                 break;
    //             }
    //         }
    //     }

    //     if (collisionDetected)
    //     {
    //         return currentRotation;
    //     }

    //     return base.CalculateRotation(targetDirection, currentRotation, rotationSpeed, deltaTime);
    // }

    /// <summary>
    /// Performs collision-aware rotation calculation with an explicit current position
    /// </summary>
    public float CalculateRotationWithPosition(
        Vector2 targetDirection,
        Vector2 currentPosition,
        float currentRotation,
        float rotationSpeed,
        float deltaTime
    )
    {
        if (!collisionDetectionEnabled || collisionLayers == 0 || targetDirection == Vector2.zero)
        {
            return base.CalculateRotation(
                targetDirection,
                currentRotation,
                rotationSpeed,
                deltaTime
            );
        }

        float lookAheadDistance = 1.0f + GetCurrentVelocity().magnitude * 0.5f;

        bool collisionDetected = false;

        for (int i = 0; i < raycastCount; i++)
        {
            float angle = (i * 2 * Mathf.PI) / raycastCount;

            Vector2 offset = new Vector2(
                Mathf.Cos(angle) * collisionRadius,
                Mathf.Sin(angle) * collisionRadius
            );

            Vector2 rayOrigin = currentPosition + offset;

            RaycastHit2D hit = Physics2D.Raycast(
                rayOrigin,
                targetDirection,
                lookAheadDistance,
                collisionLayers
            );

            hit = SkipTriggerColliders(hit, targetDirection, lookAheadDistance, 0);

            if (hit.collider != null)
            {
                float adjustedDistance =
                    hit.distance
                    - (offset.magnitude * Vector2.Dot(targetDirection, offset.normalized));

                if (adjustedDistance < collisionRadius + collisionBuffer)
                {
                    collisionDetected = true;
                    break; // We found a collision, no need to check more rays
                }
            }
        }

        if (collisionDetected)
        {
            return currentRotation;
        }

        // No obstacle, rotate normally
        return base.CalculateRotation(targetDirection, currentRotation, rotationSpeed, deltaTime);
    }

    /// <summary>
    /// Checks if a movement would cause a collision by using WouldMovementCollide
    /// </summary>
    public bool WouldCollide(
        Transform self,
        GameObject target,
        out Vector2 hitPoint,
        out Vector2 hitNormal,
        float extraDistance = 0f
    )
    {
        hitPoint = Vector2.zero;
        hitNormal = Vector2.zero;

        // Log input parameters
        Debug.Log(
            $"###WouldCollide CALLED: Self={self.name}, Target={target.name}, Layers={collisionLayers.value}"
        );

        if (!collisionDetectionEnabled || collisionLayers == 0)
        {
            Debug.Log($"###WouldCollide: Collision detection disabled or no layers");
            return false;
        }

        // Get current position and direction to target
        Vector2 fromPosition = self.position;
        Vector2 targetPosition = target.transform.position;
        Vector2 direction = (targetPosition - fromPosition).normalized;

        // Create a test position a small distance away in the direction of the target
        float testDistance = 0.5f; // Use a small test distance

        Debug.Log(
            $"###WouldCollide: Testing movement from {fromPosition} to {targetPosition}, direction={direction}"
        );

        // Use the utility method to check for collisions
        bool collisionDetected = MovementUtils.WouldMovementCollide(
            this,
            fromPosition,
            direction,
            testDistance,
            out hitPoint,
            out hitNormal
        );

        Debug.Log($"###WouldCollide: Collision detected={collisionDetected}, hit point={hitPoint}");

        if (collisionDetected)
        {
            Debug.Log($"###WouldCollide: RETURNING TRUE, collision detected at {hitPoint}");
            return true;
        }

        Debug.Log($"###WouldCollide: RETURNING FALSE, no collision detected");
        return false;
    }

    /// <summary>
    /// Get distance to nearest obstacle in a direction
    /// </summary>
    public float DistanceToObstacle(
        Vector2 fromPosition,
        Vector2 direction,
        float maxDistance = 10f
    )
    {
        if (!collisionDetectionEnabled || collisionLayers == 0)
            return maxDistance;

        direction = direction.normalized;
        float closestDistance = maxDistance;

        Bounds? bounds = null;
        if (attachedCollider != null)
            bounds = attachedCollider.bounds;

        float raycastRadius = bounds.HasValue
            ? Mathf.Max(bounds.Value.extents.x, bounds.Value.extents.y)
            : collisionRadius;

        for (int i = 0; i < raycastCount; i++)
        {
            float angle = ((float)i / raycastCount) * 2 * Mathf.PI;
            Vector2 offset = new Vector2(
                Mathf.Cos(angle) * raycastRadius * 0.8f,
                Mathf.Sin(angle) * raycastRadius * 0.8f
            );

            Vector2 rayOrigin = fromPosition + offset;

            RaycastHit2D hit = Physics2D.Raycast(
                rayOrigin,
                direction,
                maxDistance,
                collisionLayers
            );

            if (hit.collider != null && !hit.collider.isTrigger)
            {
                float hitDistance = hit.distance;
                if (hitDistance < closestDistance)
                {
                    closestDistance = hitDistance;
                }
            }
        }

        return closestDistance;
    }

    /// <summary>
    /// Overrides the Move method to use collision-aware movement and rotation
    /// </summary>
    public new void Move(Transform transform, Vector2 targetDirection, float deltaTime)
    {
        // Ensure we have a valid transform reference
        if (this.transform == null)
        {
            this.transform = transform;
        }

        Vector2 currentPosition = transform.position;
        float currentRotation = transform.eulerAngles.z;

        Vector2 newPosition = CalculateMovement(targetDirection, currentPosition, deltaTime);

        // Use the base CalculateRotation method for rotation
        float newRotation = base.CalculateRotation(
            targetDirection,
            currentRotation,
            GetModifiedConfig().rotationSpeed,
            deltaTime
        );

        transform.SetPositionAndRotation(newPosition, Quaternion.Euler(0, 0, newRotation));
    }

    private RaycastHit2D SkipTriggerColliders(
        RaycastHit2D hit,
        Vector2 direction,
        float distance,
        float buffer
    )
    {
        while (hit.collider != null && hit.collider.isTrigger)
        {
            hit = Physics2D.Raycast(
                hit.point + (direction * 0.01f),
                direction,
                distance - hit.distance + buffer,
                collisionLayers
            );
        }
        return hit;
    }

    /// <summary>
    /// Returns whether collision detection is enabled
    /// </summary>
    public bool GetCollisionDetectionStatus()
    {
        return collisionDetectionEnabled && collisionLayers != 0;
    }
}
