using UnityEngine;

public interface IMovementStrategy
{
    // State pattern methods
    void OnEnter(Transform self, Transform target);
    void OnUpdate(Transform self, Transform target);
    void OnExit();

    // Configuration
    MovementConfig GetMovementConfig();

    // Optional: Helper methods for checking state conditions
    bool CanExit() => true;
    bool IsComplete() => false;
}
