using UnityEngine;

public class AOESpawnTester : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private Transform aoeSpawn;

    [SerializeField]
    private GameObject aoePrefab;

    private void SpawnAOE()
    {
        GameObject aoe = Instantiate(aoePrefab);
        AOEController controller = aoe.GetComponent<AOEController>();
    }

    private void Start()
    {
        SpawnAOE();
    }
}
