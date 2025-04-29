using UnityEngine;

public abstract class CollectibleDropSO : ScriptableObject
{
    [SerializeField]
    protected GameObject collectiblePrefab;

    public abstract void HandleCollection();

    public GameObject GetCollectiblePrefab()
    {
        return collectiblePrefab;
    }

    public GameObject SpawnCollectible(Transform location)
    {
        GameObject collectible = Instantiate(
            collectiblePrefab,
            location.position,
            Quaternion.identity
        );

        return collectible;
    }
}
