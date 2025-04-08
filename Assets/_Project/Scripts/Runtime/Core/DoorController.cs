using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField]
    private bool isLocked = true;

    [SerializeField]
    private GameObject lockedVisual;

    [SerializeField]
    private GameObject unlockedVisual;

    [SerializeField]
    private BoxCollider doorTrigger;

    private GameObject targetRoom;

    private void Start()
    {
        UpdateDoorVisuals();
    }

    public void SetTargetRoom(GameObject room)
    {
        targetRoom = room;
    }

    public void UnlockDoor()
    {
        isLocked = false;
        UpdateDoorVisuals();
    }

    public void LockDoor()
    {
        isLocked = true;
        UpdateDoorVisuals();
    }

    private void UpdateDoorVisuals()
    {
        if (lockedVisual != null)
            lockedVisual.SetActive(isLocked);

        if (unlockedVisual != null)
            unlockedVisual.SetActive(!isLocked);

        if (doorTrigger != null)
            doorTrigger.enabled = !isLocked;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isLocked && targetRoom != null)
        {
            // Find entrance position in target room
            Transform entrancePoint = FindEntrancePoint(targetRoom);

            if (entrancePoint != null)
            {
                // Get the position in front of the entrance
                Vector3 teleportPosition = entrancePoint.position + entrancePoint.forward * 2f;

                // Make sure the player is facing into the room
                Quaternion teleportRotation = Quaternion.LookRotation(entrancePoint.forward);

                // Teleport the player
                other.transform.position = teleportPosition;
                other.transform.rotation = teleportRotation;
            }
        }
    }

    private Transform FindEntrancePoint(GameObject room)
    {
        // Look for a transform named "EntrancePoint" in the target room
        Transform[] allTransforms = room.GetComponentsInChildren<Transform>();
        foreach (Transform t in allTransforms)
        {
            if (t.name.Contains("EntrancePoint"))
            {
                return t;
            }
        }

        // If no entrance point is found, return the room's transform
        return room.transform;
    }
}
