// FireConfig.cs
using UnityEngine;

[CreateAssetMenu(fileName = "FireConfig", menuName = "SpaceShooter/Fire Config")]
public class FireConfig : ScriptableObject
{
    [Header("Firing")]
    [Tooltip("Minimum time between shots")]
    public float fireRate = 1f;

    [Tooltip("Accuracy of shots (0-1, where 1 is perfect accuracy)")]
    [Range(0, 1)]
    public float accuracy = 0.9f;

    [Range(10, 100)]
    public float projectileSpeed = 10f;

    private void OnValidate()
    {
        fireRate = Mathf.Max(0.1f, fireRate);
    }
}
