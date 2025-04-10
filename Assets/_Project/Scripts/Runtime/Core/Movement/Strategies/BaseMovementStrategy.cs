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
            instance.movementHandler = new CollisionAwareMovementHandler(config);

            // If we're using a CollisionAwareMovementHandler, initialize it with collision detection
            if (instance.movementHandler is CollisionAwareMovementHandler collisionHandler)
            {
                collisionHandler.InitializeCollisionDetection(
                    enemyController.combatantLayers + enemyController.obstacleLayers,
                    enemyController.weaponHandler.FirePoint
                );
            }
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
                movementHandler = new CollisionAwareMovementHandler(config);
                // If we're using a CollisionAwareMovementHandler, initialize it with collision detection
                if (movementHandler is CollisionAwareMovementHandler collisionHandler)
                {
                    collisionHandler.InitializeCollisionDetection(
                        avoidanceLayers,
                        enemyController.weaponHandler.FirePoint
                    );
                }
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

        Debug.Log(
            $"###MoveCharacter: Entity={self.name}, Target={target.name}, ShouldFollow={shouldFollow}, Distance={distanceToTarget}, FollowDistance={followDistance}"
        );
        Debug.Log(
            $"###MoveCharacter: TargetDirection={targetDirection}, AvoidanceVector={avoidanceVector}, AdjustedDirection={adjustedMoveDirection}"
        );

        // Check if WouldCollide is available and working
        if (movementHandler is CollisionAwareMovementHandler collisionHandler)
        {
            Vector2 currentPosition = self.position;
            Vector2 nextPosition =
                currentPosition
                + (adjustedMoveDirection * Time.deltaTime * config.maxSpeed * config.maxSpeed);

            Debug.Log(
                $"###MoveCharacter: Checking collision from {currentPosition} to {nextPosition}"
            );

            Vector2 hitPoint;
            Vector2 hitNormal;
            bool wouldCollide = collisionHandler.WouldCollide(
                self,
                target.gameObject,
                out hitPoint,
                out hitNormal
            );

            Debug.Log(
                $"###[{GetType().Name}] WouldCollide check: {wouldCollide}, Hit Point: {hitPoint}, Hit Normal: {hitNormal}"
            );

            // Log movement handler info
            Debug.Log(
                $"###MoveCharacter: MovementHandler type: {movementHandler.GetType().Name}, CollisionDetectionEnabled: {collisionHandler.GetCollisionDetectionStatus()}"
            );

            if (wouldCollide)
            {
                adjustedMoveDirection = CalculateObstacleAvoidanceDirection(
                    self.position,
                    adjustedMoveDirection,
                    collisionHandler,
                    target.position
                );
            }
        }

        movementHandler.ApplyRotation(self, directionToTarget, Time.deltaTime);

        movementHandler.ApplyMovement(self, adjustedMoveDirection, Time.deltaTime);

        // Log final position after movement
        Debug.Log($"###MoveCharacter: Final position after movement: {self.position}");
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

    /// <summary>
    /// Calculates an adjusted movement direction that avoids obstacles
    /// </summary>
    /// <param name="position">Current position</param>
    /// <param name="currentDirection">Current intended movement direction</param>
    /// <param name="collisionHandler">The collision handler to use for raycast checks</param>
    /// <param name="targetPosition">Optional target position to consider when avoiding</param>
    /// <returns>Adjusted direction that avoids obstacles</returns>
    protected Vector2 CalculateObstacleAvoidanceDirection(
        Vector2 position,
        Vector2 currentDirection,
        CollisionAwareMovementHandler collisionHandler,
        Vector2? targetPosition = null
    )
    {
        // Normalize the current direction
        Vector2 direction = currentDirection.normalized;
        if (direction == Vector2.zero)
            return currentDirection;

        // Ray distance to check
        float rayDistance = 5f;

        // Define our three directions: forward, left-forward, right-forward
        Vector2 forward = direction;

        // Create vectors perpendicular to forward
        Vector2 right = new Vector2(forward.y, -forward.x); // 90 degrees clockwise
        Vector2 left = new Vector2(-forward.y, forward.x); // 90 degrees counter-clockwise

        // Calculate the angled directions (45 degrees from forward)
        Vector2 forwardRight = (forward + right).normalized;
        Vector2 forwardLeft = (forward + left).normalized;

        // Check distances in each direction
        float forwardDistance = collisionHandler.DistanceToObstacle(position, forward, rayDistance);
        float rightDistance = collisionHandler.DistanceToObstacle(
            position,
            forwardRight,
            rayDistance
        );
        float leftDistance = collisionHandler.DistanceToObstacle(
            position,
            forwardLeft,
            rayDistance
        );

        Debug.Log(
            $"Obstacle distances - Forward: {forwardDistance}, Right: {rightDistance}, Left: {leftDistance}"
        );

        // Default to the original direction
        Vector2 adjustedDirection = direction;

        // If forward direction has an obstacle
        if (forwardDistance < rayDistance)
        {
            // Check if we should turn left or right based on which has more space
            if (leftDistance > rightDistance)
            {
                // More space to the left, rotate left by 5 degrees
                float angle = 5f;
                adjustedDirection = RotateVector(direction, angle);
                Debug.Log($"Obstacle ahead, turning left {angle} degrees");
            }
            else if (rightDistance > leftDistance)
            {
                // More space to the right, rotate right by 5 degrees
                float angle = -5f;
                adjustedDirection = RotateVector(direction, angle);
                Debug.Log($"Obstacle ahead, turning right {angle} degrees");
            }
            // If both left and right are equally blocked, then just continue in the current direction
            // The collision system will handle stopping before hitting the wall
        }

        // If we have a target position and can see it, slightly bias movement toward target
        if (targetPosition.HasValue && adjustedDirection != direction)
        {
            Vector2 directionToTarget = ((Vector2)targetPosition.Value - position).normalized;
            // Add a small bias (10%) toward the target
            adjustedDirection = Vector2.Lerp(adjustedDirection, directionToTarget, 0.1f).normalized;
        }

        return adjustedDirection;
    }

    /// <summary>
    /// Rotates a 2D vector by the specified angle in degrees
    /// </summary>
    private Vector2 RotateVector(Vector2 vector, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float x = vector.x * cos - vector.y * sin;
        float y = vector.x * sin + vector.y * cos;

        return new Vector2(x, y).normalized;
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
