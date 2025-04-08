using UnityEngine;

public class RoomCollider : MonoBehaviour
{
    [Header("Room Setup")]
    [SerializeField]
    private Transform entrancePoint;

    [Header("Snap Points")]
    [SerializeField]
    private Transform[] snapPoints; // Array of exit snap points

    private void OnTriggerEnter(Collider other)
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
}
