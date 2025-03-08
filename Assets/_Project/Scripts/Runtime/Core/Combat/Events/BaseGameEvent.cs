using UnityEngine;
using UnityEngine.Events;

public abstract class BaseGameEvent<T> : ScriptableObject
{
    private readonly UnityEvent<T> onEventRaised = new();
    public UnityEvent<T> OnEventRaised => onEventRaised;

    public void Raise(T value = default)
    {
        onEventRaised.Invoke(value);
    }

    public virtual void AddListener(UnityAction<T> subscriber)
    {
        OnEventRaised.AddListener(subscriber);
    }

    public virtual void RemoveListener(UnityAction<T> action)
    {
        OnEventRaised.RemoveListener(action);
    }
}
