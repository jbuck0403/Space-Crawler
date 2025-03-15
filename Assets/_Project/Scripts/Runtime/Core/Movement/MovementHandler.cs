using UnityEngine;

public class MovementHandler
{
    public float maxSpeed;
    public float rotationSpeed;
    public float acceleration;
    public float deceleration;

    public MovementConfig config;
    public MovementConfig prevConfig;

    private Vector2 currentVelocity;

    public void Initialize(MovementConfig config)
    {
        maxSpeed = config.maxSpeed;
        acceleration = config.acceleration;
        deceleration = config.deceleration;
        rotationSpeed = config.rotationSpeed;

        prevConfig = this.config;
        this.config = config;
    }

    public MovementHandler(MovementConfig config)
    {
        Initialize(config);
    }

    public Vector2 CalculateMovement(
        Vector2 targetDirection,
        Vector2 currentPosition,
        float deltaTime
    )
    {
        // Accelerate or decelerate based on input
        if (targetDirection != Vector2.zero)
        {
            // Accelerate towards target direction
            currentVelocity = Vector2.MoveTowards(
                currentVelocity,
                targetDirection * maxSpeed,
                acceleration * deltaTime
            );
        }
        else
        {
            // Decelerate when no input
            currentVelocity = Vector2.MoveTowards(
                currentVelocity,
                Vector2.zero,
                deceleration * deltaTime
            );
        }

        return currentPosition + (currentVelocity * deltaTime);
    }

    public void Move(Transform transform, Vector2 targetDirection, float deltaTime)
    {
        Vector2 newPosition = CalculateMovement(targetDirection, transform.position, deltaTime);
        float newRotation = CalculateRotation(
            targetDirection,
            transform.eulerAngles.z,
            rotationSpeed,
            deltaTime
        );

        // transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        ApplyMovementAndRotation(transform, newPosition, newRotation);
    }

    public void ApplyMovement(Transform transform, Vector2 targetDirection, float deltaTime)
    {
        Debug.Log("APPLYMOVEMENT");
        Vector2 newPosition = CalculateMovement(targetDirection, transform.position, deltaTime);

        transform.position = newPosition;
    }

    public void ApplyRotation(Transform transform, Vector2 targetDirection, float deltaTime)
    {
        float rotation = CalculateRotation(
            targetDirection,
            transform.eulerAngles.z,
            rotationSpeed,
            deltaTime
        );
        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    public void ApplyMovementAndRotation(
        Transform transform,
        Vector2 newPosition,
        float newRotation
    )
    {
        transform.SetPositionAndRotation(newPosition, Quaternion.Euler(0, 0, newRotation));
    }

    public float CalculateRotation(
        Vector2 targetDirection,
        float currentRotation,
        float rotationSpeed,
        float deltaTime
    )
    {
        if (targetDirection != Vector2.zero)
        {
            // Calculate target angle in degrees
            float targetAngle =
                Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;

            // Smoothly rotate towards target angle
            return Mathf.LerpAngle(currentRotation, targetAngle, rotationSpeed * deltaTime);
        }
        return currentRotation;
    }

    public void ResetVelocity()
    {
        currentVelocity = Vector2.zero;
    }

    public Vector2 GetCurrentVelocity()
    {
        return currentVelocity;
    }

    public void ApplyVelocity(Vector2 velocity)
    {
        currentVelocity = velocity;
    }
}
