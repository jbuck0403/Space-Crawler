using UnityEngine;

public class MovementHandler
{
    private readonly float maxSpeed;
    private readonly float acceleration;
    private readonly float deceleration;

    private Vector2 currentVelocity;

    public MovementHandler(MovementConfig config)
    {
        this.maxSpeed = config.maxSpeed;
        this.acceleration = config.acceleration;
        this.deceleration = config.deceleration;
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
}
