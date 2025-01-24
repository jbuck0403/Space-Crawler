using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private MovementConfig movementConfig;

    private MovementHandler movementHandler;
    private Vector2 moveInput;
    private Camera mainCamera;

    private void Awake()
    {
        movementHandler = new MovementHandler(movementConfig);
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Get movement input
        moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        // Get mouse position for rotation
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimDirection = (mousePosition - (Vector2)transform.position).normalized;

        // Calculate new position and rotation
        Vector2 newPosition = movementHandler.CalculateMovement(
            moveInput,
            transform.position,
            Time.deltaTime
        );

        float newRotation = movementHandler.CalculateRotation(
            aimDirection,
            transform.eulerAngles.z,
            movementConfig.rotationSpeed,
            Time.deltaTime
        );

        // Apply movement and rotation
        transform.position = newPosition;
        transform.rotation = Quaternion.Euler(0, 0, newRotation);
    }
}
