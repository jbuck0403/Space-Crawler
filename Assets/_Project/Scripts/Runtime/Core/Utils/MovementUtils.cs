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

        // calculate the angle the entity should be facing (same calculation as in MovementHandler)
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

            // Cast a ray in the target's direction
            RaycastHit2D hit = Physics2D.Raycast(self.position, direction, distance, obstacleLayer);
            Debug.Log($"HIT: {hit.collider.name}");

            // If nothing was hit, view is not obstructed
            if (hit.collider == null)
            {
                Debug.Log("RAYCAST HIT: " + hit.collider.name);
                return false;
            }

            // Check if the hit object is the target
            if (hit.collider.gameObject == targetObject)
            {
                return false;
            }
        }

        Debug.Log("XXX");
        return true;
    }
}
