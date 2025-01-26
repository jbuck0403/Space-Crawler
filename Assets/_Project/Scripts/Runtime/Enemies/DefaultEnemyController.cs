using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(DamageHandler))]
[RequireComponent(typeof(HealthSystem))]
public class DefaultEnemyController : BaseEnemyController
{
    [SerializeField]
    private float RetreatHealthThreshold = 10f;

    private void Update()
    {
        if (movementController == null || target == null)
            return;

        // float distanceToTarget = MovementUtils.GetDistanceToTarget(
        //     transform.position,
        //     target.position
        // );
        // if (distanceToTarget <= 7.5f)
        // {
        //     ChangeStrategy(Strategies.Retreat);
        // }
        // else if (distanceToTarget >= 15f)
        // {
        //     ChangeStrategy(Strategies.Default);
        // }
    }

    protected override void InitializeStrategies()
    {
        availableStrategies = new Dictionary<string, IMovementStrategy>
        {
            { Strategies.Default, new DefaultMovementStrategy(movementConfig) },
            { Strategies.Retreat, new RetreatMovementStrategy(movementConfig) }
        };
    }

    protected override void SetDefaultStrategy()
    {
        ChangeStrategy(Strategies.Default);
    }

    // subscribe to "low health" event in health module (TBI)
    private void Retreat(float currentHealth)
    {
        if (currentHealth < RetreatHealthThreshold)
        {
            ChangeStrategy(Strategies.Retreat);
        }
    }

    // // Optional: Add health system reference and handle strategy changes based on health
    // private void OnHealthChanged(float healthPercent)
    // {
    //     // Example: Change strategy based on health
    //     if (healthPercent < 0.3f)
    //     {
    //         ChangeStrategy(Strategies.Retreat);
    //     }
    // }
}
