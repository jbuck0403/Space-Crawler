using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField]
    float selfDestructTime = 0f;

    [SerializeField]
    public DamageData damageData;

    public bool hasDealtDamage;

    private PoolBase pool;

    private int behaviorsCleanedUp = 0;
    private int totalBehaviors = 0;

    protected virtual IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(selfDestructTime);

        DestroyProjectile();
    }

    public virtual void Initialize(PoolBase pool, DamageData damageData = default)
    {
        this.pool = pool;
        this.damageData = damageData;
    }

    private void OnEnable()
    {
        if (selfDestructTime != 0f)
        {
            StartCoroutine(SelfDestruct());
        }
    }

    public virtual void DestroyProjectile()
    {
        gameObject.SetActive(false);
        behaviorsCleanedUp = 0;

        var behaviors = GetComponents<IProjectileBehavior>();
        totalBehaviors = behaviors.Length;

        // if no behaviors to clean up, destroy immediately
        if (totalBehaviors == 0)
        {
            HandleDestroyProjectile();
            return;
        }

        // clean up all behaviors
        foreach (var behavior in behaviors)
        {
            behavior.OnCleanupComplete += OnBehaviorCleanupComplete;
            behavior.Cleanup();
        }
    }

    private void OnBehaviorCleanupComplete()
    {
        behaviorsCleanedUp++;

        // only destroy when all behaviors are cleaned up
        if (behaviorsCleanedUp == totalBehaviors)
        {
            HandleDestroyProjectile();
        }
    }

    public virtual void OnHit(Collider2D other)
    {
        hasDealtDamage = true;
    }

    private void HandleDestroyProjectile()
    {
        if (pool != null)
        {
            hasDealtDamage = false;
            damageData = new DamageData();
            pool.ReturnToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
