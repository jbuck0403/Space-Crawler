// FireConfig.cs
using UnityEngine;

[CreateAssetMenu(fileName = "FireConfig", menuName = "SpaceShooter/Fire Config")]
public class FireConfig : ScriptableObject
{
    [Header("Firing")]
    [Tooltip("Minimum time between shots")]
    public float fireRate = 1f;

    [Tooltip("Maximum range at which the entity will fire")]
    public float maxFireRange = 15f;

    [Tooltip("Minimum range at which the entity will fire")]
    public float minFireRange = 2f;

    [Tooltip("Accuracy of shots (0-1, where 1 is perfect accuracy)")]
    [Range(0, 1)]
    public float accuracy = 0.9f;

    [Header("Burst Fire")]
    [Tooltip("Number of shots in a burst (1 for single fire)")]
    public int burstCount = 1;

    [Tooltip("Time between shots in a burst")]
    public float burstInterval = 0.1f;

    // optional: Add validation
    private void OnValidate()
    {
        fireRate = Mathf.Max(0.1f, fireRate);
        maxFireRange = Mathf.Max(minFireRange + 1f, maxFireRange);
        minFireRange = Mathf.Max(0f, minFireRange);
        burstCount = Mathf.Max(1, burstCount);
        burstInterval = Mathf.Max(0.05f, burstInterval);
    }
}
