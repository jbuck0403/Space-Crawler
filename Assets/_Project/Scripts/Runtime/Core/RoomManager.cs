using System.Collections.Generic;
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
    private List<GameObject> allRooms = new List<GameObject>();
    private int roomsCompleted = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Spawn initial room
        currentRoom = Instantiate(normalRoomPrefab, Vector3.zero, Quaternion.identity);
        allRooms.Add(currentRoom);

        // Spawn connecting rooms
        SpawnConnectingRooms();
    }

    // Called by RoomCollider when player enters a room
    public void OnPlayerEnteredRoom(GameObject room)
    {
        // Set this as the current room
        currentRoom = room;

        // Destroy all other rooms
        DestroyAllRoomsExcept(currentRoom);

        // Increment counter (this is a new room)
        roomsCompleted++;

        // Spawn new connecting rooms
        SpawnConnectingRooms();
    }

    // Spawns rooms with chance-based selection
    private void SpawnConnectingRooms()
    {
        RoomCollider roomCollider = currentRoom.GetComponent<RoomCollider>();
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
                SpawnRoomAtSnapPoint(snapPoints[0], bossRoomPrefab);
                Debug.Log("Boss room spawned!");
            }
            else
            {
                SpawnRoomAtSnapPoint(snapPoints[0], normalRoomPrefab);
            }
        }

        // Second snap point has a chance for elite room
        if (snapPoints.Length > 1 && Random.value < eliteRoomChance)
        {
            GameObject roomPrefab = eliteRoomPrefab != null ? eliteRoomPrefab : normalRoomPrefab;
            SpawnRoomAtSnapPoint(snapPoints[1], roomPrefab);
        }

        // Third snap point has a chance for treasure room
        if (snapPoints.Length > 2 && Random.value < treasureRoomChance)
        {
            GameObject roomPrefab =
                treasureRoomPrefab != null ? treasureRoomPrefab : normalRoomPrefab;
            SpawnRoomAtSnapPoint(snapPoints[2], roomPrefab);
        }
    }

    // Spawn a specific room prefab at a snap point
    private void SpawnRoomAtSnapPoint(Transform snapPoint, GameObject roomPrefab)
    {
        if (snapPoint == null || roomPrefab == null)
            return;

        // Instantiate the room at the exact position and rotation of the snap point
        GameObject newRoom = Instantiate(roomPrefab, snapPoint.position, snapPoint.rotation);
        allRooms.Add(newRoom);

        Debug.Log($"Room of type {roomPrefab.name} spawned at: {snapPoint.position}");
    }

    // Destroys all rooms except the specified one
    private void DestroyAllRoomsExcept(GameObject roomToKeep)
    {
        for (int i = allRooms.Count - 1; i >= 0; i--)
        {
            if (allRooms[i] != roomToKeep)
            {
                Destroy(allRooms[i]);
                allRooms.RemoveAt(i);
            }
        }
    }
}
