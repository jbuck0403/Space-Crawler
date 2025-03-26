using UnityEngine;

public abstract class BaseDoTEffect : BaseStatusEffect
{
    protected readonly DamageHandler damageHandler;
    protected readonly DamageData damageData;

    protected BaseDoTEffect(StatusEffectData data, GameObject target, DamageData damageData)
        : base(data, target)
    {
        damageHandler = target.GetComponent<DamageHandler>();
        if (damageHandler == null)
        {
            Debug.LogError($"Target {target.name} does not have a DamageHandler component!");
        }
        this.damageData = damageData;
    }

    public override void OnApply()
    {
        if (damageHandler == null)
            return;
        ApplyInitialEffect();
    }

    protected override void OnTick()
    {
        if (damageHandler == null)
            return;
        ApplyTickEffect();
    }

    public override void OnRemove()
    {
        if (damageHandler == null)
            return;
        ApplyFinalEffect();
    }

    public override string GetEffectID()
    {
        return new string($"{damageData.Type}{data.EffectType}");
    }

    // template methods for derived classes to implement
    protected abstract void ApplyInitialEffect();
    protected abstract void ApplyTickEffect();
    protected abstract void ApplyFinalEffect();
}
