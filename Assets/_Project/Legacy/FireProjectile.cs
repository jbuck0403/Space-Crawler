// using Unity.VisualScripting;
// using UnityEngine;

// public class FireProjectile
// {
//     public static Projectile Fire(
//         ProjectilePool projectilePool,
//         DamageProfile damageProfile,
//         Transform firePoint,
//         Vector2 direction,
//         float speed,
//         Transform source
//     )
//     {
//         // get a projectile from the pool
//         GameObject projectileObject = projectilePool.GetProjectile(firePoint);

//         // get the projectile component
//         Projectile projectile = projectileObject.GetComponent<Projectile>();
//         if (projectile == null)
//         {
//             Debug.LogError("Projectile from pool does not have a Projectile component!");
//             projectilePool.ReturnToPool(projectileObject);
//             return null;
//         }

//         // generate damage data from the profile
//         DamageData damageData = damageProfile.CreateDamageData(source);

//         // set the damage data on the projectile
//         projectile.damageData = damageData;

//         ApplyVelocity(projectileObject, direction, speed);

//         return projectile;
//     }

//     private static void ApplyVelocity(GameObject projectileObject, Vector2 direction, float speed)
//     {
//         Rigidbody2D rb = projectileObject.GetComponent<Rigidbody2D>();
//         if (rb != null)
//         {
//             rb.velocity = direction.normalized * speed;
//         }
//         else
//         {
//             Debug.LogWarning(
//                 "Projectile does not have a Rigidbody component. Movement will not be applied."
//             );
//         }
//     }
// }
