using UnityEngine;

// [RequireComponent(MovementHandler)]
public class MovementController : MonoBehaviour
{
    [SerializeField]
    private MovementConfig defaultConfig;

    [SerializeField]
    private Transform target;

    private MovementHandler movementHandler;
    private IMovementStrategy currentStrategy;

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
        currentStrategy.OnEnter(transform, target);
    }

    private void Update()
    {
        if (currentStrategy == null || movementHandler == null)
            return;

        // Update current strategy
        currentStrategy.OnUpdate(transform, target);

        // Check if strategy is complete and needs changing
        if (currentStrategy.IsComplete())
        {
            // Handle strategy completion (could trigger new strategy)
        }
    }
}
