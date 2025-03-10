using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField]
    float selfDestructTime = 0f;

    public DamageData damageData;

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

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(selfDestructTime);

        DestroyProjectile();
    }

    public void Initialize(PoolBase bulletPool)
    {
        pool = bulletPool;
    }

    public void DestroyProjectile()
    {
        if (pool != null)
        {
            damageData = new DamageData();
            pool.ReturnToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
