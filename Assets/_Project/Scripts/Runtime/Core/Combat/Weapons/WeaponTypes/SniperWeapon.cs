using UnityEngine;

[CreateAssetMenu(fileName = "SniperWeapon", menuName = "SpaceShooter/Weapon Types/Sniper")]
public class SniperWeapon : BaseWeaponSO
{
    [Header("Charge Settings")]
    [SerializeField]
    private float minFullChargeTime = 2f; // Time to reach 100% charge

    [SerializeField]
    private float maxChargeTime = 4f; // Time to reach max charge (300%)

    // State tracking
    private bool isCharging = false;
    private float chargeStartTime = 0f;
    private float chargePercent = 0f;

    // Called on initial mouse press
    public override bool FireWeapon(
        Transform firePoint,
        Vector2 direction,
        Transform source,
        GameObject sourceObject,
        IProjectileDataProvider provider
    )
    {
        // Start charging
        isCharging = true;
        chargeStartTime = Time.time;
        Debug.Log("SNIPER: Started charging");
        return true;
    }

    // Called each frame while mouse button is held
    public override bool UpdateFireWeapon(
        Transform firePoint,
        Vector2 direction,
        Transform source,
        GameObject sourceObject,
        IProjectileDataProvider provider
    )
    {
        if (!isCharging)
            return false;

        float chargeDuration = Time.time - chargeStartTime;
        chargePercent = CalculateChargePercent(chargeDuration);

        if (weaponHandler != null && weaponHandler.OnChargingWeapon != null)
        {
            float uiCharge = Mathf.Clamp01(chargeDuration / maxChargeTime);
            weaponHandler.OnChargingWeapon.Raise(sourceObject, uiCharge);
        }

        Debug.Log($"SNIPER: Charging... {chargePercent * 100:0}%");
        return false; // We never fire during update
    }

    // Simple charge calculation
    private float CalculateChargePercent(float chargeDuration)
    {
        if (chargeDuration < minFullChargeTime)
        {
            // Always 100% for the first minFullChargeTime seconds
            return 1.0f;
        }
        else if (chargeDuration < maxChargeTime)
        {
            // 100% to 300% between minFullChargeTime and maxChargeTime
            float overchargePercent =
                (chargeDuration - minFullChargeTime) / (maxChargeTime - minFullChargeTime);
            return 1.0f + 2.0f * overchargePercent;
        }
        else
        {
            // Max at 300%
            return 3.0f;
        }
    }

    public override void NotifyWeaponStoppedFiring()
    {
        if (isCharging && weaponHandler != null && CanFire())
        {
            Debug.Log($"SNIPER: FIRED with charge {chargePercent * 100:0}%");

            float originalDamageModifier = damageModifier;
            float originalVelocityModifier = velocityModifier;

            damageModifier *= chargePercent;
            velocityModifier *= chargePercent;

            Debug.Log(
                $"SNIPER: Damage multiplier: {damageModifier:0.00}x, Velocity multiplier: {velocityModifier:0.00}x"
            );

            Transform firePoint = weaponHandler.FirePoint;
            Vector2 direction = weaponHandler.transform.up;
            Transform source = weaponHandler.transform;
            GameObject sourceObject = weaponHandler.gameObject;

            bool fired = base.FireWeapon(firePoint, direction, source, sourceObject, weaponHandler);
            if (fired)
                weaponHandler.RaiseOnFireWeaponEvent();

            damageModifier = originalDamageModifier;
            velocityModifier = originalVelocityModifier;

            UpdateNextFireTime(sourceObject);

            isCharging = false;
        }
    }

    protected override void UniqueAbility(IWeaponAbilityDataProvider provider)
    {
        Debug.Log("SNIPER: Ability triggered");
    }
}
