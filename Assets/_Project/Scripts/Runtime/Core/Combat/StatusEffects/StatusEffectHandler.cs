using System.Collections.Generic;
using UnityEngine;

public class StatusEffectHandler : MonoBehaviour, IStatusEffectReceiver
{
    [SerializeField]
    private StatusEffectEvent onStatusEffectApplied;

    [SerializeField]
    private StatusEffectEvent onStatusEffectRemoved;

    [SerializeField]
    private StatusEffectEvent onStatusEffectTick;

    private Dictionary<string, BaseStatusEffect> activeEffects =
        new Dictionary<string, BaseStatusEffect>();

    private void Update()
    {
        List<string> effectsToRemove = new List<string>();

        foreach (var effect in activeEffects.Values)
        {
            effect.Update(Time.deltaTime);
            onStatusEffectTick.Raise(effect.Data.EffectType);

            if (effect.IsExpired)
            {
                effectsToRemove.Add(effect.Data.EffectName);
            }
        }

        foreach (var effectName in effectsToRemove)
        {
            RemoveStatusEffect(effectName);
        }
    }

    public void ApplyStatusEffect(StatusEffectData effectData)
    {
        if (activeEffects.TryGetValue(effectData.EffectName, out var existingEffect))
        {
            existingEffect.OnStack();
        }
        else
        {
            var newEffect = StatusEffectFactory.CreateStatusEffect(effectData, gameObject);
            if (newEffect != null)
            {
                activeEffects[effectData.EffectName] = newEffect;
                newEffect.OnApply();
                onStatusEffectApplied.Raise(effectData.EffectType);
            }
        }
    }

    public void RemoveStatusEffect(string effectName)
    {
        if (activeEffects.TryGetValue(effectName, out var effect))
        {
            effect.OnRemove();
            activeEffects.Remove(effectName);
            onStatusEffectRemoved.Raise(effect.Data.EffectType);
        }
    }

    public bool HasStatusEffect(string effectName)
    {
        return activeEffects.ContainsKey(effectName);
    }

    public void ClearAllStatusEffects()
    {
        foreach (var effect in activeEffects.Values)
        {
            effect.OnRemove();
        }
        activeEffects.Clear();
        onStatusEffectRemoved.Raise();
    }
}
