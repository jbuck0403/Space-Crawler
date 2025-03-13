using UnityEngine;

public abstract class StrategyController<TStrategy> : MonoBehaviour
    where TStrategy : IStrategy
{
    protected TStrategy currentStrategy;

    public virtual TStrategy SetStrategy(TStrategy newStrategy)
    {
        Debug.Log(
            $"[{GetType().Name}] Setting strategy: {(newStrategy == null ? "null" : newStrategy.GetType().Name)}"
        );
        if (newStrategy is BaseMovementStrategy bms)
        {
            Debug.Log($"[{GetType().Name}] New strategy isInstance status: {bms.IsInstance()}");
        }

        // Check if current strategy allows exit
        if (currentStrategy != null && !currentStrategy.CanExit())
        {
            Debug.Log($"[{GetType().Name}] Current strategy doesn't allow exit");
            return currentStrategy;
        }

        // Exit current strategy
        if (currentStrategy != null)
        {
            Debug.Log(
                $"[{GetType().Name}] Exiting current strategy: {currentStrategy.GetType().Name}"
            );
            if (currentStrategy is BaseMovementStrategy currentBms)
            {
                Debug.Log(
                    $"[{GetType().Name}] Current strategy isInstance status before exit: {currentBms.IsInstance()}"
                );
            }
            OnStrategyExit(currentStrategy);
        }

        // Set new strategy
        currentStrategy = newStrategy;

        // Enter new strategy
        if (currentStrategy != null)
        {
            Debug.Log(
                $"[{GetType().Name}] Entering new strategy: {currentStrategy.GetType().Name}"
            );
            if (currentStrategy is BaseMovementStrategy newCurrentBms)
            {
                Debug.Log(
                    $"[{GetType().Name}] New current strategy isInstance status on enter: {newCurrentBms.IsInstance()}"
                );
            }
            OnStrategyEnter(currentStrategy);

            // Check instance status after enter
            if (currentStrategy is BaseMovementStrategy finalBms)
            {
                Debug.Log(
                    $"[{GetType().Name}] Strategy isInstance status after enter: {finalBms.IsInstance()}"
                );
            }
        }

        return currentStrategy;
    }

    protected virtual void Update()
    {
        if (currentStrategy == null)
            return;

        // Update current strategy
        OnStrategyUpdate(currentStrategy);

        // Check if strategy is complete
        if (currentStrategy.IsComplete())
        {
            currentStrategy.OnStrategyComplete();
        }
    }

    protected abstract void OnStrategyExit(TStrategy strategy);
    protected abstract void OnStrategyEnter(TStrategy strategy);
    protected abstract void OnStrategyUpdate(TStrategy strategy);
}
