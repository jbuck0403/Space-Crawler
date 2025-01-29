using UnityEngine;

public abstract class BaseStatusConditionEffect : BaseStatusEffect
{
    protected BaseStatusConditionEffect(StatusEffectData data, GameObject target)
        : base(data, target) { }

    public override void OnApply()
    {
        ApplyStatusCondition();
    }

    protected override void OnTick()
    {
        // most status conditions don't need tick behavior
        // override if needed
    }

    public override void OnRemove()
    {
        RemoveStatusCondition();
    }

    protected abstract void ApplyStatusCondition();
    protected abstract void RemoveStatusCondition();
}
