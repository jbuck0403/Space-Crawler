using UnityEngine;

public abstract class StrategyController<TStrategy> : MonoBehaviour
    where TStrategy : IStrategy
{
    protected TStrategy currentStrategy;

    public virtual void SetStrategy(TStrategy newStrategy)
    {
        // Check if current strategy allows exit
        if (currentStrategy != null && !currentStrategy.CanExit())
            return;

        // Exit current strategy
        if (currentStrategy != null)
        {
            OnStrategyExit(currentStrategy);
        }

        // Set new strategy
        currentStrategy = newStrategy;

        // Enter new strategy
        if (currentStrategy != null)
        {
            OnStrategyEnter(currentStrategy);
        }
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
