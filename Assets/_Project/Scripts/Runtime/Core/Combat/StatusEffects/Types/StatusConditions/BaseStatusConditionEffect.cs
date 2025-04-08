using UnityEngine;

public abstract class BaseStatusConditionEffect : BaseStatusEffect
{
    protected BaseStatusConditionEffect(StatusEffectData data, GameObject target, Transform source)
        : base(data, target, source) { }

    public override void OnApply()
    {
        ApplyStatusCondition();
    }

    protected override void OnTick() { }

    public override void OnRemove()
    {
        RemoveStatusCondition();
    }

    protected abstract void ApplyStatusCondition();
    protected abstract void RemoveStatusCondition();
}
