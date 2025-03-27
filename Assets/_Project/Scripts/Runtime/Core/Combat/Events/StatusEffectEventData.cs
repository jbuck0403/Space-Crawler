using UnityEngine;

/// <summary>
/// Data class containing all relevant information about a status effect for UI and event subscribers.
/// This ensures complete decoupling between status effect processing and UI display.
/// </summary>
public class StatusEffectEventData
{
    public StatusEffectData EffectData { get; private set; }

    public int CurrentStacks { get; private set; }

    public float RemainingDuration { get; private set; }

    public GameObject Target { get; private set; }

    public Transform Source { get; private set; }

    public StatusEffectEventData(
        StatusEffectData effectData,
        int currentStacks,
        float remainingDuration,
        GameObject target,
        Transform source
    )
    {
        EffectData = effectData;
        CurrentStacks = currentStacks;
        RemainingDuration = remainingDuration;
        Target = target;
        Source = source;
    }
}
