using UnityEngine;

public abstract class BaseWeapon : StrategyController<IFireStrategy>
{
    [Header("Projectile Settings")]
    [SerializeField]
    private ProjectilePool projectilePool;

    [SerializeField]
    private DamageProfile damageProfile;

    [SerializeField]
    private Transform firePoint;

    [SerializeField]
    private float projectileSpeed = 10f;

    [SerializeField]
    private VoidEvent OnFire;

    [Header("Weapon Settings")]
    [SerializeField]
    private float fireRate = 0.5f;

    private BaseEnemyController enemyController;

    private void Start()
    {
        enemyController = GetComponent<BaseEnemyController>();
    }

    public bool Fire(Vector2? direction = null)
    {
        // get the fire direction
        Vector2 finalDirection = direction ?? Vector2.right; // assuming the weapon points along its right axis

        // fire the projectile
        Projectile projectile = FireProjectile.Fire(
            projectilePool,
            damageProfile,
            firePoint != null ? firePoint : transform,
            finalDirection,
            projectileSpeed,
            transform
        );

        OnFire.Raise(); // trigger OnFire subscribers

        return projectile != null;
    }

    protected override void OnStrategyExit(IFireStrategy strategy)
    {
        strategy.OnExit();
    }

    protected override void OnStrategyEnter(IFireStrategy strategy)
    {
        strategy.OnEnter(this, firePoint, enemyController.GetTarget());
    }

    protected override void OnStrategyUpdate(IFireStrategy strategy)
    {
        strategy.OnUpdate(this, firePoint, enemyController.GetTarget());
    }
}
