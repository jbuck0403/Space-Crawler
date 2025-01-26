using UnityEngine;

public abstract class BaseAOEZone : MonoBehaviour
{
    [Header("Effect Settings")]
    [SerializeField]
    protected float radius = 5f;

    [SerializeField]
    protected bool triggerOnEnter = true;

    [SerializeField]
    protected bool triggerOnExit = false;

    [SerializeField]
    protected bool triggerOverTime = false;

    [SerializeField]
    protected float tickRate = 1f;

    [SerializeField]
    protected LayerMask targetLayers;

    [SerializeField]
    protected float duration = 2f;

    [SerializeField]
    protected bool destroyOnEnd = true;

    protected float lastTickTime;
    protected float spawnTime;
    protected DamageType damageType;
    protected CircleCollider2D effectCollider;
    protected Transform owner;

    public void Initialize(
        Transform owner,
        Vector2 position,
        DamageType damageType,
        float? customRadius = null,
        float? customDuration = null
    )
    {
        this.owner = owner;
        transform.position = position;

        if (customRadius.HasValue)
            SetRadius(customRadius.Value);
        if (customDuration.HasValue)
            duration = customDuration.Value;

        this.damageType = damageType;

        spawnTime = Time.time;
    }

    protected virtual void Awake()
    {
        effectCollider = gameObject.GetComponent<CircleCollider2D>();
        if (effectCollider == null)
            effectCollider = gameObject.AddComponent<CircleCollider2D>();

        effectCollider.isTrigger = true;
        effectCollider.radius = radius;
        gameObject.layer = LayerMask.NameToLayer("AOE");
    }

    protected virtual void Update()
    {
        if (destroyOnEnd && Time.time >= spawnTime + duration)
        {
            OnEffectExpired();
            Destroy(gameObject);
        }
    }

    protected virtual void OnEffectExpired()
    {
        // Override this in child classes if needed
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnEnter && IsInTargetLayer(other.gameObject))
            ApplyEffect(other);
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (triggerOnExit && IsInTargetLayer(other.gameObject))
            ApplyEffect(other);
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if (triggerOverTime && CanTriggerEffect() && IsInTargetLayer(other.gameObject))
            ApplyEffect(other);
    }

    protected bool CanTriggerEffect()
    {
        if (!triggerOverTime)
            return false;

        if (Time.time >= lastTickTime + (1f / tickRate))
        {
            lastTickTime = Time.time;
            return true;
        }
        return false;
    }

    protected bool IsInTargetLayer(GameObject obj)
    {
        return (targetLayers.value & (1 << obj.layer)) != 0;
    }

    public void SetRadius(float newRadius)
    {
        radius = newRadius;
        if (effectCollider != null)
            effectCollider.radius = radius;
    }

    public Transform GetOwner() => owner;

    // can be anything that will happen when standing in the zone (burning, healing, etc.)
    protected abstract void ApplyEffect(Collider2D target);
}
