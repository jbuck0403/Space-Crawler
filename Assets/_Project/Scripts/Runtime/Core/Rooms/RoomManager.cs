// using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using Unity.VisualScripting;
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

    [DoNotSerialize]
    private int roomsBeforeBoss = 10;

    [SerializeField]
    private int baseRoomsBeforeBoss = 1; // Just start and boss initially

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

        roomsBeforeBoss = baseRoomsBeforeBoss + GameManager.Instance.GameData.runsCompleted;

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

            allRooms.Add(currentRoom, false);

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
            WeaponHandler weaponHandler = player.GetComponent<WeaponHandler>();
            CollectibleManager.Instance.Initialize(weaponHandler);

            playerController = player.GetComponent<PlayerController>();
            defaultTarget = player.transform;

            GameManager.Instance.VirtualCamera.Follow = defaultTarget;
            GameManager.Instance.VirtualCamera.LookAt = defaultTarget;
        }
    }

    public void OnPlayerEnteredRoom(GameObject room)
    {
        if (!allRooms.ContainsKey(room))
        {
            Debug.LogWarning("Player entered an unknown room");
            return;
        }

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

        allRooms[room] = true;

        currentRoom = room;

        Room roomComponent = currentRoom.GetComponent<Room>();
        if (roomComponent != null)
        {
            currentRoomComponent = roomComponent;
            roomComponent.InitializeSpawnedEnemies(defaultTarget);
        }

        DestroyAllRoomsExcept(currentRoom);

        roomsCompleted++;

        if (bossRoomSpawned)
        {
            print($"#BOSS SPAWNED {bossRoomSpawned} ENTERED {bossRoomEntered}");
            return;
        }

        SpawnConnectingRooms();
    }

    private void SpawnConnectingRooms() // instantiates enemies but doesn't initialize
    {
        Room roomCollider = currentRoom.GetComponent<Room>();
        if (roomCollider == null)
            return;

        Transform[] snapPoints = roomCollider.GetSnapPoints();
        if (snapPoints == null || snapPoints.Length == 0)
            return;

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
            if (snapPoints.Length > 1 && Random.value < eliteRoomChance)
            {
                GameObject roomPrefab =
                    eliteRoomPrefab != null ? eliteRoomPrefab : normalRoomPrefab;
                SpawnRoomAtSnapPoint(snapPoints[1], roomPrefab, RoomType.Elite);
            }

            if (snapPoints.Length > 2 && Random.value < treasureRoomChance)
            {
                GameObject roomPrefab =
                    treasureRoomPrefab != null ? treasureRoomPrefab : normalRoomPrefab;
                SpawnRoomAtSnapPoint(snapPoints[2], roomPrefab, RoomType.Treasure);
            }
        }

        AreRoomsSpawned = true;

        if (currentRoomComponent != null && currentRoomComponent.spawnedEnemies.Count == 0)
        {
            AreEnemiesCleared = true;
            CheckRoomCompletion();
        }

        if (GameManager.Instance.CurrentStateType == GameStateType.GameplayRoom)
        {
            GameplayRoomState gameplayState =
                GameManager.Instance.CurrentState as GameplayRoomState;
            if (gameplayState != null)
            {
                Debug.Log("#ROOM Notifying GameplayRoomState that rooms have been spawned");
                gameplayState.OnRoomsSpawned();

                Debug.Log("#ROOM Notifying GameplayRoomState about room entry");
                gameplayState.MonitorCurrentRoom();
            }
        }
    }

    private void SpawnRoomAtSnapPoint(Transform snapPoint, GameObject roomPrefab, RoomType roomType)
    {
        if (snapPoint == null || roomPrefab == null)
            return;

        Room newRoomComponent = null;
        Vector3 offset = snapPoint.position - CurrentRoom.transform.position;

        GameObject newRoom = RoomHandler.CreateRoom(
            roomPrefab,
            currentRoom,
            offset,
            snapPoint.rotation
        );
        allRooms.Add(newRoom, false);

        DoorData exitDoor = FindDoorByCoordsOrSnapPoint(currentRoomComponent, snapPoint);
        if (exitDoor != null)
        {
            Debug.Log($"#ROOM Found exit door in current room: {exitDoor.doorSide}");
            exitDoor.hasRoomBeyond = true;

            newRoomComponent = newRoom.GetComponent<Room>();
            if (newRoomComponent != null && newRoomComponent.EntranceDoor != null)
            {
                Debug.Log(
                    $"#ROOM New room entrance door: {newRoomComponent.EntranceDoor.doorSide}"
                );

                newRoomComponent.EntranceDoor.hasRoomBeyond = true;
                newRoomComponent.EntranceDoor.roomBeyondSnapPoint = currentRoom.transform;
            }
        }
        else
        {
            Debug.LogError("#ROOM Could not find exit door in current room for given snap point");
        }

        if (roomType == RoomType.Treasure)
            InstantiateTreasure(newRoomComponent, roomType);
        else
            InstantiateEnemies(newRoomComponent, roomType);
        Debug.Log($"#ROOM Room of type {roomPrefab.name} spawned at: {snapPoint.position}");
    }

    private void InstantiateTreasure(Room treasureRoom, RoomType roomType)
    {
        if (roomType != RoomType.Treasure)
            return;

        int numSpawnLocations = treasureRoom.enemySpawnLocations.Count();
        Transform treasureTransform = null;

        if (treasureRoom != null && numSpawnLocations > 0)
        {
            treasureTransform = treasureRoom.enemySpawnLocations[0].transform;
        }

        SpawnLoot(treasureTransform);
    }

    private void SpawnLoot(Transform transform)
    {
        int rand = RandomUtils.Range(0, 2);
        switch (rand)
        {
            case 0:
                CollectibleManager.SpawnRandomWeapon(transform);
                break;
            case 1:
                CollectibleManager.SpawnRandomAmmo(transform);
                break;
        }
    }

    private int HowManyEnemiesToSpawn(Room roomComponent)
    {
        int totalSpawnLocations = roomComponent.GetTotalSpawnLocations();

        float progressionRatio = Mathf.Min(1.0f, (float)roomsCompleted / roomsBeforeBoss);

        int maxEnemies = Mathf.Max(1, Mathf.CeilToInt(totalSpawnLocations * progressionRatio));

        return RandomUtils.Range(1, maxEnemies);
    }

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

    private void InstantiateEnemies(Room room, RoomType roomType)
    {
        int enemiesToSpawn = HowManyEnemiesToSpawn(room);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            room.AddEnemyToSpawnedEnemies(GetCorrectEnemyType(roomType));
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

    public void DestroyAllRoomsExcept(GameObject roomToKeep)
    {
        List<GameObject> roomsToRemove = new List<GameObject>();

        foreach (var roomEntry in allRooms)
        {
            if (roomEntry.Key != roomToKeep)
            {
                roomsToRemove.Add(roomEntry.Key);
            }
        }

        foreach (var room in roomsToRemove)
        {
            Destroy(room);
            allRooms.Remove(room);
        }
    }

    public void HandleEnemyDefeated(BaseEnemyController enemy)
    {
        if (currentRoomComponent != null)
        {
            Debug.Log($"#ROOM Spawned Enemies {currentRoomComponent.spawnedEnemies.Count}");
            currentRoomComponent.RemoveEnemyFromSpawnedList(enemy);

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
