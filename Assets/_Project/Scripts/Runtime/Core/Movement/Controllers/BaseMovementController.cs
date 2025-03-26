using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BaseEnemyController))]
public class BaseMovementController : StrategyController<IMovementStrategy>
{
    private Transform defaultTarget;
    public Transform CurrentTarget { get; private set; }
    private Transform previousTarget;

    protected BaseEnemyController enemyController;
    protected MovementConfig config;

    public void Awake()
    {
        // defaultTarget = GameObject.FindGameObjectWithTag("Player").transform;
        // CurrentTarget = defaultTarget;
        enemyController = GetComponent<BaseEnemyController>();
    }

    public void Initialize(
        BaseEnemyController enemyController,
        MovementConfig config,
        Transform defaultTarget
    )
    {
        this.defaultTarget = defaultTarget;
        CurrentTarget = this.defaultTarget;

        this.enemyController = enemyController;
        this.config = config;
    }

    public Transform GetCurrentTarget()
    {
        return CurrentTarget.transform;
    }

    public void ChangeTarget(Transform target)
    {
        previousTarget = CurrentTarget;
        CurrentTarget = target;

        if (target == null)
        {
            currentStrategy = null;
        }
    }

    public Transform GetDefaultTarget()
    {
        return defaultTarget;
    }

    public void ChangeDefaultTarget(Transform target)
    {
        defaultTarget = target;
    }

    public void TargetPreviousTarget()
    {
        ChangeTarget(previousTarget);
    }

    public void TargetDefaultTarget()
    {
        ChangeTarget(defaultTarget);
    }

    protected override void OnStrategyExit(IMovementStrategy strategy)
    {
        strategy.OnExit();
    }

    protected override void OnStrategyEnter(IMovementStrategy strategy)
    {
        strategy.OnEnter(transform, CurrentTarget);
    }

    protected override void OnStrategyUpdate(IMovementStrategy strategy)
    {
        strategy.OnUpdate(transform, CurrentTarget);
    }

    public MovementHandler GetMovementHandler()
    {
        return currentStrategy.GetMovementHandler();
    }
}
