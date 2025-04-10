using UnityEngine;

public static class EnemySpawner
{
    /// <summary>
    /// Spawns an enemy at the given position and rotation
    /// </summary>
    /// <param name="enemyPrefab">The enemy prefab to spawn</param>
    /// <param name="position">The position to spawn the enemy at</param>
    /// <param name="rotation">The rotation of the spawned enemy</param>
    /// <param name="target">Optional target for the enemy to follow</param>
    /// <returns>The spawned enemy game object, or null if spawn failed</returns>
    public static GameObject SpawnEnemy(
        GameObject enemyPrefab,
        Vector3 position,
        Quaternion rotation,
        Transform target
    )
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Cannot spawn null enemy prefab");
            return null;
        }

        GameObject enemyInstance = Object.Instantiate(enemyPrefab, position, rotation);
        BaseEnemyController instanceEnemyController =
            enemyInstance.GetComponent<BaseEnemyController>();
        if (instanceEnemyController != null)
        {
            instanceEnemyController.Initialize(target);
        }

        return enemyInstance;
    }

    /// <summary>
    /// Sets the target for an enemy
    /// </summary>
    /// <param name="enemyInstance">The enemy instance to set the target for</param>
    /// <param name="target">The target transform</param>
    /// <returns>True if successful, false otherwise</returns>
    public static bool SetEnemyTarget(GameObject enemyInstance, Transform target)
    {
        if (enemyInstance == null || target == null)
            return false;

        BaseEnemyController enemyController = enemyInstance.GetComponent<BaseEnemyController>();
        if (enemyController != null)
        {
            enemyController.UpdateTarget(target, true);
            return true;
        }

        return false;
    }
}
