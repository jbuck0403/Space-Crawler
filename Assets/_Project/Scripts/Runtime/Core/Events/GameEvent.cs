using UnityEngine;
using UnityEngine.Events;

public abstract class BaseGameEvent<T> : ScriptableObject
{
    private readonly UnityEvent<T> onEventRaised = new();
    public UnityEvent<T> OnEventRaised => onEventRaised;

    public void Raise(T value)
    {
        onEventRaised.Invoke(value);
    }
}

public struct Void { } // Empty struct for parameterless events
