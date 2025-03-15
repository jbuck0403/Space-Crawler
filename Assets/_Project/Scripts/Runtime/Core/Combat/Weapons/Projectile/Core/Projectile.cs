using System.Collections;
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

    public Projectile(DamageData damageData)
    {
        this.damageData = damageData;
    }

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

    public virtual void Initialize(PoolBase bulletPool)
    {
        pool = bulletPool;
        hasDealtDamage = false;
    }

    public virtual void DestroyProjectile()
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

    public virtual void OnHit(Collider2D other)
    {
        hasDealtDamage = true;
    }
}
