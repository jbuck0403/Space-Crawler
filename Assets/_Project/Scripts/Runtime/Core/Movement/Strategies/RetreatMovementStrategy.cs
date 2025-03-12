using UnityEngine;

[CreateAssetMenu(
    fileName = "RetreatMovementStrategy",
    menuName = "SpaceShooter/Strategies/Movement Strategies/Retreat"
)]
public class RetreatMovementStrategy : BaseMovementStrategy
{
    [SerializeField]
    private float retreatDistance = 15f;

    public override void OnUpdate(Transform self, Transform target)
    {
        if (!isInitialized || target == null)
            return;

        // get direction from target to self (opposite of direction to target)
        Vector2 directionFromTarget = MovementUtils.GetTargetDirection(
            target.position,
            self.position
        );

        float distanceFromTarget = MovementUtils.GetDistanceToTarget(
            self.position,
            target.position
        );

        // only retreat if we're closer than retreat distance
        Vector2 targetDirection =
            distanceFromTarget < retreatDistance ? directionFromTarget : Vector2.zero;

        movementHandler.ApplyMovement(self, targetDirection, Time.deltaTime);
    }

    public override void OnStrategyComplete()
    {
        // change strategy to default
        enemyController.SetDefaultStrategy();

        base.OnStrategyComplete();
    }
}
