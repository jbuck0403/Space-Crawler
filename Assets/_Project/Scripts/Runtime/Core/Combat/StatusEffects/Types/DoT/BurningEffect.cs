using UnityEngine;

public class BurningEffect : BaseDoTEffect
{
    public BurningEffect(StatusEffectData data, GameObject target, DamageData damageData)
        : base(data, target, damageData) { }

    protected override void ApplyInitialEffect() { }

    protected override void ApplyTickEffect()
    {
        float tickDamage = damageData.Amount * CurrentStacks;

        DamageData data = new DamageData(
            tickDamage,
            damageData.Source,
            damageData.CritMultiplier,
            damageData.CritChance,
            damageData.Type
        );

        Debug.Log($"Burning for {data.Amount} damage");
        damageHandler.HandleDamage(data);
    }

    protected override void ApplyFinalEffect() { }
}
