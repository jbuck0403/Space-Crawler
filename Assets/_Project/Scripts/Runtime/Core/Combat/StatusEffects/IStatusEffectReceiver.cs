public interface IStatusEffectReceiver
{
    void ApplyStatusEffect(StatusEffectData effectData);
    void RemoveStatusEffect(string effectName);
    bool HasStatusEffect(string effectName);
    void ClearAllStatusEffects();
}
