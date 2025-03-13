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

        movementHandler.ApplyRotation(self, directionToTarget, Time.deltaTime);
        movementHandler.ApplyMovement(self, targetDirection, Time.deltaTime);
    }
}
