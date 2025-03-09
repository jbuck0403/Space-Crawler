using UnityEngine;

public class DefaultMovementStrategy : IMovementStrategy
{
    private readonly MovementConfig config;
    private readonly float followDistance = 5f;
    private readonly MovementHandler movementHandler;
    private bool isInitialized;

    public DefaultMovementStrategy(MovementConfig config)
    {
        this.config = config;
        movementHandler = new MovementHandler(config);
    }

    public void OnEnter(Transform self, Transform target)
    {
        isInitialized = true;
    }

    public void OnUpdate(Transform self, Transform target)
    {
        if (!isInitialized || target == null)
            return;

        Vector2 directionToTarget = MovementUtils.GetTargetDirection(
            self.position,
            target.position
        );
        float distanceToTarget = MovementUtils.GetDistanceToTarget(self.position, target.position);

        // only move if we're further than follow distance
        Vector2 targetDirection =
            distanceToTarget > followDistance ? directionToTarget : Vector2.zero;

        Debug.Log(targetDirection);

        movementHandler.ApplyMovement(self, targetDirection, Time.deltaTime);
    }

    public void OnExit()
    {
        isInitialized = false;
    }

    public void OnStrategyComplete() { }

    public MovementConfig GetMovementConfig() => config;
}
