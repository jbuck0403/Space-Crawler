using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public abstract class BaseEnemyController : MonoBehaviour
{
    [SerializeField]
    protected MovementConfig movementConfig;

    protected Transform target; // Usually the player

    protected Dictionary<string, IMovementStrategy> availableStrategies;
    protected MovementController movementController;

    protected virtual void Awake()
    {
        movementController = GetComponent<MovementController>();
        target = movementController.DefaultTarget;
        InitializeStrategies();
    }

    protected virtual void Start()
    {
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
