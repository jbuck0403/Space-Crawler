using UnityEngine;

[CreateAssetMenu(
    fileName = "CautiousMovementStrategy",
    menuName = "SpaceShooter/Strategies/Movement Strategies/Cautious"
)]
public class CautiousMovementStrategy : BaseMovementStrategy
{
    [SerializeField]
    [Range(1.5f, 3f)]
    private float caution;

    [SerializeField]
    private float distanceTolerance = 0.5f;
    private float minimumDistance;

    public override void OnEnter(Transform self, Transform target)
    {
        base.OnEnter(self, target);

        minimumDistance = followDistance * caution;
    }

    public override void OnUpdate(Transform self, Transform target)
    {
        if (!isInitialized || target == null)
            return;

        Vector2 directionToTarget = MovementUtils.GetTargetDirection(
            self.position,
            target.position
        );
        float distanceToTarget = MovementUtils.GetDistanceToTarget(self.position, target.position);

        float idealMinDistance = minimumDistance - distanceTolerance;
        float idealMaxDistance = minimumDistance + distanceTolerance;

        Vector2 targetDirection;

        if (distanceToTarget < idealMinDistance)
        {
            targetDirection = -directionToTarget;
        }
        else if (distanceToTarget > idealMaxDistance)
        {
            targetDirection = directionToTarget;
        }
        else
        {
            targetDirection = Vector2.zero;
        }

        // Apply obstacle avoidance if needed and we're moving
        if (
            targetDirection != Vector2.zero
            && movementHandler is CollisionAwareMovementHandler collisionHandler
        )
        {
            Vector2 hitPoint;
            Vector2 hitNormal;
            bool wouldCollide = collisionHandler.WouldCollide(
                self,
                target.gameObject,
                out hitPoint,
                out hitNormal
            );

            if (wouldCollide)
            {
                Vector2? targetPos = null;
                // If moving toward target, provide target position for bias
                if (targetDirection == directionToTarget)
                {
                    targetPos = target.position;
                }

                targetDirection = CalculateObstacleAvoidanceDirection(
                    self.position,
                    targetDirection,
                    collisionHandler,
                    targetPos
                );

                Debug.Log(
                    $"[CautiousMovementStrategy] Adjusted direction to avoid obstacle: {targetDirection}"
                );
            }
        }

        movementHandler.ApplyRotation(self, directionToTarget, Time.deltaTime);
        movementHandler.ApplyMovement(self, targetDirection, Time.deltaTime);
    }
}
