using UnityEngine;

/// <summary>
/// Simple handler for door objects to toggle their visibility
/// </summary>
public class DoorHandler : MonoBehaviour
{
    [Tooltip("Optional visual representation of the door")]
    [SerializeField]
    private GameObject doorVisual;

    private void Awake()
    {
        // If no door visual is assigned, use this object as the door visual
        if (doorVisual == null)
        {
            doorVisual = gameObject;
        }
    }

    /// <summary>
    /// Opens the door by deactivating it
    /// </summary>
    public void OpenDoor()
    {
        Debug.Log(
            $"#ROOM DoorHandler.OpenDoor called on {gameObject.name}, doorVisual: {(doorVisual != null ? doorVisual.name : "null")}"
        );
        if (doorVisual == null)
        {
            Debug.LogError($"#ROOM doorVisual is null in DoorHandler on {gameObject.name}");
            return;
        }

        doorVisual.SetActive(false);
        Debug.Log(
            $"#ROOM Door visual should now be inactive: {doorVisual.name}, active: {doorVisual.activeSelf}"
        );
    }

    /// <summary>
    /// Closes the door by activating it
    /// </summary>
    public void CloseDoor()
    {
        Debug.Log(
            $"#ROOM DoorHandler.CloseDoor called on {gameObject.name}, doorVisual: {(doorVisual != null ? doorVisual.name : "null")}"
        );
        if (doorVisual == null)
        {
            Debug.LogError($"#ROOM doorVisual is null in DoorHandler on {gameObject.name}");
            return;
        }

        doorVisual.SetActive(true);
        Debug.Log(
            $"#ROOM Door visual should now be active: {doorVisual.name}, active: {doorVisual.activeSelf}"
        );
    }
}
