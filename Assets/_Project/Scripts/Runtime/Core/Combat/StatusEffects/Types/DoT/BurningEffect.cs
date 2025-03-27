using UnityEngine;

public class BurningEffect : BaseDoTEffect
{
    public BurningEffect(DoTEffectData data, GameObject target, Transform source)
        : base(data, target, source) { }

    protected override void ApplyInitialEffect() { }

    protected override void ApplyTickEffect()
    {
        float tickDamage = dotData.BaseDamage * CurrentStacks;
        var data = CreateDamageData(tickDamage);

        Debug.Log($"#StatusEffect# Burning for {data.Amount} damage pre-mitigation");
        damageHandler.HandleDamage(data);
    }

    protected override void ApplyFinalEffect() { }
}
