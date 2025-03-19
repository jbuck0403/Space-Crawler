using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAOEZone : MonoBehaviour
{
    private readonly HashSet<AOEReceiver> activeReceivers = new HashSet<AOEReceiver>();

    [SerializeField]
    public AOEProfile aoeProfile;

    protected float spawnTime;

    protected CircleCollider2D effectCollider;
    protected Transform owner;

    protected DamageData damageData;

    protected AOEData aoeData;

    public AOEData AOEData => aoeData;

    protected virtual void Start()
    {
        Initialize(transform, transform.position, aoeProfile);
        InitCollider();

        CheckExistingTargetsInZone();
    }

    public DamageData GetDamageData()
    {
        return damageData;
    }

    public static T Create<T>(
        Transform owner,
        Vector2 position,
        AOEProfile aoeProfile,
        float? customRadius = null,
        float? customDuration = null
    )
        where T : BaseAOEZone
    {
        var zoneObject = new GameObject($"AOE_{typeof(T).Name}");
        var zone = zoneObject.AddComponent<T>();
        zone.Initialize(owner, position, aoeProfile, customRadius, customDuration);

        // zone.AddAOEComponents(zoneObject);

        return zone;
    }

    // private void AddAOEComponents(GameObject aoeZoneObject)
    // {
    //     aoeZoneObject.AddComponent<AOEVisualizer>();

    //     if (aoeProfile.followTarget)
    //     {
    //         AOEController controller = aoeZoneObject.AddComponent<AOEController>();

    //         if (controller != null)
    //         {
    //             // add basemovement controller
    //             // init controller with target, etc.
    //         }
    //     }
    // }

    public void Initialize(
        Transform owner,
        Vector2 position,
        AOEProfile aoeProfile,
        float? customRadius = null,
        float? customDuration = null
    )
    {
        this.owner = owner;
        transform.position = position;
        this.aoeProfile = aoeProfile;

        if (aoeProfile.damageProfile == null)
        {
            Debug.LogError($"[AOE] DamageProfile is null in AOEProfile '{aoeProfile.name}'");
            return;
        }

        damageData = aoeProfile.damageProfile.CreateDamageData(owner);

        aoeData = aoeProfile.aoeData;
        if (aoeData == null)
        {
            Debug.LogError($"[AOE] AOEData is null in AOEProfile '{aoeProfile.name}'");
            return;
        }

        if (customRadius.HasValue)
        {
            SetRadius(customRadius.Value);
        }

        if (customDuration.HasValue)
        {
            aoeData.duration = customDuration.Value;
        }

        spawnTime = Time.time;

        InitCollider();
    }

    private string LayerMaskToString(LayerMask mask)
    {
        var layers = new System.Text.StringBuilder();
        for (int i = 0; i < 32; i++)
        {
            if ((mask.value & (1 << i)) != 0)
            {
                layers.Append(LayerMask.LayerToName(i)).Append(", ");
            }
        }
        return layers.Length > 0 ? layers.ToString(0, layers.Length - 2) : "None";
    }

    private void InitCollider()
    {
        effectCollider = gameObject.GetComponent<CircleCollider2D>();
        if (effectCollider == null)
            effectCollider = gameObject.AddComponent<CircleCollider2D>();

        effectCollider.isTrigger = true;
        effectCollider.radius = AOEData.radius;
    }

    protected virtual void CheckExistingTargetsInZone()
    {
        if (effectCollider == null || AOEData == null)
        {
            Debug.LogError(
                $"[AOE] Cannot check for targets: effectCollider = {effectCollider == null} or AOEData = {aoeData == null}"
            );
            return;
        }

        // find all colliders within the AOE radius that match the target layers
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
            transform.position,
            AOEData.radius,
            AOEData.targetLayers
        );

        foreach (var collider in hitColliders)
        {
            var receiver = collider.GetComponent<AOEReceiver>();
            if (receiver != null)
            {
                if (!activeReceivers.Contains(receiver))
                {
                    // Process the target as if it just entered the zone
                    if (AOEData.triggerOnEnter)
                    {
                        receiver.OnZoneSpawnedOnTarget(this);
                        OnTargetEnter(receiver);
                    }
                }
            }
        }
    }

    protected virtual void Update()
    {
        if (AOEData != null && AOEData.destroyOnEnd && Time.time >= spawnTime + AOEData.duration)
        {
            DestroyZone();
        }
    }

    protected bool IsInTargetLayer(GameObject obj)
    {
        return (AOEData.targetLayers.value & (1 << obj.layer)) != 0;
    }

    public void SetRadius(float newRadius)
    {
        AOEData.radius = newRadius;
        if (effectCollider != null)
            effectCollider.radius = AOEData.radius;
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
        {
            return;
        }

        activeReceivers.Add(target);
        OnTargetEnterEffect(target);
    }

    public void OnTargetExit(AOEReceiver target)
    {
        if (!IsInTargetLayer(target.gameObject))
        {
            return;
        }

        OnTargetExitEffect(target);
    }

    public void OnTargetStay(AOEReceiver target)
    {
        if (!IsInTargetLayer(target.gameObject))
            return;

        OnTargetStayEffect(target);
    }

    protected virtual void OnEffectExpired() { }

    protected abstract void OnTargetEnterEffect(AOEReceiver target);
    protected abstract void OnTargetExitEffect(AOEReceiver target);
    protected abstract void OnTargetStayEffect(AOEReceiver target);
}
