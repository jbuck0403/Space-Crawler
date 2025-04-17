using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RoomHandler
{
    // Create a new room based on the current room's position
    public static GameObject CreateRoom(
        GameObject roomPrefab,
        GameObject currentRoom,
        Vector3 offset,
        Quaternion rotation
    )
    {
        if (roomPrefab == null)
            return null;

        // Calculate position relative to current room
        Vector3 position =
            currentRoom != null ? currentRoom.transform.position + offset : Vector3.zero;

        // Create the new room
        GameObject newRoom = Object.Instantiate(roomPrefab, position, rotation);
        return newRoom;
    }

    // Destroy a room
    public static void DestroyRoom(GameObject room)
    {
        if (room != null)
            Object.Destroy(room);
    }
}
