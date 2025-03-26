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

    void Start()
    {
        if (selfDestructTime != 0f)
            StartCoroutine(SelfDestruct());
    }

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

    public virtual void DestroyProjectile()
    {
        // // clean up any behaviors attached to this projectile
        // foreach (var behavior in GetComponents<IProjectileBehavior>())
        // {
        //     behavior.Cleanup();
        // }

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

    public virtual void OnHit(Collider2D other)
    {
        hasDealtDamage = true;
    }
}
