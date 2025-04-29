using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Room Setup")]
    [SerializeField]
    private Transform entrancePoint;

    [SerializeField]
    private DoorData entranceDoor;

    [SerializeField]
    private List<DoorData> exitDoors;

    [Header("Snap Points")]
    [SerializeField]
    private Transform[] snapPoints; // Array of exit snap points

    [Header("Enemy Spawn Locations")]
    [SerializeField]
    public Transform[] enemySpawnLocations;
    public int numEnemiesToSpawn = 1;
    public int maxEnemiesToSpawn = 8;
    public int enemyPopulationIncrement = 1;

    public List<BaseEnemyController> spawnedEnemies = new List<BaseEnemyController>();

    private Dictionary<Transform, bool> spawnLocationUsed = new Dictionary<Transform, bool>();

    public DoorData EntranceDoor => entranceDoor;
    public List<DoorData> ExitDoors => exitDoors;

    private void OnValidate()
    {
        numEnemiesToSpawn = Mathf.Clamp(numEnemiesToSpawn, 0, maxEnemiesToSpawn);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (entranceDoor != null && entranceDoor.door != null)
                entranceDoor.door.CloseDoor();
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

        GameObject enemy = InstantiateEnemy(enemyToAdd, spawnLocation);
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

    public void RemoveEnemyFromSpawnedList(BaseEnemyController baseEnemyController)
    {
        spawnedEnemies.Remove(baseEnemyController);
    }

    public int GetTotalSpawnLocations()
    {
        return enemySpawnLocations.Length;
    }

    public Transform[] GetSpawnLocations()
    {
        return enemySpawnLocations;
    }

    public GameObject InstantiateEnemy(GameObject prefab, Transform spawnLocation)
    {
        return Instantiate(prefab, spawnLocation);
    }

    public void OpenDoorsWithRoomBeyond()
    {
        Debug.Log(
            $"#ROOM Room.OpenDoorsWithRoomBeyond called on {gameObject.name} - {System.DateTime.Now.ToString("HH:mm:ss.fff")}"
        );
        Debug.Log($"#ROOM Exit doors count: {exitDoors.Count}");

        foreach (DoorData doorData in exitDoors)
        {
            if (doorData == null)
            {
                Debug.LogError("#ROOM DoorData is null in exitDoors list");
                continue;
            }

            Debug.Log(
                $"#ROOM Checking door on {doorData.doorSide} side, hasRoomBeyond: {doorData.hasRoomBeyond}"
            );

            if (doorData.door == null)
            {
                Debug.LogError($"#ROOM Door handler is null for {doorData.doorSide} door");
                continue;
            }

            if (doorData.hasRoomBeyond)
            {
                Debug.Log($"#ROOM Opening door on {doorData.doorSide} side - it has a room beyond");
                doorData.door.OpenDoor();
            }
            else
            {
                Debug.Log($"#ROOM Not opening door on {doorData.doorSide} side - no room beyond");
            }
        }
    }
}

[Serializable]
public enum DoorSide
{
    North,
    South,
    East,
    West
}

[Serializable]
public class DoorData
{
    public DoorHandler door;
    public GameObject doorObject;
    public DoorSide doorSide;
    public Transform roomBeyondSnapPoint;
    public bool hasRoomBeyond;

    public DoorData(
        DoorHandler door,
        GameObject doorObject,
        Transform roomBeyondSnapPoint,
        DoorSide doorSide
    )
    {
        this.door = door;
        this.doorObject = doorObject;
        this.doorSide = doorSide;
        this.roomBeyondSnapPoint = roomBeyondSnapPoint;
        hasRoomBeyond = false;
    }
}
