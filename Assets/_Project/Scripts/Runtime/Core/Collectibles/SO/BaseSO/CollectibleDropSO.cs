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

    public GameObject SpawnCollectible(Transform location, GameObject defaultVFXPrefab)
    {
        GameObject collectible = Instantiate(
            collectiblePrefab != null ? collectiblePrefab : defaultVFXPrefab,
            location.position,
            Quaternion.identity
        );

        return collectible;
    }
}
