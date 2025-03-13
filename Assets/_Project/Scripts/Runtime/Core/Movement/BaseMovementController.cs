using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BaseEnemyController))]
public class BaseMovementController : StrategyController<IMovementStrategy>
{
    private Transform DefaultTarget;
    public Transform currentTarget { get; private set; }
    private Transform previousTarget;

    protected BaseEnemyController enemyController;
    protected MovementConfig config;

    public void Awake()
    {
        DefaultTarget = GameObject.FindGameObjectWithTag("Player").transform;
        currentTarget = DefaultTarget;
        enemyController = GetComponent<BaseEnemyController>();
    }

    public void Initialize(BaseEnemyController enemyController, MovementConfig config)
    {
        this.enemyController = enemyController;
        this.config = config;
    }

    public void ChangeTarget(Transform target)
    {
        previousTarget = currentTarget;
        currentTarget = target;

        if (enemyController != null)
        {
            enemyController.UpdateTarget(currentTarget);
        }
    }

    public void TargetPreviousTarget()
    {
        ChangeTarget(previousTarget);
    }

    public void TargetDefaultTarget()
    {
        ChangeTarget(DefaultTarget);
    }

    protected override void OnStrategyExit(IMovementStrategy strategy)
    {
        strategy.OnExit();
    }

    protected override void OnStrategyEnter(IMovementStrategy strategy)
    {
        strategy.OnEnter(transform, currentTarget);
    }

    protected override void OnStrategyUpdate(IMovementStrategy strategy)
    {
        strategy.OnUpdate(transform, currentTarget);
    }

    public override IMovementStrategy SetStrategy(IMovementStrategy newStrategy)
    {
        // If the strategy is a BaseMovementStrategy but not an instance
        if (newStrategy is BaseMovementStrategy baseStrategy)
        {
            Debug.Log(
                $"[BaseMovementController] Setting strategy of type {baseStrategy.GetType().Name}"
            );
            Debug.Log(
                $"[BaseMovementController] Strategy isInstance before check: {baseStrategy.IsInstance()}"
            );

            if (!baseStrategy.IsInstance())
            {
                Debug.Log(
                    $"[BaseMovementController] Strategy is not an instance, creating new instance"
                );
                newStrategy = baseStrategy.Initialize(config, enemyController);
                Debug.Log(
                    $"[BaseMovementController] New instance created, isInstance: {((BaseMovementStrategy)newStrategy).IsInstance()}"
                );
            }
        }

        return base.SetStrategy(newStrategy);
    }

    // public override IMovementStrategy SetStrategy(IMovementStrategy newStrategy)
    // {
    //     // If the strategy is a BaseMovementStrategy but not an instance
    //     if (newStrategy is BaseMovementStrategy baseStrategy && !baseStrategy.IsInstance())
    //     {
    //         Debug.LogWarning(
    //             $"[{GetType().Name}] Attempting to set non-instance strategy: {newStrategy.GetType().Name}"
    //         );

    //         // Find the proper instance in the enemy controller
    //         if (enemyController != null)
    //         {
    //             // Get the type of the strategy
    //             var strategyType = newStrategy.GetType();

    //             // Look for an instance of the same type in the enemy controller
    //             foreach (var pair in enemyController.movementStrategies)
    //             {
    //                 if (pair.strategy.GetType() == strategyType && pair.strategy.IsInstance())
    //                 {
    //                     Debug.Log(
    //                         $"[{GetType().Name}] Found instance of {strategyType.Name} in enemy controller"
    //                     );
    //                     newStrategy = pair.strategy;
    //                     break;
    //                 }
    //             }
    //         }
    //     }

    //     return base.SetStrategy(newStrategy);
    // }
}
