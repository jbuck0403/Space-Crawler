using UnityEngine;

public abstract class BaseMovementStrategy : ScriptableObject, IMovementStrategy
{
    protected MovementConfig config;
    protected float followDistance = 5f;
    protected MovementHandler movementHandler;
    protected bool isInitialized;

    protected BaseEnemyController enemyController;

    public virtual void Initialize(MovementConfig config, BaseEnemyController enemyController)
    {
        this.config = config;
        this.enemyController = enemyController;
        movementHandler = new MovementHandler(config);
    }

    public virtual bool IsComplete()
    {
        return enemyController.isComplete;
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

        bool shouldFollow = distanceToTarget >= followDistance;

        Vector2 targetDirection = shouldFollow ? directionToTarget : Vector2.zero;

        // Debug.Log($"DIRECTION: {directionToTarget} DISTANCE: {distanceToTarget}");

        if (targetDirection == Vector2.zero)
        {
            movementHandler.ApplyRotation(self, directionToTarget, Time.deltaTime);
        }
        movementHandler.ApplyMovement(self, targetDirection, Time.deltaTime);
    }

    public virtual void OnExit()
    {
        isInitialized = false;
    }

    public virtual void OnStrategyComplete()
    {
        enemyController.isComplete = false;
    }

    public MovementConfig GetMovementConfig() => config;
}
