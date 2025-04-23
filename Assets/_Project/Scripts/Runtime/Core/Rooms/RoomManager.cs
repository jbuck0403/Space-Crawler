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

    private bool bossRoomSpawned = false;
    private bool bossRoomEntered = false;
    private GameObject player;
    private PlayerController playerController;
    public PlayerController PlayerController => playerController;
    public GameObject Player => player;
    public GameObject CurrentRoom => currentRoom;
    public Room CurrentRoomComponent => currentRoomComponent;

    public bool IsRoomCompleted { get; private set; } = false;
    public bool AreEnemiesCleared { get; private set; } = false;
    public bool AreRoomsSpawned { get; private set; } = false;
    public bool IsRoomEntered { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool Initialize()
    {
        InitDungeonState();

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

            allRooms.Add(currentRoom, false); // initialize as not entered yet

            return true;
        }

        return false;
    }

    private void InitDungeonState()
    {
        roomsCompleted = 0;
        currentRoom = null;
        currentRoomComponent = null;

        bossRoomSpawned = false;
        bossRoomEntered = false;

        IsRoomCompleted = false;
        AreEnemiesCleared = false;
        AreRoomsSpawned = false;
        IsRoomEntered = false;
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
            playerController = player.GetComponent<PlayerController>();
            defaultTarget = player.transform;

            GameManager.Instance.VirtualCamera.Follow = defaultTarget;
            GameManager.Instance.VirtualCamera.LookAt = defaultTarget;
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

        IsRoomEntered = true;
        IsRoomCompleted = false;
        AreEnemiesCleared = false;
        AreRoomsSpawned = false;

        if (bossRoomSpawned)
        {
            bossRoomEntered = true;
        }

        // Mark the room as entered
        allRooms[room] = true;

        // Set this as the current room
        currentRoom = room;

        Room roomComponent = currentRoom.GetComponent<Room>();
        if (roomComponent != null)
        {
            currentRoomComponent = roomComponent;
            roomComponent.InitializeSpawnedEnemies(defaultTarget);
        }

        // Destroy all other rooms
        DestroyAllRoomsExcept(currentRoom);

        // Increment counter (this is a new room)
        roomsCompleted++;

        if (bossRoomSpawned)
        {
            print($"#BOSS SPAWNED {bossRoomSpawned} ENTERED {bossRoomEntered}");
            return;
        }

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
            if (!bossRoomSpawned && roomsCompleted >= roomsBeforeBoss && bossRoomPrefab != null)
            {
                SpawnRoomAtSnapPoint(snapPoints[0], bossRoomPrefab, RoomType.Boss);
                Debug.Log("Boss room spawned!");
                bossRoomSpawned = true;
            }
            else
            {
                SpawnRoomAtSnapPoint(snapPoints[0], normalRoomPrefab, RoomType.Default);
            }
        }

        if (!bossRoomSpawned)
        {
            // Second snap point has a chance for elite room
            if (snapPoints.Length > 1 && Random.value < eliteRoomChance)
            {
                GameObject roomPrefab =
                    eliteRoomPrefab != null ? eliteRoomPrefab : normalRoomPrefab;
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

        // After all rooms have been spawned, update state
        AreRoomsSpawned = true;

        // Check if current room has any enemies
        if (currentRoomComponent != null && currentRoomComponent.spawnedEnemies.Count == 0)
        {
            AreEnemiesCleared = true;
            CheckRoomCompletion();
        }

        // Notify GameplayRoomState
        if (GameManager.Instance.CurrentStateType == GameStateType.GameplayRoom)
        {
            GameplayRoomState gameplayState =
                GameManager.Instance.CurrentState as GameplayRoomState;
            if (gameplayState != null)
            {
                Debug.Log("#ROOM Notifying GameplayRoomState that rooms have been spawned");
                gameplayState.OnRoomsSpawned();

                // Also notify about room entry so it can monitor enemies
                Debug.Log("#ROOM Notifying GameplayRoomState about room entry");
                gameplayState.MonitorCurrentRoom();
            }
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

        // Find which door in the current room this snap point belongs to
        DoorData exitDoor = FindDoorByCoordsOrSnapPoint(currentRoomComponent, snapPoint);
        if (exitDoor != null)
        {
            Debug.Log($"#ROOM Found exit door in current room: {exitDoor.doorSide}");
            exitDoor.hasRoomBeyond = true;

            // Find the opposite door in the new room to set as its entrance
            Room newRoomComponent = newRoom.GetComponent<Room>();
            if (newRoomComponent != null && newRoomComponent.EntranceDoor != null)
            {
                Debug.Log(
                    $"#ROOM New room entrance door: {newRoomComponent.EntranceDoor.doorSide}"
                );

                // Link the entrance door to look back at the current room
                newRoomComponent.EntranceDoor.hasRoomBeyond = true;
                newRoomComponent.EntranceDoor.roomBeyondSnapPoint = currentRoom.transform;
            }
        }
        else
        {
            Debug.LogError("#ROOM Could not find exit door in current room for given snap point");
        }

        InstantiateEnemies(newRoom, roomType);
        Debug.Log($"#ROOM Room of type {roomPrefab.name} spawned at: {snapPoint.position}");
    }

    // Add helper method to find door by snap point
    private DoorData FindDoorByCoordsOrSnapPoint(Room room, Transform snapPoint)
    {
        if (room == null || snapPoint == null)
            return null;

        foreach (DoorData doorData in room.ExitDoors)
        {
            if (doorData.roomBeyondSnapPoint == snapPoint)
            {
                return doorData;
            }
        }

        Debug.LogWarning("#ROOM No door found with matching snap point");
        return null;
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
    public void DestroyAllRoomsExcept(GameObject roomToKeep)
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

    // New method to handle enemy death notification
    public void HandleEnemyDefeated(BaseEnemyController enemy)
    {
        if (currentRoomComponent != null)
        {
            Debug.Log($"#ROOM Spawned Enemies {currentRoomComponent.spawnedEnemies.Count}");
            currentRoomComponent.RemoveEnemyFromSpawnedList(enemy);

            // Check if all enemies are cleared
            if (currentRoomComponent.spawnedEnemies.Count == 0)
            {
                Debug.Log("#ROOM All enemies defeated - room cleared");
                AreEnemiesCleared = true;
                CheckRoomCompletion();
            }

            Debug.Log(
                $"#ROOM Enemy defeated - {enemy.name}, remaining: {currentRoomComponent.spawnedEnemies.Count}, in boss room: {bossRoomEntered}"
            );
        }
    }

    // New method to check for room completion
    private void CheckRoomCompletion()
    {
        if (!IsRoomCompleted && AreEnemiesCleared && AreRoomsSpawned)
        {
            Debug.Log("#ROOM Room completion criteria met");
            CompleteCurrentRoom();
        }
        else if (bossRoomEntered && bossRoomSpawned)
        {
            CompleteCurrentRoom();
        }

        Debug.Log(
            $"#ROOM Checking completion - enemies cleared: {AreEnemiesCleared}, rooms spawned: {AreRoomsSpawned}, is complete: {IsRoomCompleted}, in boss room: {bossRoomEntered}"
        );
    }

    // New method to handle room completion actions
    private void CompleteCurrentRoom()
    {
        IsRoomCompleted = true;

        if (currentRoomComponent != null)
        {
            if (bossRoomEntered)
            {
                Debug.Log("#ROOM BOSS DEFEATED - COMPLETING RUN");

                GameManager.Instance.SetBossDefeated();
            }
            else
            {
                Debug.Log("#ROOM Opening doors with rooms beyond");
                currentRoomComponent.OpenDoorsWithRoomBeyond();
            }
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
