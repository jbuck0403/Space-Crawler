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
    [Range(0.1f, 4f)]
    private float chargeSpeedMultiplier = 1.3f;

    [SerializeField]
    [Range(0.1f, 1f)]
    private float destinationReachedThreshold = 0.5f;

    [SerializeField]
    [Range(0f, 10f)]
    private float stunSelfDuraton = 2f;

    private Vector2 chargeDestination;
    private bool isPositioning = false;
    private bool isCharging = false;
    private bool isPausing = false;
    private bool isStunned = false;
    private float timePaused = 0f;
    private float timeStunned = 0f;
    private float originalMaxSpeed;

    public override void OnEnter(Transform self, Transform target)
    {
        Debug.Log("ONENTER CMS");
        base.OnEnter(self, target);

        enemyController.EnableShooting(false, true);
        // store original speed for later restoration
        originalMaxSpeed = movementHandler.maxSpeed;

        // start in positioning state
        isPositioning = true;
        isCharging = false;
        isPausing = false;
        isStunned = false;

        timePaused = 0f;
        timeStunned = 0f;
    }

    public override void OnUpdate(Transform self, Transform target)
    {
        if (!isInitialized || target == null)
            return;

        if (isPositioning)
        {
            Debug.Log("Positioning");
            if (MovementUtils.IsFacingTarget(self, target.position))
            {
                isPausing = true;
                isPositioning = false;

                return;
            }

            FaceTarget(self, target);
        }

        // Handle pause state
        if (isPausing)
        {
            Debug.Log("Pausing");
            timePaused += Time.deltaTime;

            if (timePaused >= chargePauseTime)
            {
                isPausing = false;
                isCharging = true;

                chargeDestination = target.position;
                movementHandler.ResetVelocity();

                movementHandler.maxSpeed = originalMaxSpeed * chargeSpeedMultiplier;

                return;
            }
            else
            {
                FaceTarget(self, target);
                return;
            }
        }

        if (isCharging)
        {
            Debug.Log("Charging");
            float distanceToDestination = MovementUtils.GetDistanceToTarget(
                self.position,
                chargeDestination
            );

            if (distanceToDestination <= destinationReachedThreshold)
            {
                isCharging = false;
                isStunned = true;

                // restore original speed
                MovementConfig config = GetMovementConfig();
                config.maxSpeed = originalMaxSpeed;
                movementHandler.Initialize(config);

                return;
            }

            // continue charging toward destination
            Vector2 directionToDestination = MovementUtils.GetTargetDirection(
                self.position,
                chargeDestination
            );
            movementHandler.ApplyMovement(self, directionToDestination, Time.deltaTime);
            movementHandler.ApplyRotation(self, directionToDestination, Time.deltaTime);
        }

        if (isStunned)
        {
            Debug.Log("Stunned");
            timeStunned += Time.deltaTime;

            if (timeStunned >= stunSelfDuraton)
            {
                isComplete = true;
                isStunned = false;

                return;
            }
        }
    }

    public override void OnExit()
    {
        Debug.Log("ONEXIT CMS");
        // ensure speed is restored when exiting
        if (movementHandler != null)
        {
            MovementConfig config = GetMovementConfig();
            config.maxSpeed = originalMaxSpeed;

            movementHandler.Initialize(config);
        }

        enemyController.EnableShooting(false);

        base.OnExit();
    }

    public override void OnStrategyComplete()
    {
        base.OnStrategyComplete();

        enemyController.ChangeToDefaultStrategy();
    }

    private void FaceTarget(Transform self, Transform target)
    {
        if (!MovementUtils.IsFacingTarget(self, target.position))
        {
            Vector2 directionToTarget = MovementUtils.GetTargetDirection(
                self.position,
                target.position
            );

            movementHandler.ApplyRotation(self, directionToTarget, Time.deltaTime);
        }
    }
}
