using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseMovementController))]
public class BaseEnemyController : BaseCharacterController, IProjectileDataProvider
{
    public List<MovementStrategyPair> movementStrategies;

    [SerializeField]
    public Transform defaultTarget;

    [SerializeField]
    protected MovementStrategyType defaultStrategy;

    [SerializeField]
    public LayerMask combatantLayers;

    protected BaseMovementController movementController;
    protected HealthSystem healthSystem;

    protected bool initialized = false;

    protected override void Awake()
    {
        base.Awake();

        movementController = GetComponent<BaseMovementController>();
        healthSystem = GetComponent<HealthSystem>();
    }

    public virtual bool Initialize(Transform defaultTarget)
    {
        if (movementController != null && defaultTarget != null)
        {
            this.defaultTarget = defaultTarget;
            movementController.Initialize(this, movementConfig, this.defaultTarget);
            InitializeStrategies();
            ChangeToDefaultStrategy();

            initialized = true;
            return initialized;
        }
        else
        {
            Debug.Log(
                $"{name} MovementController: {movementController == null} Default Target: {defaultTarget}"
            );
        }

        return false;
    }

    protected override void HandleShooting(Transform target = null)
    {
        if (
            weaponHandler != null
            && !MovementUtils.TargetViewObstructed(
                weaponHandler.FirePoint,
                movementController.CurrentTarget.gameObject,
                obstacleLayers + combatantLayers
            )
        )
        {
            base.HandleShooting(target);
        }
        else
        {
            StopFiringWeapon();
        }
    }

    // protected virtual void Start()
    // {
    //     InitializeStrategies();
    //     ChangeToDefaultStrategy();
    // }

    public void UpdateTarget(Transform target, bool setDefault = false)
    {
        if (movementController != null)
        {
            movementController.ChangeTarget(target);

            if (setDefault || movementController.GetDefaultTarget() == null)
            {
                movementController.ChangeDefaultTarget(target);
            }
        }
    }

    public Transform GetTarget()
    {
        if (movementController != null)
        {
            return movementController.GetCurrentTarget();
        }

        return null;
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

    public override MovementHandler GetMovementHandler()
    {
        return movementController.GetMovementHandler();
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
