using System;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : IModifiable
{
    // Delegate type for modifying movement config
    public delegate MovementConfig MovementConfigModifier(MovementConfig config);

    // List of active movement config modifiers
    private List<MovementConfigModifier> movementModifiers = new List<MovementConfigModifier>();

    // Single instance of modified config that gets updated when modifiers change
    private MovementConfig modifiedConfig;

    // Global movement multiplier that affects all movement calculations
    private float movementMultiplier = 1f;

    public float maxSpeed;
    public float rotationSpeed;
    public float acceleration;
    public float deceleration;

    public MovementConfig config;
    public MovementConfig prevConfig;

    private Vector2 currentVelocity;

    public Dictionary<ModifierType, List<(object Source, Delegate Modifier)>> modifiers =
        new Dictionary<ModifierType, List<(object Source, Delegate Modifier)>>();
    public Dictionary<ModifierType, List<(object Source, Delegate Modifier)>> Modifiers =>
        modifiers;

    // Property to get/set the movement multiplier
    public float MovementMultiplier
    {
        get { return movementMultiplier; }
        set { movementMultiplier = Mathf.Max(value, 0.25f); } // Prevent complete immobilization
    }

    public void Initialize(MovementConfig config)
    {
        maxSpeed = config.maxSpeed;
        acceleration = config.acceleration;
        deceleration = config.deceleration;
        rotationSpeed = config.rotationSpeed;

        prevConfig = this.config;
        this.config = config;

        modifiedConfig = ScriptableObject.CreateInstance<MovementConfig>();
        UpdateModifiedConfig();
    }

    public MovementHandler(MovementConfig config)
    {
        Initialize(config);
    }

    public virtual Vector2 CalculateMovement(
        Vector2 targetDirection,
        Vector2 currentPosition,
        float deltaTime
    )
    {
        // Apply movement multiplier modifiers at the beginning
        float calculatedMultiplier = CalculateMovementMultiplier();

        // TBI Skill Point Delegate: BEFORE_MOVEMENT_CALCULATION
        // Get the modified config for this frame
        MovementConfig modifiedConfig = GetModifiedConfig();

        // Accelerate or decelerate based on input
        if (targetDirection != Vector2.zero)
        {
            // TBI Skill Point Delegate: MOVEMENT_ACCELERATION_MODIFIER
            // Accelerate towards target direction
            currentVelocity = Vector2.MoveTowards(
                currentVelocity,
                targetDirection * modifiedConfig.maxSpeed * calculatedMultiplier,
                modifiedConfig.acceleration * deltaTime * calculatedMultiplier
            );
        }
        else
        {
            // TBI Skill Point Delegate: MOVEMENT_DECELERATION_MODIFIER
            // Decelerate when no input
            currentVelocity = Vector2.MoveTowards(
                currentVelocity,
                Vector2.zero,
                modifiedConfig.deceleration * deltaTime
            );
        }

        // foreach (
        //     var modifier in ModifierHelper.GetModifiers<ModifierHelper.Vector2InVector2Out>(
        //         this,
        //         ModifierType.AFTER_MOVEMENT_CALCULATION
        //     )
        // )
        // {
        //     currentVelocity = modifier(currentVelocity);
        // }

        // TBI Skill Point Delegate: AFTER_MOVEMENT_CALCULATION
        return currentPosition + (currentVelocity * deltaTime);
    }

    public void Move(Transform transform, Vector2 targetDirection, float deltaTime)
    {
        Vector2 newPosition = CalculateMovement(targetDirection, transform.position, deltaTime);
        float newRotation = CalculateRotation(
            targetDirection,
            transform.eulerAngles.z,
            GetModifiedConfig().rotationSpeed,
            deltaTime
        );

        ApplyMovementAndRotation(transform, newPosition, newRotation);
    }

    public void ApplyMovement(Transform transform, Vector2 targetDirection, float deltaTime)
    {
        Vector2 newPosition = CalculateMovement(targetDirection, transform.position, deltaTime);
        transform.position = newPosition;
    }

    public void ApplyRotation(Transform transform, Vector2 targetDirection, float deltaTime)
    {
        float rotation = CalculateRotation(
            targetDirection,
            transform.eulerAngles.z,
            GetModifiedConfig().rotationSpeed,
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

    public virtual float CalculateRotation(
        Vector2 targetDirection,
        float currentRotation,
        float rotationSpeed,
        float deltaTime
    )
    {
        if (targetDirection != Vector2.zero)
        {
            // Apply movement multiplier modifiers
            float calculatedMultiplier = CalculateMovementMultiplier();

            // TBI Skill Point Delegate: BEFORE_ROTATION_CALCULATION
            // Calculate target angle in degrees
            float targetAngle =
                Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;

            // Smoothly rotate towards target angle
            // TBI Skill Point Delegate: ROTATION_SPEED_MODIFIER
            // Apply the movement multiplier to rotation speed as well
            return Mathf.LerpAngle(
                currentRotation,
                targetAngle,
                rotationSpeed * deltaTime * calculatedMultiplier
            );
        }
        return currentRotation;
    }

    // Calculate the current movement multiplier by applying all modifiers
    private float CalculateMovementMultiplier()
    {
        // Start with the base value
        float result = movementMultiplier;

        // Apply all MOVEMENT_MULTIPLIER modifiers
        foreach (
            var modifier in ModifierHelper.GetModifiers<ModifierHelper.FloatInFloatOutModifier>(
                this,
                ModifierType.MOVEMENT_MULTIPLIER
            )
        )
        {
            result = modifier(result);
        }

        // Ensure we don't go below minimum
        return Mathf.Max(result, 0.25f);
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

    public void AddToVelocity(Vector2 velocity)
    {
        currentVelocity += velocity;
    }

    public MovementConfig GetMovementConfig()
    {
        return config;
    }

    public MovementConfig SetMovementConfig(MovementConfig newConfig)
    {
        prevConfig = config;
        config = newConfig;

        return config;
    }

    public MovementConfig RevertToPrevConfig()
    {
        MovementConfig currentConfig = config;
        config = prevConfig;
        prevConfig = currentConfig;

        return config;
    }

    public void AddMovementModifier(MovementConfigModifier modifier)
    {
        // TBI Skill Point Delegate: REGISTER_MOVEMENT_MODIFIER
        if (modifier != null)
        {
            movementModifiers.Add(modifier);
            UpdateModifiedConfig();
        }
    }

    public void RemoveMovementModifier(MovementConfigModifier modifier)
    {
        // TBI Skill Point Delegate: UNREGISTER_MOVEMENT_MODIFIER
        if (modifier != null)
        {
            movementModifiers.Remove(modifier);
            UpdateModifiedConfig();
        }
    }

    public void UpdateMovementModifier(
        MovementConfigModifier oldModifier,
        MovementConfigModifier newModifier
    )
    {
        if (oldModifier != null && newModifier != null)
        {
            int index = movementModifiers.IndexOf(oldModifier);
            if (index != -1)
            {
                movementModifiers[index] = newModifier;
                UpdateModifiedConfig();
            }
        }
    }

    private void UpdateModifiedConfig()
    {
        // create a new instance starting with base config values
        MovementConfig newConfig = ScriptableObject.CreateInstance<MovementConfig>();
        newConfig.maxSpeed = config.maxSpeed;
        newConfig.acceleration = config.acceleration;
        newConfig.deceleration = config.deceleration;
        newConfig.rotationSpeed = config.rotationSpeed;

        // Apply each modifier to the new instance
        foreach (var modifier in movementModifiers)
        {
            newConfig = modifier(newConfig);
        }

        // Replace the class-wide instance with our new one
        modifiedConfig = newConfig;
    }

    // Get the current modified config
    public MovementConfig GetModifiedConfig()
    {
        return modifiedConfig;
    }
}
