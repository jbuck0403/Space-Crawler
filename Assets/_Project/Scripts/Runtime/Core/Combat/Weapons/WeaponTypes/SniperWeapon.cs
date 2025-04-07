using UnityEngine;

[CreateAssetMenu(fileName = "SniperWeapon", menuName = "SpaceShooter/Weapon Types/Sniper")]
public class SniperWeapon : BaseWeaponSO
{
    [Header("Charge Settings")]
    [SerializeField]
    private float minChargeTime = 2f;

    [SerializeField]
    private float maxChargeTime = 4f;

    [SerializeField]
    private float autoFireTime = 6f;

    [SerializeField]
    private float maxDamageModifier = 3f;

    [SerializeField]
    private float maxVelocityModifier = 3f;

    private float chargeStartTime = 0f;
    private bool isCharging = false;
    private float chargeProgress = 0f;

    private float originalDamageModifier;
    private float originalVelocityModifier;
    private float originalAccuracy;

    // public delegate void ChargeUpdatedHandler(float chargePercent);
    // public event ChargeUpdatedHandler OnChargeUpdated;

    public override bool FireWeapon(
        Transform firePoint,
        Vector2 direction,
        Transform source,
        GameObject sourceObject,
        IProjectileDataProvider provider
    )
    {
        if (!isCharging)
        {
            chargeStartTime = Time.time;
            isCharging = true;
            chargeProgress = 0f;
        }

        return true;
    }

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

        if (Input.GetMouseButtonUp(0))
        {
            return FireChargedShot(firePoint, direction, source, sourceObject, provider);
        }

        float chargeTime = Time.time - chargeStartTime;

        if (chargeTime >= minChargeTime && chargeTime <= maxChargeTime)
        {
            chargeProgress = Mathf.Clamp01(
                (chargeTime - minChargeTime) / (maxChargeTime - minChargeTime)
            );
        }

        if (chargeTime >= autoFireTime)
        {
            return FireChargedShot(firePoint, direction, source, sourceObject, provider);
        }

        return false;
    }

    private bool FireChargedShot(
        Transform firePoint,
        Vector2 direction,
        Transform source,
        GameObject sourceObject,
        IProjectileDataProvider provider
    )
    {
        float chargeTime = Time.time - chargeStartTime;
        isCharging = false;

        if (chargeTime < minChargeTime)
        {
            chargeProgress = 0f;
            return false;
        }

        originalDamageModifier = damageModifier;
        originalVelocityModifier = velocityModifier;
        originalAccuracy = fireConfig.accuracy;

        damageModifier = originalDamageModifier * (1f + chargeProgress * (maxDamageModifier - 1f));
        velocityModifier =
            originalVelocityModifier * (1f + chargeProgress * (maxVelocityModifier - 1f));
        fireConfig.accuracy = Mathf.Lerp(originalAccuracy, 1f, chargeProgress);

        bool fired = base.FireWeapon(firePoint, direction, source, sourceObject, provider);

        damageModifier = originalDamageModifier;
        velocityModifier = originalVelocityModifier;
        fireConfig.accuracy = originalAccuracy;

        if (fired)
        {
            UpdateNextFireTime();
        }

        chargeProgress = 0f;
        return fired;
    }

    protected override void UniqueAbility(IWeaponAbilityDataProvider provider) { }
}
