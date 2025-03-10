// DefaultFiringStrategy.cs
using System.Collections;
using UnityEngine;

public class DefaultFiringStrategy : IFireStrategy
{
    private readonly FireConfig config;
    private bool isInitialized;
    private float nextFireTime;

    public DefaultFiringStrategy(FireConfig config)
    {
        this.config = config;

        nextFireTime = 0f;
    }

    public void OnEnter(BaseWeapon weapon, Transform self, Transform target)
    {
        isInitialized = true;
        nextFireTime = Time.time + Random.Range(0f, config.fireRate); // random initial delay
    }

    public void OnUpdate(BaseWeapon weapon, Transform self, Transform target)
    {
        if (!isInitialized || target == null || weapon == null)
            return;

        nextFireTime -= Time.deltaTime;

        float distanceToTarget = Vector2.Distance(self.position, target.position);

        // check if target is in firing range
        if (CanFire(config, distanceToTarget))
        {
            weapon.Fire();
        }
    }

    private bool CanFire(FireConfig config, float distanceToTarget)
    {
        return TargetInRange(config, distanceToTarget) && NextShotReady(config);
    }

    private bool TargetInRange(FireConfig config, float distanceToTarget)
    {
        return distanceToTarget <= config.maxFireRange && distanceToTarget >= config.minFireRange;
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

    public void OnExit()
    {
        isInitialized = false;
    }

    public void OnStrategyComplete() { }

    public FireConfig GetFireConfig() => config;
}
