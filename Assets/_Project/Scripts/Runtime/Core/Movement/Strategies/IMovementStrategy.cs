using UnityEngine;

public interface IMovementStrategy
{
    // configuration
    MovementConfig GetMovementConfig();

    // state pattern methods
    void OnEnter(Transform self, Transform target);
    void OnUpdate(Transform self, Transform target);
    void OnExit();

    // optional: helper methods for checking state conditions
    bool CanExit() => true;
    bool IsComplete() => false;
    void OnStrategyComplete(); // called when strategy reads IsComplete() == true
}
