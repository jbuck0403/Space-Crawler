using UnityEngine;

public abstract class BaseMovementStrategy : ScriptableObject, IMovementStrategy
{
    protected MovementConfig config;
    protected float followDistance = 5f;
    protected MovementHandler movementHandler;
    protected bool isInitialized;
    private bool isInstance;

    protected bool isComplete = false;

    protected BaseEnemyController enemyController;

    public virtual BaseMovementStrategy Initialize(
        MovementConfig config,
        BaseEnemyController enemyController
    )
    {
        Debug.Log($"[{GetType().Name}] Initializing new instance");
        BaseMovementStrategy instance = Instantiate(this);
        instance.isInstance = true;
        instance.config = config;
        instance.enemyController = enemyController;

        // create movement handler immediately after setting config
        if (config != null)
        {
            instance.movementHandler = new MovementHandler(config);
        }
        else
        {
            Debug.LogError(
                $"[{GetType().Name}] Cannot initialize MovementHandler - config is null!"
            );
        }

        Debug.Log(
            $"[{GetType().Name}] New instance created with isInstance: {instance.isInstance}"
        );
        return instance;
    }

    public virtual bool IsComplete()
    {
        return isComplete;
    }

    public virtual bool IsInstance()
    {
        Debug.Log($"[{GetType().Name}] IsInstance check: {isInstance}");
        return isInstance;
    }

    public virtual void OnEnter(Transform self, Transform target)
    {
        Debug.Log(
            $"[{GetType().Name}] OnEnter - isInstance: {isInstance}, movementHandler: {(movementHandler == null ? "null" : "not null")}"
        );
        isInitialized = true;
        isComplete = false;
    }

    public virtual void OnUpdate(Transform self, Transform target)
    {
        if (!isInitialized || target == null)
        {
            Debug.Log(
                $"[{GetType().Name}] OnUpdate early return - isInitialized: {isInitialized}, target: {(target == null ? "null" : "not null")}"
            );
            return;
        }

        // Debug check for null movementHandler
        if (movementHandler == null)
        {
            Debug.LogError(
                $"[{GetType().Name}] movementHandler is NULL! isInstance: {isInstance}, isInitialized: {isInitialized}, config: {(config == null ? "null" : "not null")}"
            );

            // Try to recover by creating a new handler if config is available
            if (config != null)
            {
                Debug.Log(
                    $"[{GetType().Name}] Attempting to recover by creating new MovementHandler"
                );
                movementHandler = new MovementHandler(config);
            }
            else
            {
                return; // Can't proceed without config
            }
        }

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
        movementHandler.Move(self, targetDirection, Time.deltaTime);
    }

    public virtual void OnExit()
    {
        Debug.Log($"[{GetType().Name}] OnExit");
        isInitialized = false;
    }

    public virtual void OnStrategyComplete()
    {
        isComplete = false;
    }

    public MovementConfig GetMovementConfig() => config;

    public IStrategy CreateInstance()
    {
        return Initialize(config, enemyController);
    }

    public MovementHandler GetMovementHandler()
    {
        return movementHandler;
    }
}
