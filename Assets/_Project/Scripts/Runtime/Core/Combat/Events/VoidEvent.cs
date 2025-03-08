using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "VoidEvent", menuName = "Game/Events/Void Event")]
public class VoidEvent : BaseGameEvent<Void>
{
    private readonly Dictionary<UnityAction, UnityAction<Void>> wrapperDictionary =
        new Dictionary<UnityAction, UnityAction<Void>>();

    public virtual void AddListener(UnityAction action)
    {
        if (wrapperDictionary.ContainsKey(action))
        {
            RemoveListener(action);
        }

        UnityAction<Void> wrapper = _ => action();
        wrapperDictionary[action] = wrapper;

        OnEventRaised.AddListener(wrapper);
    }

    public virtual void RemoveListener(UnityAction action)
    {
        if (wrapperDictionary.TryGetValue(action, out UnityAction<Void> wrapper))
        {
            OnEventRaised.RemoveListener(wrapper);
            wrapperDictionary.Remove(action);
        }
    }
}

public struct Void { } // empty struct for parameterless events
