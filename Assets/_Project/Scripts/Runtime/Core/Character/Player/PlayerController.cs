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

    // Add dash timer variables
    private bool applyingExternalMovement = false;
    private float externalMovementDisableTime = 0f;

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

    private void Update()
    {
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
        HandleWeaponAbility();
    }

    private bool HandleWeaponSwap()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            return weaponHandler.SwitchToWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            return weaponHandler.SwitchToWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            return weaponHandler.SwitchToWeapon(2);
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
        // calculate new position and rotation
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
