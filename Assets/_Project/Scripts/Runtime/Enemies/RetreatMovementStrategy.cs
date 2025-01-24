using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetreatMovementStrategy : IMovementStrategy
{
    private readonly MovementConfig config;
    private readonly float retreatDistance = 15f;
    private bool isInitialized;

    public RetreatMovementStrategy(MovementConfig config)
    {
        this.config = config;
    }

    public void OnEnter(Transform self, Transform target)
    {
        isInitialized = true;
    }

    public void OnUpdate(Transform self, Transform target) { }

    public void OnExit()
    {
        isInitialized = false;
    }

    public MovementConfig GetMovementConfig() => config;
}
