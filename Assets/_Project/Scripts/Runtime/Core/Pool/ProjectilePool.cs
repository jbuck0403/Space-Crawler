// using Unity.VisualScripting;
// using UnityEngine;

// public class ProjectilePool : PoolBase
// {
//     [SerializeField]
//     int poolCount;

//     public static ProjectilePool Instance { get; private set; }

//     private void Awake()
//     {
//         Instance = this;
//     }

//     private void Update()
//     {
//         poolCount = pool.Count;

//         if (
//             initialized
//             && poolCount <= numObjects / 2
//             && currentNumObjects < maxPoolSize - (incrementSize * 2)
//         )
//         {
//             if (poolCount <= numObjects / incrementSize * 2)
//             {
//                 IncreasePoolSize(incrementSize * 2);
//             }
//             else
//             {
//                 IncreasePoolSize(incrementSize);
//             }
//         }
//     }

//     private void RepositionProjectileForFiring(GameObject obj, Transform weaponTip)
//     {
//         obj.transform.SetPositionAndRotation(weaponTip.position, weaponTip.rotation);

//         Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
//         if (rb != null)
//         {
//             rb.velocity = Vector2.zero;
//             rb.angularVelocity = 0f;
//         }
//         else
//         {
//             Debug.LogWarning("Projectile does not have a Rigidbody2D component!");
//         }
//     }

//     public GameObject GetProjectile(Transform weaponTip)
//     {
//         GameObject projectile = GetObject();
//         RepositionProjectileForFiring(projectile, weaponTip);

//         return projectile;
//     }

//     protected override GameObject AddObjectToPool()
//     {
//         GameObject obj = base.AddObjectToPool();

//         if (obj != null)
//         {
//             Projectile projectile = obj.GetComponent<Projectile>();
//             if (projectile != null)
//             {
//                 projectile.Initialize();
//             }
//         }

//         return obj;
//     }
// }
