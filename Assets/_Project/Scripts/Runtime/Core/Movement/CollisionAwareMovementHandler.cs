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
            return base.CalculateMovement(targetDirection, currentPosition, deltaTime);
        }

        Vector2 targetPosition = base.CalculateMovement(
            targetDirection,
            currentPosition,
            deltaTime
        );

        if (targetPosition == currentPosition)
        {
            return currentPosition;
        }

        Vector2 movement = targetPosition - currentPosition;
        float movementDistance = movement.magnitude;
        Vector2 movementDirection = movement.normalized;

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

            ResetVelocity();

            return currentPosition + (movementDirection * safeDistance);
        }

        return targetPosition;
    }

    /// <summary>
    /// Overrides the CalculateRotation method to prevent rotation into obstacles
    /// </summary>
    public override float CalculateRotation(
        Vector2 targetDirection,
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

        Vector2 currentPosition = transform.position;

        if (currentPosition == Vector2.zero)
        {
            return base.CalculateRotation(
                targetDirection,
                currentRotation,
                rotationSpeed,
                deltaTime
            );
        }

        if (Physics2D.Raycast(currentPosition, targetDirection, lookAheadDistance, collisionLayers))
        {
            return currentRotation;
        }

        return base.CalculateRotation(targetDirection, currentRotation, rotationSpeed, deltaTime);
    }

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
    /// Checks if a movement would cause a collision and returns true if it would
    /// </summary>
    public bool WouldCollide(
        Vector2 fromPosition,
        Vector2 toPosition,
        out Vector2 hitPoint,
        out Vector2 hitNormal,
        float extraDistance = 0f
    )
    {
        hitPoint = toPosition;
        hitNormal = Vector2.zero;

        if (!collisionDetectionEnabled || collisionLayers == 0)
            return false;

        Vector2 direction = toPosition - fromPosition;
        float distance = direction.magnitude + extraDistance;

        if (distance < 0.001f)
            return false;

        direction = direction.normalized;

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

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, distance, collisionLayers);

            if (hit.collider != null && !hit.collider.isTrigger)
            {
                hitPoint = hit.point;
                hitNormal = hit.normal;
                return true;
            }
        }

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
        Vector2 currentPosition = transform.position;
        float currentRotation = transform.eulerAngles.z;

        Vector2 newPosition = CalculateMovement(targetDirection, currentPosition, deltaTime);

        float newRotation = CalculateRotationWithPosition(
            targetDirection,
            currentPosition,
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
}
