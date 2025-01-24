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
}
