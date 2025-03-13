using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseGameEvent<T> : ScriptableObject
{
    private readonly Dictionary<GameObject, UnityEvent<T>> eventsByGameObject = new();

    public void Raise(GameObject sender, T value = default)
    {
        if (eventsByGameObject.TryGetValue(sender, out var gameObjectEvent))
        {
            gameObjectEvent.Invoke(value);
        }
    }

    public virtual void AddListener(GameObject gameObject, UnityAction<T> subscriber)
    {
        if (!eventsByGameObject.ContainsKey(gameObject))
        {
            eventsByGameObject[gameObject] = new UnityEvent<T>();
        }
        eventsByGameObject[gameObject].AddListener(subscriber);
    }

    public virtual void RemoveListener(GameObject gameObject, UnityAction<T> action)
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
