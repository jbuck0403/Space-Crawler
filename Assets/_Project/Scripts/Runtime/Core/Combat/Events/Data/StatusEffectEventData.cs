using UnityEngine;

public class StatusEffectEventData
{
    public StatusEffectData EffectData { get; private set; }
    public string EffectID { get; private set; }

    public int CurrentStacks { get; private set; }

    public float RemainingDuration { get; private set; }

    public GameObject Target { get; private set; }

    public Transform Source { get; private set; }

    public StatusEffectEventData(
        StatusEffectData effectData,
        string effectID,
        int currentStacks,
        float remainingDuration,
        GameObject target,
        Transform source
    )
    {
        EffectData = effectData;
        EffectID = effectID;
        CurrentStacks = currentStacks;
        RemainingDuration = remainingDuration;
        Target = target;
        Source = source;
    }
}
