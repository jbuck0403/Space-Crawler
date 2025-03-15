using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseMovementController))]
public abstract class BaseEnemyController : BaseCharacterController
{
    [SerializeField]
    public List<MovementStrategyPair> movementStrategies;

    [SerializeField]
    protected MovementStrategyType defaultStrategy;

    // protected List<MovementStrategyPair> movementStrategyInstances;
    protected BaseMovementController movementController;
    protected HealthSystem healthSystem;

    protected Transform target;

    public void UpdateTarget(Transform target)
    {
        this.target = target;
    }

    public Transform GetTarget()
    {
        return target;
    }

    protected override void Awake()
    {
        base.Awake();

        movementController = GetComponent<BaseMovementController>();
        healthSystem = GetComponent<HealthSystem>();
    }

    protected override void Start()
    {
        InitializeStrategies();
        movementController.Initialize(this, movementConfig);

        target = movementController.currentTarget;
        base.Start();

        ChangeToDefaultStrategy();
    }

    protected virtual void InitializeStrategies()
    {
        bool definesAvailableDefault = false;

        if (movementStrategies == null || movementStrategies.Count == 0)
        {
            Debug.LogWarning($"No movement strategies assigned to {gameObject.name}");
            return;
        }

        Debug.Log($"[{gameObject.name}] Initializing {movementStrategies.Count} strategies");

        movementStrategies.ForEach(pair =>
        {
            if (pair.strategyType == MovementStrategyType.Default)
            {
                definesAvailableDefault = true;
            }
            if (pair.strategy == null)
            {
                Debug.LogWarning($"Null strategy found in {gameObject.name}");
                return;
            }
            pair.strategy = pair.strategy.Initialize(movementConfig, this);
            // print(pair.strategy.IsInstance());
        });

        if (!definesAvailableDefault)
        {
            defaultStrategy = movementStrategies[0].strategyType;
        }
    }

    public virtual void ChangeToDefaultStrategy()
    {
        ChangeMovementStrategy(defaultStrategy);
    }

    public T ChangeMovementStrategy<T>()
        where T : BaseMovementStrategy
    {
        Debug.Log($"[BaseEnemyController] Changing to strategy type {typeof(T).Name}");
        foreach (MovementStrategyPair pair in movementStrategies)
        {
            if (pair.strategy is T)
            {
                Debug.Log(
                    $"[BaseEnemyController] Found matching strategy, isInstance: {pair.strategy.IsInstance()}"
                );
                IMovementStrategy strategy = movementController.SetStrategy(pair.strategy);
                Debug.Log(
                    $"[BaseEnemyController] Strategy set, returning instance status: {((BaseMovementStrategy)strategy).IsInstance()}"
                );
                return strategy as T;
            }
        }
        return null;
    }

    public void ChangeMovementStrategy(MovementStrategyType strategyType)
    {
        Debug.Log($"[BaseEnemyController] Changing to strategy type {strategyType}");
        foreach (MovementStrategyPair pair in movementStrategies)
        {
            if (pair.strategyType == strategyType)
            {
                Debug.Log(
                    $"[BaseEnemyController] Found matching strategy, isInstance: {pair.strategy.IsInstance()}"
                );
                IMovementStrategy strategy = movementController.SetStrategy(pair.strategy);
                Debug.Log(
                    $"[BaseEnemyController] Strategy set, returning instance status: {((BaseMovementStrategy)strategy).IsInstance()}"
                );
                return;
            }
        }
    }

    public bool HasMovementStrategy(MovementStrategyType strategyType)
    {
        foreach (MovementStrategyPair pair in movementStrategies)
        {
            if (pair.strategyType == strategyType)
            {
                return true;
            }
        }

        return false;
    }

    public T GetMovementStrategy<T>()
        where T : BaseMovementStrategy
    {
        foreach (MovementStrategyPair pair in movementStrategies)
        {
            if (pair.strategy is T)
            {
                return pair.strategy as T;
            }
        }

        return null;
    }
}

[Serializable]
public class MovementStrategyPair
{
    public MovementStrategyType strategyType;
    public BaseMovementStrategy strategy;
}

public enum MovementStrategyType
{
    Default,
    Circle,
    Charge,
    Retreat,
    Cautious,
}
