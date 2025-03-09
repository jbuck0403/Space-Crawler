using UnityEngine;

public class BaseMovementController : MonoBehaviour
{
    [SerializeField]
    public Transform DefaultTarget;
    private Transform currentTarget;
    private Transform previousTarget;

    private IMovementStrategy currentStrategy;
    private MovementHandler movementHandler;

    public void Start()
    {
        currentTarget = DefaultTarget;
    }

    public void ChangeTarget(Transform target)
    {
        previousTarget = this.currentTarget;
        currentTarget = target;
    }

    public void TargetPreviousTarget()
    {
        ChangeTarget(previousTarget);
    }

    public void TargetDefaultTarget()
    {
        ChangeTarget(DefaultTarget);
    }

    public void SetStrategy(IMovementStrategy newStrategy)
    {
        // check if current strategy allows exit
        if (currentStrategy != null && !currentStrategy.CanExit())
            return;

        // exit current strategy
        currentStrategy?.OnExit();

        // set and initialize new strategy
        currentStrategy = newStrategy;
        movementHandler = new MovementHandler(newStrategy.GetMovementConfig());

        // enter new strategy
        currentStrategy.OnEnter(transform, currentTarget);
    }

    private void Update()
    {
        if (currentStrategy == null || movementHandler == null)
            return;

        // update current strategy
        currentStrategy.OnUpdate(transform, currentTarget);

        // check if strategy is complete and needs changing
        if (currentStrategy.IsComplete())
        {
            currentStrategy.OnStrategyComplete();
        }
    }
}
