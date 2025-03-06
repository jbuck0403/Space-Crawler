using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAOEZone : MonoBehaviour
{
    private readonly HashSet<AOEReceiver> activeReceivers = new HashSet<AOEReceiver>();

    [SerializeField]
    private readonly DamageProfile damageProfile;

    [SerializeField]
    private AOEData aoeData;

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
            DestroyZone();
        }
    }

    protected virtual void OnEffectExpired()
    {
        // override this in child classes if needed (explosions at the end, etc.)
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

    //use this for zones with destroyOnEnd=false that need manual cleanup.
    public void DestroyZone()
    {
        OnEffectExpired();
        Destroy(gameObject);
    }

    protected virtual void OnDestroy()
    {
        // clean up all receiver references when zone is destroyed
        foreach (var receiver in activeReceivers)
        {
            if (receiver != null)
            {
                receiver.OnZoneDestroyed(this);
            }
        }
    }

    public void OnTargetEnter(AOEReceiver target)
    {
        if (!IsInTargetLayer(target.gameObject))
            return;

        activeReceivers.Add(target);
        OnTargetEnterEffect(target);
    }

    public void OnTargetExit(AOEReceiver target)
    {
        if (!IsInTargetLayer(target.gameObject))
            return;

        OnTargetExitEffect(target);
    }

    public void OnTargetStay(AOEReceiver target)
    {
        if (!IsInTargetLayer(target.gameObject))
            return;

        OnTargetStayEffect(target);
    }

    protected abstract void OnTargetEnterEffect(AOEReceiver target);
    protected abstract void OnTargetExitEffect(AOEReceiver target);
    protected abstract void OnTargetStayEffect(AOEReceiver target);
}
