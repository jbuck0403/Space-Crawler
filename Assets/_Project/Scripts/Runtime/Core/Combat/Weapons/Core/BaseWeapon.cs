using System;
using UnityEngine;

public abstract class BaseWeapon : StrategyController<IFireStrategy>, IFireWeapon
{
    [SerializeField]
    public WeaponConfig weaponConfig;

    [SerializeField]
    public Transform firePoint;

    [SerializeField]
    public VoidEvent OnFireWeapon;

    public ProjectilePool projectilePool { private set; get; }

    private bool canFire = false;

    protected virtual void Start()
    {
        projectilePool = FindAnyObjectByType<ProjectilePool>();
    }

    public virtual bool FireWeapon(Vector2? direction = null)
    {
        // get the fire direction
        Vector2 finalDirection = direction ?? Vector2.right; // assuming the weapon points along its right axis

        // fire the projectile
        Projectile projectile = FireProjectile.Fire(
            projectilePool,
            weaponConfig.damageProfile,
            firePoint != null ? firePoint : transform,
            finalDirection,
            weaponConfig.fireConfig.projectileSpeed,
            transform
        );

        OnFireWeapon.Raise(); // trigger OnFire subscribers

        return projectile != null;
    }

    public virtual void SetCanFire(bool canFire)
    {
        this.canFire = canFire;
    }

    protected override void OnStrategyExit(IFireStrategy strategy)
    {
        strategy.OnExit();
    }

    protected override void OnStrategyEnter(IFireStrategy strategy)
    {
        strategy.OnEnter(this, weaponConfig.fireConfig);
    }

    protected override void OnStrategyUpdate(IFireStrategy strategy)
    {
        if (canFire)
            strategy.OnUpdate(this, weaponConfig.fireConfig);
    }
}
