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
            onStatusEffectTick.Raise(gameObject, effect.GetEventData());

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

    public void ApplyStatusEffect(StatusEffectData effectData)
    {
        print("#StatusEffect# Applying Status Effect");
        var newEffect = StatusEffectFactory.CreateStatusEffect(
            effectData,
            gameObject,
            effectData.Source
        );
        if (newEffect == null)
            return;
        print("#StatusEffect# Applying Status Effect Null Check #1");

        string effectID = newEffect.GetEffectID();

        print($"#StatusEffect# EffectID: {effectID}");

        if (activeEffects.TryGetValue(effectID, out var existingEffect))
        {
            print($"#StatusEffect# {effectID} Stacking...");
            existingEffect.OnStack();
            onStatusEffectApplied.Raise(gameObject, existingEffect.GetEventData());
        }
        else
        {
            print($"#StatusEffect# {effectID} Applying New...");
            activeEffects[effectID] = newEffect;
            newEffect.OnApply();
            onStatusEffectApplied.Raise(gameObject, newEffect.GetEventData());
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
            var eventData = effect.GetEventData();
            effect.OnRemove();
            activeEffects.Remove(effectID);
            onStatusEffectRemoved.Raise(gameObject, eventData);
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
            var eventData = effect.GetEventData();
            effect.OnRemove();
            onStatusEffectRemoved.Raise(gameObject, eventData);
        }
        activeEffects.Clear();
        onAllStatusEffectsRemoved.Raise(gameObject);
    }
}
