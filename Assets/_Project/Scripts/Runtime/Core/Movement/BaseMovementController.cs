using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BaseEnemyController))]
public class BaseMovementController : StrategyController<IMovementStrategy>
{
    private Transform DefaultTarget;
    public Transform currentTarget { get; private set; }
    private Transform previousTarget;

    BaseEnemyController enemyController;

    public void Awake()
    {
        DefaultTarget = GameObject.FindGameObjectWithTag("Player").transform;
        currentTarget = DefaultTarget;
        enemyController = GetComponent<BaseEnemyController>();
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
}
