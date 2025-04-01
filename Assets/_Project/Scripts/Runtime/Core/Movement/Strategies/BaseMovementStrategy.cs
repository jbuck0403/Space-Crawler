using UnityEngine;

public abstract class BaseMovementStrategy : ScriptableObject, IMovementStrategy
{
    [Header("Follow Settings")]
    [SerializeField]
    protected float followDistance = 5f;

    [Header("Avoidance Settings")]
    [SerializeField]
    protected float avoidanceRadius = 2.0f;

    [SerializeField]
    protected float avoidanceStrength = 2.0f;

    [SerializeField]
    [Tooltip("How much avoidance affects rotation (0 = not at all, 1 = fully)")]
    protected float rotationAvoidanceStrength = 0.2f;

    [SerializeField]
    protected LayerMask avoidanceLayers;
    protected MovementConfig config;
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
        bool shouldReturn = NullCheck(target);
        if (shouldReturn)
            return;

        MoveCharacter(self, target);
    }

    private bool NullCheck(Transform target)
    {
        if (!isInitialized || target == null)
        {
            Debug.Log(
                $"[{GetType().Name}] OnUpdate early return - isInitialized: {isInitialized}, target: {(target == null ? "null" : "not null")}"
            );
            return true;
        }

        if (movementHandler == null)
        {
            Debug.LogError(
                $"[{GetType().Name}] movementHandler is NULL! isInstance: {isInstance}, isInitialized: {isInitialized}, config: {(config == null ? "null" : "not null")}"
            );

            if (config != null)
            {
                Debug.Log(
                    $"[{GetType().Name}] Attempting to recover by creating new MovementHandler"
                );
                movementHandler = new MovementHandler(config);
            }
            else
            {
                return true;
            }
        }

        return false;
    }

    protected virtual void MoveCharacter(Transform self, Transform target)
    {
        if (target == null)
            return;

        Vector2 directionToTarget = MovementUtils.GetTargetDirection(
            self.position,
            target.position
        );
        float distanceToTarget = MovementUtils.GetDistanceToTarget(self.position, target.position);

        bool shouldFollow = distanceToTarget >= followDistance;

        Vector2 targetDirection = shouldFollow ? directionToTarget : Vector2.zero;

        Vector2 avoidanceVector = AvoidOtherCharacters(self);

        Vector2 adjustedMoveDirection = (targetDirection + avoidanceVector).normalized;

        movementHandler.ApplyRotation(self, directionToTarget, Time.deltaTime);

        movementHandler.ApplyMovement(self, adjustedMoveDirection, Time.deltaTime);
    }

    protected virtual Vector2 AvoidOtherCharacters(Transform self)
    {
        Vector2 avoidanceVector = Vector2.zero;

        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(
            self.position,
            avoidanceRadius,
            avoidanceLayers
        );

        if (nearbyColliders.Length == 0)
            return avoidanceVector;

        foreach (Collider2D other in nearbyColliders)
        {
            if (other.transform == self)
                continue;

            Vector2 directionToOther = MovementUtils.GetTargetDirection(
                self.position,
                other.transform.position
            );
            float distance = MovementUtils.GetDistanceToTarget(
                self.position,
                other.transform.position
            );

            if (distance > avoidanceRadius || distance < 0.001f)
                continue;

            float weight = 1.0f - (distance / avoidanceRadius);
            avoidanceVector -= avoidanceStrength * weight * directionToOther;
        }

        return avoidanceVector;
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
