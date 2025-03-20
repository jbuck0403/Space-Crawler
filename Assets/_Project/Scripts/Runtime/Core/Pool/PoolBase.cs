using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolBase : MonoBehaviour
{
    [SerializeField]
    GameObject prefab;

    [SerializeField]
    protected int numObjects;

    [SerializeField]
    protected float DespawnTime = 5f;

    [SerializeField]
    protected bool autoExpand = true;

    [SerializeField]
    protected int maxPoolSize = 100;

    [SerializeField]
    protected int incrementSize = 2;

    protected Queue<GameObject> pool = new Queue<GameObject>();

    protected bool initialized = false;

    [SerializeField]
    protected int currentNumObjects = 0;

    protected virtual void Start()
    {
        PopulateObjects();
        initialized = true;
    }

    protected virtual void OnObjectSpawned(GameObject obj) { }

    protected virtual void OnObjectDespawned() { }

    public GameObject GetObject()
    {
        GameObject obj = pool.Dequeue();
        obj.SetActive(true);
        StartCoroutine(DelayedReturnToPool(obj));

        OnObjectSpawned(obj);

        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        OnObjectDespawned();

        obj.transform.position = Vector3.zero;
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    private void PopulateObjects()
    {
        IncreasePoolSize(numObjects);
    }

    protected void IncreasePoolSize(int numToAdd)
    {
        initialized = false;

        for (int i = 0; i < numToAdd; i++)
        {
            AddObjectToPool();
            currentNumObjects++;
        }

        initialized = true;
    }

    protected virtual GameObject AddObjectToPool()
    {
        GameObject obj = InstantiatePrefab();
        obj.SetActive(false);
        pool.Enqueue(obj);

        return obj;
    }

    private GameObject InstantiatePrefab(GameObject prefabOverride = null)
    {
        GameObject prefabObj = Instantiate(
            prefabOverride != null ? prefabOverride : prefab,
            transform
        );

        return prefabObj;
    }

    // returns obj back to the pool after a predefined delay; can override delay with optional arg
    private IEnumerator DelayedReturnToPool(GameObject obj, float delayOverride = 0f)
    {
        float delay = delayOverride == 0 ? DespawnTime : delayOverride;
        if (DespawnTime > 0f || delayOverride > 0f)
        {
            yield return new WaitForSeconds(delay);

            ReturnToPool(obj);
        }
    }
}
