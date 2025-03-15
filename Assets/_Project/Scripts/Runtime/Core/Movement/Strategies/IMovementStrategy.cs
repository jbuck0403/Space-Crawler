using UnityEngine;

public interface IMovementStrategy : IStrategy
{
    // configuration
    MovementConfig GetMovementConfig();
    MovementHandler GetMovementHandler();

    // state pattern methods
    void OnEnter(Transform self, Transform target);
    void OnUpdate(Transform self, Transform target);
    void OnExit();
}
