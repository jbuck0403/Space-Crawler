using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "VoidEvent", menuName = "Game/Events/Void Event")]
public class VoidEvent : ScriptableObject
{
    private readonly Dictionary<GameObject, UnityEvent> eventsByGameObject = new();

    public void Raise(GameObject sender)
    {
        if (eventsByGameObject.TryGetValue(sender, out var gameObjectEvent))
        {
            gameObjectEvent.Invoke();
        }
    }

    public virtual void AddListener(GameObject gameObject, UnityAction subscriber)
    {
        if (!eventsByGameObject.ContainsKey(gameObject))
        {
            eventsByGameObject[gameObject] = new UnityEvent();
        }
        eventsByGameObject[gameObject].AddListener(subscriber);
    }

    public virtual void RemoveListener(GameObject gameObject, UnityAction action)
    {
        if (eventsByGameObject.TryGetValue(gameObject, out var gameObjectEvent))
        {
            gameObjectEvent.RemoveListener(action);
            if (gameObjectEvent.GetPersistentEventCount() == 0)
            {
                eventsByGameObject.Remove(gameObject);
            }
        }
    }
}

public struct Void { } // empty struct for parameterless events
