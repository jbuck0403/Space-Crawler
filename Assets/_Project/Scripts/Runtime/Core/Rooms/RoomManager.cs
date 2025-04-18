using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [Header("Room Prefabs")]
    [SerializeField]
    private GameObject normalRoomPrefab;

    [SerializeField]
    private GameObject eliteRoomPrefab;

    [SerializeField]
    private GameObject treasureRoomPrefab;

    [SerializeField]
    private GameObject bossRoomPrefab;

    [Header("Spawn Chances")]
    [SerializeField]
    private float eliteRoomChance = 0.5f;

    [SerializeField]
    private float treasureRoomChance = 0.2f;

    [SerializeField]
    private int roomsBeforeBoss = 10;

    // Currently active room
    private GameObject currentRoom;
    private Room currentRoomComponent;

    // Dictionary of all rooms: GameObject -> bool (has been entered)
    private Dictionary<GameObject, bool> allRooms = new Dictionary<GameObject, bool>();
    private int roomsCompleted = 0;

    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private Transform defaultTarget;

    [SerializeField]
    private GameObject startingRoomPrefab;

    [SerializeField]
    private GameObject[] defaultEnemyPrefabs;

    [SerializeField]
    private GameObject[] eliteEnemyPrefabs;

    [SerializeField]
    private GameObject[] bossEnemyPrefabs;

    private GameObject player;
    public GameObject Player => player;
    public GameObject CurrentRoom => currentRoom;
    public Room CurrentRoomComponent => currentRoomComponent;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool Initialize()
    {
        // Spawn initial room
        currentRoom = RoomHandler.CreateRoom(
            startingRoomPrefab,
            null,
            default,
            Quaternion.identity
        );

        Room room = currentRoom.GetComponent<Room>();
        if (room != null)
        {
            currentRoomComponent = room;
            InitPlayer(currentRoomComponent);

            allRooms.Add(currentRoom, false); // Initialize as not entered yet

            return true;
        }

        return false;
    }

    private void InitPlayer(Room room)
    {
        player = Instantiate(
            playerPrefab,
            room.GetEntrancePoint().position,
            room.GetEntrancePoint().rotation
        );
        if (player != null)
        {
            GameManager.Instance.VirtualCamera.Follow = player.transform;
            GameManager.Instance.VirtualCamera.LookAt = player.transform;
        }
    }

    // Called by RoomCollider when player enters a room
    public void OnPlayerEnteredRoom(GameObject room) // initialize spawned enemies
    {
        // Check if we have this room in our dictionary
        if (!allRooms.ContainsKey(room))
        {
            Debug.LogWarning("Player entered an unknown room");
            return;
        }

        // Check if the room has been entered before
        if (allRooms[room])
        {
            Debug.Log("Room has been entered before, not respawning");
            return;
        }

        // Mark the room as entered
        allRooms[room] = true;

        // Set this as the current room
        currentRoom = room;

        Room roomComponent = currentRoom.GetComponent<Room>();
        if (roomComponent != null)
        {
            currentRoomComponent = roomComponent;
            roomComponent.InitializeSpawnedEnemies(Player.transform);
        }

        // Destroy all other rooms
        DestroyAllRoomsExcept(currentRoom);

        // Increment counter (this is a new room)
        roomsCompleted++;

        // Spawn new connecting rooms
        SpawnConnectingRooms();
    }

    // Spawns rooms with chance-based selection
    private void SpawnConnectingRooms() // instantiates enemies but doesn't initialize
    {
        Room roomCollider = currentRoom.GetComponent<Room>();
        if (roomCollider == null)
            return;

        Transform[] snapPoints = roomCollider.GetSnapPoints();
        if (snapPoints == null || snapPoints.Length == 0)
            return;

        // First snap point always gets a normal room or boss room (based on progression)
        if (snapPoints.Length > 0)
        {
            if (roomsCompleted >= roomsBeforeBoss && bossRoomPrefab != null)
            {
                SpawnRoomAtSnapPoint(snapPoints[0], bossRoomPrefab, RoomType.Boss);
                Debug.Log("Boss room spawned!");
            }
            else
            {
                SpawnRoomAtSnapPoint(snapPoints[0], normalRoomPrefab, RoomType.Default);
            }
        }

        // Second snap point has a chance for elite room
        if (snapPoints.Length > 1 && Random.value < eliteRoomChance)
        {
            GameObject roomPrefab = eliteRoomPrefab != null ? eliteRoomPrefab : normalRoomPrefab;
            SpawnRoomAtSnapPoint(snapPoints[1], roomPrefab, RoomType.Elite);
        }

        // Third snap point has a chance for treasure room
        if (snapPoints.Length > 2 && Random.value < treasureRoomChance)
        {
            GameObject roomPrefab =
                treasureRoomPrefab != null ? treasureRoomPrefab : normalRoomPrefab;
            SpawnRoomAtSnapPoint(snapPoints[2], roomPrefab, RoomType.Treasure);
        }
    }

    // Spawn a specific room prefab at a snap point
    private void SpawnRoomAtSnapPoint(Transform snapPoint, GameObject roomPrefab, RoomType roomType)
    {
        if (snapPoint == null || roomPrefab == null)
            return;

        Vector3 offset = snapPoint.position - CurrentRoom.transform.position;

        GameObject newRoom = RoomHandler.CreateRoom(
            roomPrefab,
            currentRoom,
            offset,
            snapPoint.rotation
        );
        allRooms.Add(newRoom, false); // Add to dictionary as not entered yet

        InstantiateEnemies(newRoom, roomType);

        Debug.Log($"Room of type {roomPrefab.name} spawned at: {snapPoint.position}");
    }

    private void InstantiateEnemies(GameObject room, RoomType roomType, int numEnemies = 1)
    {
        Room roomComponent = room.GetComponent<Room>();
        int maxEnemies = roomComponent.GetTotalSpawnLocations();

        numEnemies = Mathf.Max(1, numEnemies);
        int enemiesToSpawn = Mathf.Min(numEnemies, maxEnemies);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            roomComponent.AddEnemyToSpawnedEnemies(GetCorrectEnemyType(roomType));
        }
    }

    private GameObject GetCorrectEnemyType(RoomType roomType)
    {
        switch (roomType)
        {
            case RoomType.Default:
                return defaultEnemyPrefabs[RandomUtils.Range(0, defaultEnemyPrefabs.Length - 1)];
            case RoomType.Elite:
                return eliteEnemyPrefabs[RandomUtils.Range(0, eliteEnemyPrefabs.Length - 1)];
            case RoomType.Boss:
                return bossEnemyPrefabs[RandomUtils.Range(0, bossEnemyPrefabs.Length - 1)];
            default:
                return null;
        }
    }

    // Destroys all rooms except the specified one
    private void DestroyAllRoomsExcept(GameObject roomToKeep)
    {
        List<GameObject> roomsToRemove = new List<GameObject>();

        // Collect rooms to remove
        foreach (var roomEntry in allRooms)
        {
            if (roomEntry.Key != roomToKeep)
            {
                roomsToRemove.Add(roomEntry.Key);
            }
        }

        // Destroy and remove collected rooms
        foreach (var room in roomsToRemove)
        {
            Destroy(room);
            allRooms.Remove(room);
        }
    }
}

public enum RoomType
{
    Default,
    Elite,
    Treasure,
    Boss
}
