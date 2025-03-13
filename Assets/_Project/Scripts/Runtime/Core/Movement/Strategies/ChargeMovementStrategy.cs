using System.Collections;
using UnityEngine;

[CreateAssetMenu(
    fileName = "ChargeMovementStrategy",
    menuName = "SpaceShooter/Strategies/Movement Strategies/Charge"
)]
public class ChargeMovementStrategy : BaseMovementStrategy
{
    [SerializeField]
    [Range(0.5f, 2f)]
    private float chargePauseTime = 1f;

    [SerializeField]
    [Range(1f, 2f)]
    private float chargeSpeedMultiplier = 1.3f;

    [SerializeField]
    [Range(0.1f, 1f)]
    private float destinationReachedThreshold = 0.5f;

    private Vector2 chargeDestination;
    private bool isCharging = false;
    private bool isPausing = false;
    private float pauseStartTime;
    private float originalMaxSpeed;

    public override void OnEnter(Transform self, Transform target)
    {
        base.OnEnter(self, target);

        // Store original speed for later restoration
        originalMaxSpeed = movementHandler.maxSpeed;

        // Start in pausing state
        isPausing = true;
        isCharging = false;
        pauseStartTime = Time.time;

        // Face the target immediately
        Vector2 directionToTarget = MovementUtils.GetTargetDirection(
            self.position,
            target.position
        );
        movementHandler.ApplyRotation(self, directionToTarget, 1f);
    }

    public override void OnUpdate(Transform self, Transform target)
    {
        if (!isInitialized || target == null)
            return;

        // Handle pause state
        if (isPausing)
        {
            // Wait for pause time to complete
            if (Time.time - pauseStartTime >= chargePauseTime)
            {
                // Transition to charging state
                isPausing = false;
                isCharging = true;

                // Set charge destination to current target position
                chargeDestination = target.position;

                // Apply speed boost for charging

                movementHandler.maxSpeed = originalMaxSpeed * chargeSpeedMultiplier;
            }
            else
            {
                // While pausing, just face the target
                Vector2 directionToTarget = MovementUtils.GetTargetDirection(
                    self.position,
                    target.position
                );
                movementHandler.ApplyRotation(self, directionToTarget, Time.deltaTime);
                return;
            }
        }

        // Handle charging state
        if (isCharging)
        {
            // Check if we've reached the destination
            float distanceToDestination = MovementUtils.GetDistanceToTarget(
                self.position,
                chargeDestination
            );

            if (distanceToDestination <= destinationReachedThreshold)
            {
                // Charge complete
                isCharging = false;

                // Restore original speed
                MovementConfig config = GetMovementConfig();
                config.maxSpeed = originalMaxSpeed;

                // Signal completion
                isComplete = true;
                return;
            }

            // Continue charging toward destination
            Vector2 directionToDestination = MovementUtils.GetTargetDirection(
                self.position,
                chargeDestination
            );
            movementHandler.ApplyMovement(self, directionToDestination, Time.deltaTime);
            movementHandler.ApplyRotation(self, directionToDestination, Time.deltaTime);
        }
    }

    public override void OnExit()
    {
        // Ensure speed is restored when exiting
        if (movementHandler != null)
        {
            MovementConfig config = GetMovementConfig();
            config.maxSpeed = originalMaxSpeed;
        }

        base.OnExit();

        enemyController.ChangeMovementStrategy(MovementStrategyType.Default);
    }
}
