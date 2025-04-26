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

    // private PoolBase pool;

    private int behaviorsCleanedUp = 0;
    private int totalBehaviors = 0;
    private IProjectileBehavior[] behaviors;
    private GameObject onHitVFXPrefab;

    protected virtual IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(selfDestructTime);

        DestroyProjectile();
    }

    public virtual void Initialize(GameObject onHitVFXPrefab, DamageData damageData = default)
    {
        this.onHitVFXPrefab = onHitVFXPrefab;
        this.damageData = damageData;
    }

    public virtual void CanonizeBehaviors()
    {
        behaviors = GetComponents<IProjectileBehavior>();
        totalBehaviors = behaviors.Length;
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

    public virtual void OnHit(GameObject hitGameObject)
    {
        damageData.ApplyAllStatusEffects(hitGameObject);

        if (totalBehaviors > 0)
        {
            foreach (var behavior in behaviors)
            {
                behavior.OnHit();
            }
        }

        WeaponVFXHandler.HandleOnHitEffect(onHitVFXPrefab, transform);

        hasDealtDamage = true;
    }

    private void HandleDestroyProjectile()
    {
        Destroy(gameObject);
    }
}
