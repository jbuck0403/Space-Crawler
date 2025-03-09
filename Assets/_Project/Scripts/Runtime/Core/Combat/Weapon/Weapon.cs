using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private ProjectilePool projectilePool;

    [SerializeField]
    private DamageProfile damageProfile;

    [SerializeField]
    private Transform weaponTip;

    [SerializeField]
    private float fireRate = 0.5f;

    [SerializeField]
    private float projectileSpeed = 20f;

    private float nextFireTime;

    private void Awake()
    {
        if (weaponTip == null)
            weaponTip = transform;
    }

    public void Fire()
    {
        if (Time.time < nextFireTime)
            return;

        nextFireTime = Time.time + fireRate;

        // Get projectile from pool
        GameObject projectileObj = projectilePool.GetProjectile(weaponTip);

        // Set damage data
        if (projectileObj.TryGetComponent<Projectile>(out var projectile))
        {
            // Create damage data from profile
            projectile.damageData = damageProfile.CreateDamageData(transform);

            // Apply velocity to the projectile
            Rigidbody rb = projectileObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = weaponTip.forward * projectileSpeed;
            }
        }
    }

    // Optional: Method to fire with custom damage data
    public void FireWithCustomDamageData(DamageData customDamageData)
    {
        if (Time.time < nextFireTime)
            return;

        nextFireTime = Time.time + fireRate;

        // Get projectile from pool
        GameObject projectileObj = projectilePool.GetProjectile(weaponTip);

        // Set custom damage data
        if (projectileObj.TryGetComponent<Projectile>(out var projectile))
        {
            projectile.damageData = customDamageData;

            // Apply velocity to the projectile
            Rigidbody rb = projectileObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = weaponTip.forward * projectileSpeed;
            }
        }
    }
}
