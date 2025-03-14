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

        // Handle instantiation if needed
        if (!newStrategy.IsInstance())
        {
            Debug.Log($"[{GetType().Name}] Creating instance of strategy");
            newStrategy = (TStrategy)newStrategy.CreateInstance();
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
            OnStrategyEnter(currentStrategy);
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
