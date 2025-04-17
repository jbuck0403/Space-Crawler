using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Room Setup")]
    [SerializeField]
    private Transform entrancePoint;

    [Header("Snap Points")]
    [SerializeField]
    private Transform[] snapPoints; // Array of exit snap points

    [Header("Enemy Spawn Locations")]
    [SerializeField]
    Transform[] enemySpawnLocations;

    public List<BaseEnemyController> spawnedEnemies = new List<BaseEnemyController>();

    private Dictionary<Transform, bool> spawnLocationUsed = new Dictionary<Transform, bool>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Notify RoomManager that player entered this room
            RoomManager.Instance.OnPlayerEnteredRoom(gameObject);
        }
    }

    // Get entrance point
    public Transform GetEntrancePoint()
    {
        return entrancePoint;
    }

    // Get all snap points
    public Transform[] GetSnapPoints()
    {
        return snapPoints;
    }

    public void InitializeSpawnedEnemies(Transform target)
    {
        foreach (BaseEnemyController enemy in spawnedEnemies)
        {
            enemy.Initialize(target);
        }
    }

    public void AddEnemyToSpawnedEnemies(GameObject enemyToAdd, Transform spawnLocation = null)
    {
        if (spawnLocation == null)
        {
            spawnLocation = GetUnusedSpawnLocation();

            if (spawnLocation == null)
            {
                Debug.Log("No Appropriate Spawn Location Found");
                return;
            }
        }

        GameObject enemy = IntsantiateEnemy(enemyToAdd, spawnLocation);
        BaseEnemyController enemyController = enemy.GetComponent<BaseEnemyController>();
        if (enemyController != null)
        {
            spawnedEnemies.Add(enemyController);
        }
    }

    public Transform GetUnusedSpawnLocation()
    {
        foreach (Transform spawnLocation in enemySpawnLocations)
        {
            if (!spawnLocationUsed.ContainsKey(spawnLocation) || !spawnLocationUsed[spawnLocation])
            {
                spawnLocationUsed[spawnLocation] = true;
                return spawnLocation;
            }
        }

        return null;
    }

    public int GetTotalSpawnLocations()
    {
        return enemySpawnLocations.Length;
    }

    public Transform[] GetSpawnLocations()
    {
        return enemySpawnLocations;
    }

    public GameObject IntsantiateEnemy(GameObject prefab, Transform spawnLocation)
    {
        return Instantiate(prefab, spawnLocation);
    }
}
