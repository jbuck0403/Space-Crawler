using UnityEngine;

[CreateAssetMenu(fileName = "SniperWeapon", menuName = "SpaceShooter/Weapon Types/Sniper")]
public class SniperWeapon : BaseWeaponSO
{
    public override bool FireWeapon(
        Transform firePoint,
        Vector2 direction,
        Transform source,
        GameObject sourceObject,
        IProjectileDataProvider provider
    )
    {
        bool fired = base.FireWeapon(firePoint, direction, source, sourceObject, provider);
        UpdateNextFireTime();
        return fired;
    }

    protected override void UniqueAbility()
    {
        throw new System.NotImplementedException();
    }
}
