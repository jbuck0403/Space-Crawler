using UnityEngine;

public class FreezingEffect : BaseStatusConditionEffect
{
    private MovementHandler movementHandler;
    private const float SPEED_REDUCTION_PER_STACK = 0.1f; // 10% per stack

    public FreezingEffect(StatusEffectData data, GameObject target, Transform source)
        : base(data, target, source)
    {
        // Get the BaseCharacterController
        BaseCharacterController baseCharacterController =
            target.GetComponent<BaseCharacterController>();

        movementHandler = baseCharacterController.GetMovementHandler();
        if (movementHandler == null)
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
            // Apply freezing modifier to movement multiplier
            ApplyFreezeModifier();
        }
    }

    protected override void RemoveStatusCondition()
    {
        if (movementHandler != null)
        {
            // Remove the freezing modifier
            ModifierHelper.RemoveModifiersFromSource(movementHandler, this);
        }
    }

    public override void OnStack()
    {
        base.OnStack();
        if (movementHandler != null)
        {
            // update freeze modifier with new stack count
            ModifierHelper.RemoveModifiersFromSource(movementHandler, this);
            ApplyFreezeModifier();
        }
    }

    private void ApplyFreezeModifier()
    {
        // create a movement multiplier modifier that reduces speed by 10% per stack (max 50%)
        ModifierHelper.FloatInFloatOutModifier freezeModifier = (float baseMultiplier) =>
        {
            float reduction = SPEED_REDUCTION_PER_STACK * currentStacks;

            reduction = Mathf.Min(reduction, 0.5f);

            float finalMultiplier = baseMultiplier * (1f - reduction);

            Debug.Log(
                $"FreezingEffect: Applied with {currentStacks} stacks, reduction = {reduction}, final multiplier = {finalMultiplier}"
            );

            return finalMultiplier;
        };

        ModifierHelper.AddModifier(
            movementHandler,
            ModifierType.MOVEMENT_MULTIPLIER,
            this,
            freezeModifier
        );
    }
}
