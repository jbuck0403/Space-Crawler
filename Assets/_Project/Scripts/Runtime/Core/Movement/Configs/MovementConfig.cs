using UnityEngine;

[CreateAssetMenu(fileName = "MovementConfig", menuName = "SpaceShooter/Movement Config")]
public class MovementConfig : ScriptableObject
{
    [Header("Movement")]
    [Tooltip("Maximum movement speed")]
    public float maxSpeed = 5f;

    [Tooltip("How quickly the entity reaches max speed")]
    public float acceleration = 50f;

    [Tooltip("How quickly the entity comes to a stop")]
    public float deceleration = 50f;

    [Header("Rotation")]
    [Tooltip("How quickly the entity rotates to face movement direction")]
    public float rotationSpeed = 10f;

    private void OnValidate()
    {
        maxSpeed = Mathf.Max(0.1f, maxSpeed);
        acceleration = Mathf.Max(0.1f, acceleration);
        deceleration = Mathf.Max(0.1f, deceleration);
        rotationSpeed = Mathf.Max(0f, rotationSpeed);
    }
}
