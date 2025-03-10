using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseMovementController))]
[RequireComponent(typeof(BaseWeapon))]
public abstract class BaseEnemyController : MonoBehaviour
{
    [SerializeField]
    protected MovementConfig movementConfig;

    protected BaseWeapon weapon;

    // public Transform target; // usually the player

    protected Dictionary<string, IMovementStrategy> availableMovementStrategies;
    protected Dictionary<string, IFireStrategy> availableFiringStrategies;
    protected BaseMovementController movementController;

    protected Transform target;

    public void UpdateTarget(Transform target)
    {
        this.target = target;
    }

    public Transform GetTarget()
    {
        return target;
    }

    protected virtual void Awake()
    {
        movementController = GetComponent<BaseMovementController>();
        target = movementController.currentTarget;
        InitializeStrategies();
    }

    protected virtual void Start()
    {
        SetDefaultStrategy();
    }

    // initialize and add desired strategies to availableStrategies
    protected abstract void InitializeStrategies();
    protected abstract void SetDefaultStrategy();

    protected void ChangeMovementStrategy(string strategyKey)
    {
        if (availableMovementStrategies.TryGetValue(strategyKey, out IMovementStrategy strategy))
        {
            movementController.SetStrategy(strategy);
        }
    }

    protected void ChangeFiringStrategy(string strategyKey)
    {
        if (availableFiringStrategies.TryGetValue(strategyKey, out IFireStrategy strategy))
        {
            weapon.SetStrategy(strategy);
        }
    }
}
