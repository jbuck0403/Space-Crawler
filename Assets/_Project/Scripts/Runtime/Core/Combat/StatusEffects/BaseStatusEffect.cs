using UnityEngine;

public abstract class BaseStatusEffect
{
    protected readonly StatusEffectData data;
    protected readonly GameObject target;
    protected float remainingDuration;
    protected float tickTimer;
    protected int currentStacks;

    public bool IsExpired => remainingDuration <= 0;
    public int CurrentStacks => currentStacks;
    public StatusEffectData Data => data;

    protected BaseStatusEffect(StatusEffectData data, GameObject target)
    {
        this.data = data;
        this.target = target;
        this.remainingDuration = data.Duration;
        this.tickTimer = data.TickRate;
        this.currentStacks = 1;
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

    public abstract void OnApply();
    public abstract void OnRemove();
    protected abstract void OnTick();
}
