using UnityEngine;

[CreateAssetMenu(fileName = "ShotgunWeapon", menuName = "SpaceShooter/Weapon Types/Shotgun")]
public class ShotgunWeapon : BaseWeaponSO
{
    public override bool FireWeapon(
        Transform firePoint,
        Vector2 direction,
        Transform source,
        GameObject sourceObject,
        IProjectileDataProvider provider
    )
    {
        bool fired1 = base.FireWeapon(firePoint, direction, source, sourceObject, provider);
        bool fired2 = base.FireWeapon(firePoint, direction, source, sourceObject, provider);
        bool fired3 = base.FireWeapon(firePoint, direction, source, sourceObject, provider);

        UpdateNextFireTime();
        return fired1 && fired2 && fired3;
    }

    protected override void UniqueAbility(IWeaponAbilityDataProvider provider)
    {
        throw new System.NotImplementedException();
    }
}
