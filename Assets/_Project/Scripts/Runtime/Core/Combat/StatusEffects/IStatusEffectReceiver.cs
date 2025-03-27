public interface IStatusEffectReceiver
{
    void ApplyStatusEffect(StatusEffectData effectData);
    void RemoveStatusEffect(StatusEffect effect);

    /// <summary>
    /// Checks if the entity has the specified status effect and returns the current number of stacks.
    /// Returns 0 if the effect is not present.
    /// </summary>
    int HasStatusEffect(StatusEffect effect);
    void ClearAllStatusEffects();
}
