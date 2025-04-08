using UnityEngine;

public class FreezingEffect : BaseStatusConditionEffect
{
    private MovementHandler movementHandler;
    private MovementHandler.MovementConfigModifier freezeModifier;

    public FreezingEffect(StatusEffectData data, GameObject target, Transform source)
        : base(data, target, source)
    {
        // Get the MovementHandler from the BaseCharacterController
        BaseCharacterController characterController =
            target.GetComponent<BaseCharacterController>();
        if (characterController != null)
        {
            movementHandler = characterController.GetMovementHandler();

            // Create the modifier delegate
            freezeModifier = (config) =>
            {
                MovementConfig modifiedConfig = ScriptableObject.CreateInstance<MovementConfig>();
                modifiedConfig.maxSpeed = config.maxSpeed;
                modifiedConfig.acceleration = config.acceleration;
                modifiedConfig.deceleration = config.deceleration;
                modifiedConfig.rotationSpeed = config.rotationSpeed;

                // Apply freeze effect based on current stacks
                float stackRatio = (float)currentStacks / data.MaxStacks;
                modifiedConfig.acceleration *= (1 - stackRatio);
                modifiedConfig.deceleration *= (1 + stackRatio);
                modifiedConfig.maxSpeed *= (1 - stackRatio);
                modifiedConfig.rotationSpeed *= (1 - stackRatio);

                return modifiedConfig;
            };
        }
        else
        {
            Debug.LogWarning(
                $"FreezingEffect: Target {target.name} doesn't have a BaseCharacterController component."
            );
        }
    }

    protected override void ApplyStatusCondition()
    {
        if (movementHandler != null)
        {
            movementHandler.AddMovementModifier(freezeModifier);
        }
    }

    protected override void RemoveStatusCondition()
    {
        if (movementHandler != null)
        {
            movementHandler.RemoveMovementModifier(freezeModifier);
        }
    }

    public override void OnStack()
    {
        base.OnStack();
        if (movementHandler != null)
        {
            // Create a new modifier with updated stack count
            MovementHandler.MovementConfigModifier newModifier = (config) =>
            {
                MovementConfig modifiedConfig = ScriptableObject.CreateInstance<MovementConfig>();
                modifiedConfig.maxSpeed = config.maxSpeed;
                modifiedConfig.acceleration = config.acceleration;
                modifiedConfig.deceleration = config.deceleration;
                modifiedConfig.rotationSpeed = config.rotationSpeed;

                // Apply freeze effect based on current stacks
                float stackRatio = (float)currentStacks / data.MaxStacks;
                modifiedConfig.acceleration *= (1 - stackRatio);
                modifiedConfig.deceleration *= (1 + stackRatio);
                modifiedConfig.maxSpeed *= (1 - stackRatio);
                modifiedConfig.rotationSpeed *= (1 - stackRatio);

                return modifiedConfig;
            };

            // Update the modifier
            movementHandler.UpdateMovementModifier(freezeModifier, newModifier);
            freezeModifier = newModifier;
        }
    }
}
