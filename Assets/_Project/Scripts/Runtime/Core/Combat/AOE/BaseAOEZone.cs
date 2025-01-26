using UnityEngine;

public abstract class BaseAOEZone : MonoBehaviour
{
    [SerializeField]
    private readonly DamageProfile damageProfile;

    private readonly AOEData aoeData;

    protected float lastTickTime;
    protected float spawnTime;
    protected DamageType damageType;
    protected CircleCollider2D effectCollider;
    protected Transform owner;
    protected DamageData damageData;

    public AOEData AOEData => aoeData;

    public DamageData GetDamageData()
    {
        return damageData;
    }

    public static T Create<T>(
        Transform owner,
        Vector2 position,
        DamageData damageData,
        float? customRadius = null,
        float? customDuration = null
    )
        where T : BaseAOEZone
    {
        var zoneObject = new GameObject($"AOE_{typeof(T).Name}");
        var zone = zoneObject.AddComponent<T>();
        zone.Initialize(owner, position, damageData, customRadius, customDuration);
        return zone;
    }

    public void Initialize(
        Transform owner,
        Vector2 position,
        DamageData damageData,
        float? customRadius = null,
        float? customDuration = null
    )
    {
        this.owner = owner;
        transform.position = position;

        if (customRadius.HasValue)
            SetRadius(customRadius.Value);
        if (customDuration.HasValue)
            aoeData.duration = customDuration.Value;

        this.damageData = damageData;

        spawnTime = Time.time;
    }

    protected virtual void Awake()
    {
        effectCollider = gameObject.GetComponent<CircleCollider2D>();
        if (effectCollider == null)
            effectCollider = gameObject.AddComponent<CircleCollider2D>();

        effectCollider.isTrigger = true;
        effectCollider.radius = aoeData.radius;
        gameObject.layer = LayerMask.NameToLayer("AOE");
    }

    protected virtual void Update()
    {
        if (aoeData.destroyOnEnd && Time.time >= spawnTime + aoeData.duration)
        {
            OnEffectExpired();
            Destroy(gameObject);
        }
    }

    protected virtual void OnEffectExpired()
    {
        // Override this in child classes if needed
    }

    public bool CanTriggerEffect()
    {
        if (!aoeData.triggerOverTime)
            return false;

        if (Time.time >= lastTickTime + (1f / aoeData.tickRate))
        {
            lastTickTime = Time.time;
            return true;
        }
        return false;
    }

    protected bool IsInTargetLayer(GameObject obj)
    {
        return (aoeData.targetLayers.value & (1 << obj.layer)) != 0;
    }

    public void SetRadius(float newRadius)
    {
        aoeData.radius = newRadius;
        if (effectCollider != null)
            effectCollider.radius = aoeData.radius;
    }

    public Transform GetOwner() => owner;

    public abstract void OnTargetEnter(AOEDamageReceiver target);
    public abstract void OnTargetExit(AOEDamageReceiver target);
    public abstract void OnTargetStay(AOEDamageReceiver target);
}
