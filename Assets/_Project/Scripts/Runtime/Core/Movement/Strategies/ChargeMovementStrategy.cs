using System;
using UnityEngine;
using UnityEngine.TextCore.Text;

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

    [SerializeField]
    [Range(1f, 50f)]
    private float decelerationForce = 20f;

    [SerializeField]
    [Min(0f)]
    private float decelerationFlex = 1f;

    [SerializeField]
    private DamageProfile damageProfile;

    private Vector2 chargeDestination;
    private bool isPositioning = false;
    private bool isPausing = false;
    private bool isCharging = false;
    private bool isDecelerating = false;
    private bool isStunned = false;
    private float timePaused = 0f;
    private float timeStunned = 0f;
    private float originalMaxSpeed;
    private Vector2 chargeVelocity;

    CharacterProjectile characterProjectile;

    public override void OnEnter(Transform self, Transform target)
    {
        base.OnEnter(self, target);
        enemyController.EnableShooting(false, true);
        InitializeCharacterProjectile();
        originalMaxSpeed = movementHandler.maxSpeed;
        ResetState();
    }

    private void InitializeCharacterProjectile()
    {
        characterProjectile = enemyController.GetComponent<CharacterProjectile>();
        DamageData newDamageData = damageProfile.CreateDamageData(enemyController.transform);
        characterProjectile.Initialize(newDamageData);
    }

    public override void OnUpdate(Transform self, Transform target)
    {
        if (!isInitialized || target == null)
            return;

        if (isPositioning)
            HandlePositioning(self, target);

        if (isPausing)
            HandlePausing(self, target);

        if (isCharging)
            HandleCharging(self, target);

        if (isDecelerating)
            HandleDeceleration(self);

        if (isStunned)
            HandleStunned();
    }

    public override void OnExit()
    {
        if (movementHandler != null)
        {
            RestoreOriginalSpeed();
        }

        enemyController.EnableShooting(false);
        base.OnExit();
    }

    public override void OnStrategyComplete()
    {
        base.OnStrategyComplete();
        enemyController.ChangeToDefaultStrategy();
    }

    private void ResetState()
    {
        isPositioning = true;
        isPausing = false;
        isCharging = false;
        isDecelerating = false;
        isStunned = false;
        timePaused = 0f;
        timeStunned = 0f;
    }

    private void HandlePositioning(Transform self, Transform target)
    {
        if (MovementUtils.IsFacingTarget(self, target.position))
        {
            isPausing = true;
            isPositioning = false;
            return;
        }

        FaceTarget(self, target);
    }

    private void HandlePausing(Transform self, Transform target)
    {
        timePaused += Time.deltaTime;

        if (timePaused >= chargePauseTime)
        {
            StartCharging(self, target);
            return;
        }

        FaceTarget(self, target);
    }

    private void StartCharging(Transform self, Transform target)
    {
        isPausing = false;
        isCharging = true;
        chargeDestination = target.position;
        movementHandler.ResetVelocity();
        movementHandler.maxSpeed = originalMaxSpeed * chargeSpeedMultiplier;
    }

    private void HandleCharging(Transform self, Transform target)
    {
        float distanceToDestination = MovementUtils.GetDistanceToTarget(
            self.position,
            chargeDestination
        );

        if (distanceToDestination <= destinationReachedThreshold)
        {
            StartDeceleration();
            return;
        }

        Vector2 directionToDestination = MovementUtils.GetTargetDirection(
            self.position,
            chargeDestination
        );
        movementHandler.ApplyMovement(self, directionToDestination, Time.deltaTime);
        movementHandler.ApplyRotation(self, directionToDestination, Time.deltaTime);
    }

    private void StartDeceleration()
    {
        isCharging = false;
        isDecelerating = true;
        chargeVelocity = movementHandler.GetCurrentVelocity();
    }

    private void HandleDeceleration(Transform self)
    {
        Vector2 currentVelocity = movementHandler.GetCurrentVelocity();
        Vector2 decelerationDirection = -currentVelocity.normalized;
        Vector2 decelerationVector = decelerationDirection * decelerationForce * Time.deltaTime;
        Vector2 newVelocity = currentVelocity + decelerationVector;

        if (Vector2.Dot(newVelocity, chargeVelocity) <= decelerationFlex)
        {
            StartStun();
            return;
        }

        movementHandler.ApplyVelocity(newVelocity);
        Vector2 movement = newVelocity * Time.deltaTime;
        self.position = (Vector2)self.position + movement;
    }

    private void StartStun()
    {
        isDecelerating = false;
        isStunned = true;
        movementHandler.ResetVelocity();
        RestoreOriginalSpeed();
    }

    private void HandleStunned()
    {
        timeStunned += Time.deltaTime;

        if (timeStunned >= stunSelfDuraton)
        {
            isComplete = true;
            isStunned = false;
        }
    }

    private void RestoreOriginalSpeed()
    {
        MovementConfig config = GetMovementConfig();
        config.maxSpeed = originalMaxSpeed;
        movementHandler.Initialize(config);
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
