using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseMovementController))]
public abstract class BaseEnemyController : BaseCharacterController
{
    [SerializeField]
    protected List<MovementStrategyPair> movementStrategies;

    [SerializeField]
    protected MovementStrategyPair defaultStrategy;
    protected BaseMovementController movementController;
    protected HealthSystem healthSystem;

    protected Transform target;
    public bool isComplete = false;

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
        InitializeStrategies();
    }

    protected virtual void Start()
    {
        target = movementController.currentTarget;
        SetDefaultStrategy();
    }

    protected virtual void InitializeStrategies()
    {
        if (movementStrategies == null || movementStrategies.Count == 0)
        {
            Debug.LogWarning($"No movement strategies assigned to {gameObject.name}");
            return;
        }

        movementStrategies.ForEach(pair =>
        {
            if (pair.strategy == null)
            {
                Debug.LogWarning($"Null strategy found in {gameObject.name}");
                return;
            }
            pair.strategy.Initialize(movementConfig, this);
        });
    }

    public virtual void SetDefaultStrategy()
    {
        ChangeMovementStrategy(defaultStrategy.strategyType);
    }

    public void ChangeMovementStrategy(MovementStrategyType strategyType)
    {
        foreach (MovementStrategyPair pair in movementStrategies)
        {
            if (pair.strategyType == strategyType)
            {
                isComplete = false;
                movementController.SetStrategy(pair.strategy);
                return;
            }
        }
    }

    protected BaseMovementStrategy GetMovementStrategy(MovementStrategyType strategyType)
    {
        foreach (MovementStrategyPair pair in movementStrategies)
        {
            if (pair.strategyType == strategyType)
            {
                return pair.strategy;
            }
        }

        return null;
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
}

[Serializable]
public struct MovementStrategyPair
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
    HitAndRun,
    Cautious,
}
