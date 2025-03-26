using System.Collections.Generic;
using UnityEngine;

public class StatusEffectHandler : MonoBehaviour, IStatusEffectReceiver
{
    [SerializeField]
    private StatusEffectEvent onStatusEffectApplied;

    [SerializeField]
    private StatusEffectEvent onStatusEffectRemoved;

    [SerializeField]
    private StatusEffectEvent onAllStatusEffectsRemoved;

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
            onStatusEffectTick.Raise(gameObject, effect.Data.EffectType);

            if (effect.IsExpired)
            {
                effectsToRemove.Add(effect.GetEffectID());
            }
        }

        foreach (var effectID in effectsToRemove)
        {
            RemoveStatusEffect(effectID);
        }
    }

    public void ApplyStatusEffect(StatusEffectData effectData, DamageData? damageData = null)
    {
        var newEffect = StatusEffectFactory.CreateStatusEffect(effectData, gameObject, damageData);
        if (newEffect == null)
            return;

        string effectID = newEffect.GetEffectID();

        if (activeEffects.TryGetValue(effectID, out var existingEffect))
        {
            existingEffect.OnStack();
        }
        else
        {
            activeEffects[effectID] = newEffect;
            newEffect.OnApply();
            onStatusEffectApplied.Raise(gameObject, effectData.EffectType);
        }
    }

    public void RemoveStatusEffect(StatusEffect effectType)
    {
        foreach (var effect in activeEffects.Values)
        {
            if (effect.Data.EffectType == effectType)
            {
                RemoveStatusEffect(effect.GetEffectID());
                break;
            }
        }
    }

    public void RemoveStatusEffect(string effectID)
    {
        if (activeEffects.TryGetValue(effectID, out var effect))
        {
            effect.OnRemove();
            activeEffects.Remove(effectID);
            onStatusEffectRemoved.Raise(gameObject, effect.Data.EffectType);
        }
    }

    public int HasStatusEffect(StatusEffect effectType)
    {
        foreach (var effect in activeEffects.Values)
        {
            if (effect.Data.EffectType == effectType)
            {
                return effect.CurrentStacks;
            }
        }
        return 0;
    }

    public int HasStatusEffect(string effectID)
    {
        if (activeEffects.TryGetValue(effectID, out var effect))
        {
            return effect.CurrentStacks;
        }
        return 0;
    }

    public void ClearAllStatusEffects()
    {
        foreach (var effect in activeEffects.Values)
        {
            effect.OnRemove();
        }
        activeEffects.Clear();
        onAllStatusEffectsRemoved.Raise(gameObject);
    }
}
