// DefaultFiringStrategy.cs
using UnityEngine;

[CreateAssetMenu(
    fileName = "DefaultFiringStrategy",
    menuName = "SpaceShooter/Strategies/Firing Strategies/Default"
)]
public class BaseFiringStrategy : ScriptableObject, IFireStrategy
{
    [SerializeField]
    private bool initialDelay = true;
    private bool isInitialized;
    private float nextFireTime;

    private FireConfig config;

    public virtual void OnEnter(BaseWeapon weapon, FireConfig config)
    {
        isInitialized = true;
        this.config = config;

        if (initialDelay)
            nextFireTime = Time.time + Random.Range(0f, config.fireRate); // random initial delay
    }

    public virtual void OnUpdate(BaseWeapon weapon, FireConfig config)
    {
        if (!isInitialized || weapon == null)
            return;

        nextFireTime -= Time.deltaTime;

        if (CanFire(config))
        {
            // Use the weapon's forward direction (right in 2D space)
            Vector2 direction = weapon.transform.up;
            FireWeapon(weapon, direction);
        }
    }

    public virtual void OnExit()
    {
        isInitialized = false;
    }

    public void OnStrategyComplete() { }

    public FireConfig GetFireConfig() => config;

    protected virtual void FireWeapon(BaseWeapon weapon, Vector2? direction = null)
    {
        weapon.FireWeapon(direction);
    }

    protected virtual bool CanFire(FireConfig config)
    {
        return NextShotReady(config);
    }

    private bool NextShotReady(FireConfig config)
    {
        bool ready = nextFireTime <= 0f;
        if (ready)
        {
            nextFireTime = config.fireRate;
        }
        return ready;
    }
}
