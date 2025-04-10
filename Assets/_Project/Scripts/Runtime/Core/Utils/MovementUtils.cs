using UnityEngine;

public static class MovementUtils
{
    public static bool HasMovedDistance(
        Vector2 startPosition,
        Vector2 currentPosition,
        float requiredDistance
    )
    {
        float distanceMoved = Vector2.Distance(startPosition, currentPosition);
        return distanceMoved >= requiredDistance;
    }

    public static float GetDistanceToTarget(Vector2 currentPosition, Vector2 targetPosition)
    {
        return Vector2.Distance(currentPosition, targetPosition);
    }

    public static bool WithinDistance(
        Vector2 currentPosition,
        Vector2 targetPosition,
        float distance
    )
    {
        if (GetDistanceToTarget(currentPosition, targetPosition) <= distance)
        {
            return true;
        }

        return false;
    }

    public static Vector2 GetTargetDirection(Vector2 self, Vector2 target)
    {
        Vector2 directionToTarget = (target - self).normalized;

        return directionToTarget;
    }

    public static bool IsFacingTarget(
        Transform self,
        Vector2 targetPosition,
        float angleThreshold = 10f
    )
    {
        Vector2 directionToTarget = GetTargetDirection(self.position, targetPosition);

        float targetAngle =
            Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg - 90f;

        float currentAngle = self.eulerAngles.z;

        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle));

        return angleDifference <= angleThreshold;
    }

    public static bool TargetViewObstructed(
        Transform self,
        GameObject targetObject,
        LayerMask obstacleLayer
    )
    {
        if (targetObject == null)
            return true;

        Vector2 targetPosition = targetObject.transform.position;

        if (IsFacingTarget(self, targetPosition, 30f))
        {
            Vector2 direction = GetTargetDirection(self.position, targetPosition);
            float distance = GetDistanceToTarget(self.position, targetPosition);

            Vector2 offsetOrigin = (Vector2)self.position + (direction * 0.5f);
            Debug.Log(
                $"###TargetViewObstructed: Using offset origin {offsetOrigin} (original: {self.position})"
            );

            float adjustedDistance = distance - 0.5f;
            if (adjustedDistance <= 0)
                return false; // Target is too close, consider not obstructed

            RaycastHit2D hit = Physics2D.Raycast(
                offsetOrigin,
                direction,
                adjustedDistance,
                obstacleLayer
            );
            Debug.Log($"###HIT{hit.collider?.gameObject.name ?? "none"}");

            // if the raycast didn't hit anything or if it hit the target
            if (hit.collider == null || hit.collider.gameObject == targetObject)
            {
                Debug.Log($"###RETURNING FALSE {hit.collider?.gameObject.name ?? "null hit"}");
                return false;
            }
        }

        Debug.Log($"###RETURNING TRUE");
        return true;
    }

    /// <summary>
    /// Determines if movement in a direction would result in a collision by comparing
    /// expected position with collision-adjusted position
    /// </summary>
    /// <returns>True if a collision would occur, false otherwise</returns>
    public static bool WouldMovementCollide(
        CollisionAwareMovementHandler handler,
        Vector2 currentPosition,
        Vector2 direction,
        float testDistance,
        out Vector2 hitPoint,
        out Vector2 hitNormal
    )
    {
        hitPoint = Vector2.zero;
        hitNormal = Vector2.zero;

        if (direction == Vector2.zero)
            return false;

        direction = direction.normalized;

        float movementSpeed = handler.GetModifiedConfig().maxSpeed;
        Vector2 expectedPosition =
            currentPosition + (direction * testDistance * movementSpeed * Time.deltaTime);

        Vector2 resultPosition = handler.CalculateMovement(
            direction,
            currentPosition,
            Time.deltaTime
        );

        float tolerance = 0.01f;
        bool collisionDetected = Vector2.Distance(resultPosition, expectedPosition) > tolerance;

        if (collisionDetected)
        {
            hitPoint = resultPosition;

            hitNormal = -direction;
            return true;
        }

        return false;
    }
}
