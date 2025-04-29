using System.Collections.Generic;
using UnityEngine;

public class AOEReceiver : BaseDamageReceiver
{
    // Track timing per zone AND if we're in the zone
    private Dictionary<BaseAOEZone, (float lastTickTime, bool isInZone)> zoneData =
        new Dictionary<BaseAOEZone, (float lastTickTime, bool isInZone)>();

    private void Start()
    {
        var healthSystem = GetComponent<HealthSystem>();
        var defenseHandler = GetComponent<IDefenseHandler>();
        var damageHandler = GetComponent<DamageHandler>();

        if (healthSystem == null || defenseHandler == null || damageHandler == null)
        {
            // error handling instead of logging
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        var aoeZone = other.GetComponent<BaseAOEZone>();
        if (aoeZone != null)
        {
            if (aoeZone.AOEData.triggerOnEnter)
            {
                // if we don't have timing data, initialize it
                if (!zoneData.ContainsKey(aoeZone))
                {
                    zoneData[aoeZone] = (Time.time, true);
                }
                else
                {
                    // otherwise just mark as in zone but keep the timer
                    zoneData[aoeZone] = (zoneData[aoeZone].lastTickTime, true);
                }

                aoeZone.OnTargetEnter(this);
            }
        }
    }

    // Called when a zone is spawned directly on top of this receiver
    public void OnZoneSpawnedOnTarget(BaseAOEZone zone)
    {
        // Initialize timing data for this zone
        if (!zoneData.ContainsKey(zone))
        {
            zoneData[zone] = (Time.time, true);
        }
        else
        {
            // just update the zone state if it already exists
            zoneData[zone] = (zoneData[zone].lastTickTime, true);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        var aoeZone = other.GetComponent<BaseAOEZone>();
        if (aoeZone != null)
        {
            if (aoeZone.AOEData.triggerOnExit)
            {
                aoeZone.OnTargetExit(this);
            }

            // just mark as not in zone, keep the timer
            if (zoneData.ContainsKey(aoeZone))
            {
                zoneData[aoeZone] = (zoneData[aoeZone].lastTickTime, false);
            }
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        var aoeZone = other.GetComponent<BaseAOEZone>();
        if (aoeZone != null)
        {
            if (aoeZone.AOEData.triggerOverTime)
            {
                if (!zoneData.ContainsKey(aoeZone))
                {
                    zoneData[aoeZone] = (Time.time, true);
                }

                aoeZone.OnTargetStay(this);
            }
        }
    }

    public bool CanTriggerEffect(BaseAOEZone zone)
    {
        if (!zoneData.ContainsKey(zone))
        {
            zoneData[zone] = (Time.time, true);

            return zone.AOEData.triggerOnEnter;
        }

        var (lastTickTime, isInZone) = zoneData[zone];

        if (!isInZone)
            return false;

        if (zone.AOEData.triggerOverTime)
        {
            float timeSinceLastTick = Time.time - lastTickTime;
            float tickInterval = 1f / zone.AOEData.tickRate;

            if (timeSinceLastTick >= tickInterval)
            {
                zoneData[zone] = (Time.time, true);
                return true;
            }
        }

        return false;
    }

    public new void ReceiveDamage(DamageData damageData)
    {
        base.ReceiveDamage(damageData);
    }

    public void OnZoneDestroyed(BaseAOEZone zone)
    {
        zoneData.Remove(zone);
    }
}
