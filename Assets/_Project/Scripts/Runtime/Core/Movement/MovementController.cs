using UnityEngine;

public class MovementController : MonoBehaviour
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
        // Check if current strategy allows exit
        if (currentStrategy != null && !currentStrategy.CanExit())
            return;

        // Exit current strategy
        currentStrategy?.OnExit();

        // Set and initialize new strategy
        currentStrategy = newStrategy;
        movementHandler = new MovementHandler(newStrategy.GetMovementConfig());

        // Enter new strategy
        currentStrategy.OnEnter(transform, currentTarget);
    }

    private void Update()
    {
        if (currentStrategy == null || movementHandler == null)
            return;

        // Update current strategy
        currentStrategy.OnUpdate(transform, currentTarget);

        // Check if strategy is complete and needs changing
        if (currentStrategy.IsComplete())
        {
            // Handle strategy completion (could trigger new strategy)
        }
    }
}
