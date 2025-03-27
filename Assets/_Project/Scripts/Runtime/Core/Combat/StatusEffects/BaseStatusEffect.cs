using UnityEngine;

public abstract class BaseStatusEffect
{
    protected readonly StatusEffectData data;
    protected readonly GameObject target;
    protected readonly Transform source;
    protected float remainingDuration;
    protected float tickTimer;
    protected int currentStacks;

    public bool IsExpired => remainingDuration <= 0;
    public int CurrentStacks => currentStacks;
    public StatusEffectData Data => data;

    protected BaseStatusEffect(StatusEffectData data, GameObject target, Transform source)
    {
        data.SetSource(source);
        this.source = source;

        this.data = data;
        this.target = target;
        remainingDuration = data.Duration;
        tickTimer = data.TickRate;
        currentStacks = 1;
    }

    public virtual string GetEffectID()
    {
        return new string($"{data.EffectType}");
    }

    public virtual void OnStack()
    {
        if (data.IsStackable && currentStacks < data.MaxStacks)
        {
            currentStacks++;
            remainingDuration = data.Duration; // refresh duration when stacking
        }
        else if (currentStacks == data.MaxStacks)
        {
            remainingDuration = data.Duration;
        }
    }

    public virtual void Update(float deltaTime)
    {
        remainingDuration -= deltaTime;

        if (data.TickRate > 0)
        {
            tickTimer -= deltaTime;
            if (tickTimer <= 0)
            {
                OnTick();
                tickTimer = data.TickRate;
            }
        }
    }

    /// <summary>
    /// Creates a StatusEffectEventData object containing all information needed for UI and other subscribers.
    /// </summary>
    /// <returns>A complete StatusEffectEventData object</returns>
    public StatusEffectEventData GetEventData()
    {
        return new StatusEffectEventData(data, currentStacks, remainingDuration, target, source);
    }

    public abstract void OnApply();
    public abstract void OnRemove();
    protected abstract void OnTick();
}
