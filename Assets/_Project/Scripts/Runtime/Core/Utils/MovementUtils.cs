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
}
