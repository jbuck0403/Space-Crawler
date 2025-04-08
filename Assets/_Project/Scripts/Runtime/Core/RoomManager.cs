using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [Header("Room Prefabs")]
    [SerializeField]
    private GameObject normalRoomPrefab;

    // Currently active room
    private GameObject currentRoom;
    private List<GameObject> allRooms = new List<GameObject>();

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

        // Spawn new connecting rooms
        SpawnConnectingRooms();
    }

    // Spawns rooms at all snap points
    private void SpawnConnectingRooms()
    {
        RoomCollider roomCollider = currentRoom.GetComponent<RoomCollider>();
        if (roomCollider == null)
            return;

        Transform[] snapPoints = roomCollider.GetSnapPoints();
        if (snapPoints == null)
            return;

        // Create a room at each snap point
        foreach (Transform snapPoint in snapPoints)
        {
            if (snapPoint != null)
            {
                SpawnRoomAtSnapPoint(snapPoint);
            }
        }
    }

    // Spawn a room at a specific snap point using exact transform
    private void SpawnRoomAtSnapPoint(Transform snapPoint)
    {
        // Simply instantiate a room at the exact position and rotation of the snap point
        GameObject newRoom = Instantiate(normalRoomPrefab, snapPoint.position, snapPoint.rotation);
        allRooms.Add(newRoom);

        Debug.Log(
            $"Room spawned at: {snapPoint.position} with rotation: {snapPoint.rotation.eulerAngles}"
        );
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
