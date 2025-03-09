using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetreatMovementStrategy : IMovementStrategy
{
    private readonly MovementConfig config;
    private readonly float retreatDistance = 15f;
    private readonly MovementHandler movementHandler;
    private bool isInitialized;

    public RetreatMovementStrategy(MovementConfig config)
    {
        this.config = config;
        movementHandler = new MovementHandler(config);
    }

    public void OnEnter(Transform self, Transform target)
    {
        isInitialized = true;
    }

    public void OnUpdate(Transform self, Transform target)
    {
        if (!isInitialized || target == null)
            return;

        // get direction from target to self (opposite of direction to target)
        Vector2 directionFromTarget = MovementUtils.GetTargetDirection(
            target.position,
            self.position
        );

        float distanceFromTarget = MovementUtils.GetDistanceToTarget(
            self.position,
            target.position
        );

        // only retreat if we're closer than retreat distance
        Vector2 targetDirection =
            distanceFromTarget < retreatDistance ? directionFromTarget : Vector2.zero;

        movementHandler.ApplyMovement(self, targetDirection, Time.deltaTime);
    }

    public void OnExit()
    {
        isInitialized = false;
    }

    public void OnStrategyComplete() { }

    public MovementConfig GetMovementConfig() => config;
}
