using UnityEngine;

public abstract class BaseMovementStrategy : ScriptableObject, IMovementStrategy
{
    protected MovementConfig config;
    protected float followDistance = 5f;
    protected MovementHandler movementHandler;
    protected bool isInitialized;

    public virtual void Initialize(MovementConfig config)
    {
        this.config = config;
        movementHandler = new MovementHandler(config);
    }

    public virtual void OnEnter(Transform self, Transform target)
    {
        isInitialized = true;
    }

    public virtual void OnUpdate(Transform self, Transform target)
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

        Debug.Log($"DIRECTION: {directionToTarget} DISTANCE: {distanceToTarget}");

        movementHandler.ApplyMovement(self, targetDirection, Time.deltaTime);
    }

    public virtual void OnExit()
    {
        isInitialized = false;
    }

    public virtual void OnStrategyComplete() { }

    public MovementConfig GetMovementConfig() => config;
}
