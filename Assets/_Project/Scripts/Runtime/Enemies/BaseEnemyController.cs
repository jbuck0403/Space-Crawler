using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemyController : MonoBehaviour
{
    [SerializeField]
    protected MovementConfig movementConfig;

    [SerializeField]
    protected Transform target; // Usually the player

    protected Dictionary<string, IMovementStrategy> availableStrategies;
    protected MovementController movementController;

    protected virtual void Awake()
    {
        movementController = GetComponent<MovementController>();
        InitializeStrategies();
    }

    protected virtual void Start()
    {
        // Set initial strategy
        SetDefaultStrategy();
    }

    // Initialize and add desired strategies to availableStrategies
    protected abstract void InitializeStrategies();
    protected abstract void SetDefaultStrategy();

    protected void ChangeStrategy(string strategyKey)
    {
        if (availableStrategies.TryGetValue(strategyKey, out IMovementStrategy strategy))
        {
            movementController.SetStrategy(strategy);
        }
    }
}
