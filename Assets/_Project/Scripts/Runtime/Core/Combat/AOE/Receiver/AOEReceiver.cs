using System.Collections.Generic;
using UnityEngine;

public class AOEReceiver : BaseDamageReceiver
{
    // Track timing per zone AND if we're in the zone
    private Dictionary<BaseAOEZone, (float lastTickTime, bool isInZone)> zoneData =
        new Dictionary<BaseAOEZone, (float lastTickTime, bool isInZone)>();

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        var aoeZone = other.GetComponent<BaseAOEZone>();
        if (aoeZone != null && aoeZone.AOEData.triggerOnEnter)
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

            if (CanTriggerEffect(aoeZone))
            {
                aoeZone.OnTargetEnter(this);
            }
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        var aoeZone = other.GetComponent<BaseAOEZone>();
        if (aoeZone != null)
        {
            if (aoeZone.AOEData.triggerOnExit && CanTriggerEffect(aoeZone))
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
        if (aoeZone != null && aoeZone.AOEData.triggerOverTime && CanTriggerEffect(aoeZone))
        {
            aoeZone.OnTargetStay(this);
        }
    }

    public bool CanTriggerEffect(BaseAOEZone zone)
    {
        if (!zone.AOEData.triggerOverTime)
            return false;

        if (!zoneData.ContainsKey(zone))
        {
            zoneData[zone] = (Time.time, true);
            return true;
        }

        var (lastTickTime, isInZone) = zoneData[zone];
        if (Time.time >= lastTickTime + (1f / zone.AOEData.tickRate))
        {
            zoneData[zone] = (Time.time, isInZone);
            return true;
        }
        return false;
    }

    // Called when an AOE zone is destroyed
    public void OnZoneDestroyed(BaseAOEZone zone)
    {
        zoneData.Remove(zone);
    }
}
