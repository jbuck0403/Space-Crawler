using UnityEngine;

public static class WeaponVFXHandler
{
    public static void HandleMuzzleFlare(
        GameObject muzzleFlashPrefab,
        Transform firePoint,
        Transform source
    )
    {
        GameObject muzzleFlash = GameObject.Instantiate(
            muzzleFlashPrefab,
            firePoint.position,
            firePoint.rotation
        );

        float timeUntilNextShot = source
            .GetComponent<WeaponHandler>()
            .CurrentWeapon.FireConfig.fireRate;
        GameObject.Destroy(muzzleFlash, timeUntilNextShot);
    }

    public static void HandleOnHitEffect(GameObject onHitVFXPrefab, Transform transform)
    {
        GameObject onHitVFX = GameObject.Instantiate(
            onHitVFXPrefab,
            transform.position,
            transform.rotation
        );
        GameObject.Destroy(onHitVFX, 3f);
    }
}
