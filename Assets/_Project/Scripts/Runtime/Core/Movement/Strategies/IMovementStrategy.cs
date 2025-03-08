using UnityEngine;

public interface IMovementStrategy
{
    // state pattern methods
    void OnEnter(Transform self, Transform target);
    void OnUpdate(Transform self, Transform target);
    void OnExit();

    // configuration
    MovementConfig GetMovementConfig();

    // optional: helper methods for checking state conditions
    bool CanExit() => true;
    bool IsComplete() => false;
}
