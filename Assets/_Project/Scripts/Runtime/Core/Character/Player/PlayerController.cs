using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : BaseCharacterController, IProjectileDataProvider
{
    private MovementHandler movementHandler;
    private Vector2 moveInput;
    private Vector2 aimDirection;
    private Camera mainCamera;

    protected override void Awake()
    {
        base.Awake();

        movementHandler = new MovementHandler(movementConfig);
        mainCamera = Camera.main;
    }

    private void Update()
    {
        GetMoveInput();
        GetAimDirection();
        MoveAndFaceTarget();

        if (Input.GetMouseButton(0))
        {
            EnableShooting(true);
        }
        else
        {
            EnableShooting(false);
        }

        // Handle actual shooting (now delegated to BaseCharacterController)
        HandleShooting();
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
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(
            new Vector3(
                Input.mousePosition.x,
                Input.mousePosition.y,
                -mainCamera.transform.position.z
            )
        );
        aimDirection = ((Vector2)mouseWorldPosition - (Vector2)transform.position).normalized;
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
        throw new NotImplementedException();
    }
}
