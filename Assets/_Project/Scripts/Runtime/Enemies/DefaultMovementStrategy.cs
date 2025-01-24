using UnityEngine;

public class DefaultMovementStrategy : IMovementStrategy
{
    private readonly MovementConfig config;
    private readonly float followDistance = 5f;
    private bool isInitialized;

    public DefaultMovementStrategy(MovementConfig config)
    {
        this.config = config;
    }

    public void OnEnter(Transform self, Transform target)
    {
        isInitialized = true;
    }

    public void OnUpdate(Transform self, Transform target)
    {
        if (!isInitialized || target == null)
            return;

        // Simple follow behavior
        Vector3 directionToTarget = (target.position - self.position).normalized;
        float distanceToTarget = Vector3.Distance(self.position, target.position);

        // Only move if we're further than follow distance
        if (distanceToTarget > followDistance)
        {
            self.position += directionToTarget * config.maxSpeed * Time.deltaTime;
        }
    }

    public void OnExit()
    {
        isInitialized = false;
    }

    public MovementConfig GetMovementConfig() => config;
}
