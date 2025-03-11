using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : BaseCharacterController
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
        weapon = GetComponent<BaseWeapon>();

        // Set a firing strategy for the weapon
        if (weapon != null && weapon.weaponConfig != null)
        {
            // Use a firing strategy from the weapon config if available
            if (
                weapon.weaponConfig.firingStrategies != null
                && weapon.weaponConfig.firingStrategies.Count > 0
            )
            {
                weapon.SetStrategy(weapon.weaponConfig.firingStrategies[0]);
            }
        }
    }

    private void Update()
    {
        GetMoveInput();
        GetAimDirection();
        MoveAndFaceTarget();

        if (Input.GetMouseButton(0))
        {
            weapon.SetCanFire(true);
        }
        else
        {
            weapon.SetCanFire(false);
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
}
