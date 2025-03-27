using UnityEngine;

public abstract class BaseDoTEffect : BaseStatusEffect
{
    protected readonly DamageHandler damageHandler;
    protected readonly DoTEffectData dotData;

    protected BaseDoTEffect(DoTEffectData data, GameObject target, Transform source)
        : base(data, target, source)
    {
        dotData = data;
        damageHandler = target.GetComponent<DamageHandler>();
        if (damageHandler == null)
        {
            Debug.LogError(
                $"#StatusEffect# Target {target.name} does not have a DamageHandler component!"
            );
        }
    }

    public override void OnApply()
    {
        Debug.Log("#StatusEffect# DoT Applied");
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
        return new string($"{dotData.DamageType}{data.EffectType}");
    }

    protected DamageData CreateDamageData(float amount)
    {
        return new DamageData(
            amount,
            source,
            dotData.CritMultiplier,
            dotData.CritChance,
            dotData.DamageType
        );
    }

    // template methods for derived classes to implement
    protected abstract void ApplyInitialEffect();
    protected abstract void ApplyTickEffect();
    protected abstract void ApplyFinalEffect();
}
