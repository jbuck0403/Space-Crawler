using UnityEngine;

public class AOETest : MonoBehaviour
{
    [SerializeField]
    private GameObject aoePrefab;

    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private float maxRaycastDistance = 100f;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SpawnAOEAtMousePosition();
        }
    }

    private void SpawnAOEAtMousePosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, maxRaycastDistance, groundLayer);

        Vector3 spawnPosition;

        if (hit.collider != null)
        {
            // use the hit position if we hit something on the ground layer
            spawnPosition = hit.point;
        }
        else
        {
            // fallback to converting screen point to world point at z=0
            spawnPosition = mainCamera.ScreenToWorldPoint(
                new Vector3(
                    Input.mousePosition.x,
                    Input.mousePosition.y,
                    -mainCamera.transform.position.z
                )
            );
        }

        // AOESpawner.CreateTrackerAOE(
        //     aoePrefab,
        //     GameObject.FindGameObjectWithTag("Player").transform,
        //     spawnPosition
        // );
        // AOESpawner.CreateStationaryAOE(aoePrefab, spawnPosition);
        // AOESpawner.CreateAuraAOE(aoePrefab, GameObject.FindGameObjectWithTag("Player").transform);
    }
}
