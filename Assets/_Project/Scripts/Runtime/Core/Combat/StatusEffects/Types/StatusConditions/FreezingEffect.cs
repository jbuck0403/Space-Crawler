using UnityEngine;

public abstract class FreezingEffect : BaseStatusEffect
{
    private MovementHandler movementHandler;
    private MovementHandler.MovementConfigModifier freezeModifier;

    protected FreezingEffect(StatusEffectData data, GameObject target, Transform source)
        : base(data, target, source)
    {
        movementHandler = target.GetComponent<MovementHandler>();
        if (movementHandler != null)
        {
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
    }

    protected override void OnApply()
    {
        base.OnApply();
        if (movementHandler != null)
        {
            movementHandler.AddMovementModifier(freezeModifier);
        }
    }

    protected override void OnRemove()
    {
        if (movementHandler != null)
        {
            movementHandler.RemoveMovementModifier(freezeModifier);
        }
        base.OnRemove();
    }

    protected override void OnStackChanged(int newStackCount)
    {
        // No need to do anything here - the modifier will automatically use the new stack count
        base.OnStackChanged(newStackCount);
    }
}
