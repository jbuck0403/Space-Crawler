using UnityEngine;

[CreateAssetMenu(
    fileName = "CircleMovementStrategy",
    menuName = "SpaceShooter/Strategies/Movement Strategies/Circle"
)]
public class CircleMovementStrategy : BaseMovementStrategy
{
    [SerializeField]
    [Range(0.5f, 3.5f)]
    private float circlingSpeed = 1f;

    [SerializeField]
    [Range(0f, 1f)]
    private float orbitTightness = 0.7f;

    private bool isClockwise;
    private Vector2 currentCirclingDirection;

    public override void OnEnter(Transform self, Transform target)
    {
        base.OnEnter(self, target);
        isClockwise = RandomUtils.RandomBool();
        UpdateCirclingDirection(self, target);
    }

    private Vector2 CalculateOrbitDirection(Vector2 directionToTarget, float tightness)
    {
        Vector2 perpendicular = isClockwise
            ? new Vector2(-directionToTarget.y, directionToTarget.x)
            : new Vector2(directionToTarget.y, -directionToTarget.x);

        return Vector2.Lerp(directionToTarget, perpendicular, tightness).normalized;
    }

    private void UpdateCirclingDirection(Transform self, Transform target)
    {
        Vector2 directionToTarget = MovementUtils.GetTargetDirection(
            self.position,
            target.position
        );
        currentCirclingDirection = CalculateOrbitDirection(directionToTarget, orbitTightness);
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

        UpdateCirclingDirection(self, target);

        // Blend radial and circular motion based on distance
        float distanceRatio = (distanceToTarget - followDistance) / followDistance;
        Vector2 targetDirection = Vector2.Lerp(
            currentCirclingDirection,
            directionToTarget * Mathf.Sign(distanceRatio),
            Mathf.Abs(distanceRatio)
        );

        movementHandler.ApplyRotation(self, directionToTarget, Time.deltaTime);
        movementHandler.ApplyMovement(self, targetDirection * circlingSpeed, Time.deltaTime);
    }
}
