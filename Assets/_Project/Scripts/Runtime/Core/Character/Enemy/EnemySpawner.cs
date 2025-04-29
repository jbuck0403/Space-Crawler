using UnityEngine;

public static class EnemySpawner
{
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
