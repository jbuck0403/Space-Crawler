using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController
    : BaseCharacterController,
        IProjectileDataProvider,
        IWeaponAbilityDataProvider
{
    private CollisionAwareMovementHandler movementHandler;
    private Vector2 moveInput;
    private Vector2 aimDirection;
    private Camera mainCamera;

    private Transform currentTarget;

    private bool applyingExternalMovement = false;
    private float externalMovementDisableTime = 0f;

    [SerializeField]
    private bool initialized = false;

    protected override void Awake()
    {
        base.Awake();

        movementHandler = new CollisionAwareMovementHandler(movementConfig);

        if (movementHandler != null)
        {
            movementHandler.InitializeCollisionDetection(obstacleLayers, transform);
        }
        mainCamera = Camera.main;
    }

    public void SetApplyingExternalMovement(bool value, float disableTime = default)
    {
        applyingExternalMovement = value;

        if (disableTime != default)
        {
            externalMovementDisableTime = Time.time + disableTime;
        }
    }

    private void HandleDisablingExternalMovement()
    {
        if (Time.time >= externalMovementDisableTime)
        {
            SetApplyingExternalMovement(false);
            externalMovementDisableTime = default;
        }
    }

    public void Initialize(bool initialize)
    {
        initialized = initialize;
    }

    private void Update()
    {
        if (!initialized)
            return;

        HandleDisablingExternalMovement();
        GetAimDirection();

        if (!applyingExternalMovement)
        {
            GetMoveInput();
            MoveAndFaceTarget();
        }
        else
        {
            // give external movement commands a chance to apply their velocity changes
            movementHandler.ApplyMovement(transform, Vector2.zero, Time.deltaTime);
        }

        if (Input.GetMouseButton(0))
        {
            EnableShooting(true);
        }
        else
        {
            EnableShooting(false);
        }

        HandleShooting();
        HandleWeaponSwap();
        HandleProjectileSwap();
        HandleWeaponAbility();
    }

    private bool HandleWeaponSwap()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            return weaponHandler.SwitchToPreviousWeapon();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            return weaponHandler.SwitchToNextWeapon();
        }

        return false;
    }

    private bool HandleProjectileSwap()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            return weaponHandler.SwitchToPreviousProjectile();
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            return weaponHandler.SwitchToNextProjectile();
        }

        return false;
    }

    private void HandleWeaponAbility()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            weaponHandler.ActivateWeaponAbility(this);
        }
    }

    private void GetMoveInput()
    {
        moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;
    }

    private void GetAimDirection()
    {
        // get mouse position in world space
        Vector2 mouseWorldPosition = GetMouseWorldPosition();
        aimDirection = (mouseWorldPosition - (Vector2)transform.position).normalized;
    }

    private void MoveAndFaceTarget()
    {
        Vector2 newPosition = movementHandler.CalculateMovement(
            moveInput,
            transform.position,
            Time.deltaTime
        );

        float newRotation = movementHandler.CalculateRotation(
            aimDirection,
            transform.eulerAngles.z,
            movementConfig.rotationSpeed,
            Time.deltaTime
        );

        movementHandler.ApplyMovementAndRotation(transform, newPosition, newRotation);
    }

    public override MovementHandler GetMovementHandler()
    {
        return movementHandler;
    }

    public Transform GetTarget()
    {
        return currentTarget;
    }

    public Vector2 GetAbilityTarget()
    {
        Vector2 mouseWorldPosition = GetMouseWorldPosition();
        return mouseWorldPosition;
    }

    public Transform GetWeaponOwnerTransform()
    {
        return transform;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(
            new Vector3(
                Input.mousePosition.x,
                Input.mousePosition.y,
                -mainCamera.transform.position.z
            )
        );

        return mouseWorldPosition;
    }

    public Transform GetFirePoint()
    {
        return weaponHandler.FirePoint;
    }
}
