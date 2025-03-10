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
        Vector2 finalDirection = direction ?? Vector2.up; // assuming the weapon points along its right axis
        Vector2 directionAfterAccuracy = ApplyAccuracySpread(
            finalDirection,
            weaponConfig.fireConfig.spread
        );

        // fire the projectile
        Projectile projectile = FireProjectile.Fire(
            projectilePool,
            weaponConfig.damageProfile,
            firePoint != null ? firePoint : transform,
            directionAfterAccuracy,
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

    private Vector2 ApplyAccuracySpread(Vector2 baseDirection, float spreadDegrees)
    {
        if (weaponConfig.fireConfig.accuracy >= 1f)
            return baseDirection;

        float maxSpreadRadians =
            ConvertSpreadDegreesToRadians(spreadDegrees) * (1f - weaponConfig.fireConfig.accuracy);

        // generate a random angle within our spread cone
        float randomSpread = UnityEngine.Random.Range(
            -maxSpreadRadians / 2f,
            maxSpreadRadians / 2f
        );

        // rotate our base direction by the spread amount
        float cos = Mathf.Cos(randomSpread);
        float sin = Mathf.Sin(randomSpread);
        return new Vector2(
            baseDirection.x * cos - baseDirection.y * sin,
            baseDirection.x * sin + baseDirection.y * cos
        );
    }

    private float ConvertSpreadDegreesToRadians(float spreadDegrees)
    {
        // Clamp the input to valid range
        spreadDegrees = Mathf.Clamp(spreadDegrees, 0f, 180f);

        // 180 degrees = PI radians
        return spreadDegrees * Mathf.PI / 180f;
    }
}
