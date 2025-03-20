using UnityEngine;

// public class AOESpawnTester : MonoBehaviour
// {
//     [SerializeField]
//     private Transform target;

//     [SerializeField]
//     private Transform aoeSpawn;

//     [SerializeField]
//     private GameObject aoePrefab;

//     private void SpawnAOE()
//     {
//         GameObject aoe = Instantiate(aoePrefab);
//         AOEController controller = aoe.GetComponent<AOEController>();
//         controller.Initialize(target);
//     }

//     private void Start()
//     {
//         SpawnAOE();
//     }
// }

public static class AOESpawner
{
    /// <summary>
    /// Core AOE spawning method that all other methods use
    /// </summary>
    public static AOEController SpawnAOE(
        GameObject aoePrefab,
        Transform target,
        Vector3 position,
        Transform parent = null
    )
    {
        GameObject aoe = Object.Instantiate(aoePrefab, position, Quaternion.identity, parent);
        AOEController controller = aoe.GetComponent<AOEController>();
        controller.Initialize(target);

        return controller;
    }

    /// <summary>
    /// Creates an aura AOE that is parented to a character
    /// </summary>
    public static AOEController CreateAuraAOE(
        GameObject aoePrefab,
        Transform parent,
        Vector3 offset = default
    )
    {
        AOEController controller = SpawnAOE(
            aoePrefab,
            null, // No target to follow
            parent.position + offset,
            parent // Parent directly to the character
        );

        // Ensure it doesn't try to follow anything
        controller.StopFollowing();

        return controller;
    }

    /// <summary>
    /// Creates an AOE that follows a target
    /// </summary>
    public static AOEController CreateTrackerAOE(
        GameObject aoePrefab,
        Transform target,
        Vector3 startPosition = default
    )
    {
        // If position not specified, use target position
        if (startPosition == default && target != null)
        {
            startPosition = target.position;
        }

        AOEController controller = SpawnAOE(
            aoePrefab,
            target, // Target to follow
            startPosition,
            null // No parent
        );

        // Explicitly set target to follow
        controller.SetFollowTarget(target);

        return controller;
    }

    /// <summary>
    /// Creates a stationary AOE at a specific position
    /// </summary>
    public static AOEController CreateStationaryAOE(GameObject aoePrefab, Vector3 position)
    {
        AOEController controller = SpawnAOE(
            aoePrefab,
            null, // No target to follow
            position,
            null // No parent
        );

        // Explicitly stop any following behavior
        controller.StopFollowing();

        return controller;
    }
}
