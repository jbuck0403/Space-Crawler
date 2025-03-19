using System.Collections.Generic;
using UnityEngine;

public class AOEReceiver : BaseDamageReceiver
{
    // Track timing per zone AND if we're in the zone
    private Dictionary<BaseAOEZone, (float lastTickTime, bool isInZone)> zoneData =
        new Dictionary<BaseAOEZone, (float lastTickTime, bool isInZone)>();

    private void Start()
    {
        // Check for required components
        var healthSystem = GetComponent<HealthSystem>();
        var defenseHandler = GetComponent<IDefenseHandler>();
        var damageHandler = GetComponent<DamageHandler>();

        if (healthSystem == null || defenseHandler == null || damageHandler == null)
        {
            // Error handling instead of logging
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
            // Just update the zone state if it already exists
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
                // Make sure we have zone data for this zone - initialize if needed
                if (!zoneData.ContainsKey(aoeZone))
                {
                    zoneData[aoeZone] = (Time.time, true);
                }

                aoeZone.OnTargetStay(this);
            }
        }
    }

    // Checks if the effect is allowed to trigger based on zone configuration
    public bool CanTriggerEffect(BaseAOEZone zone)
    {
        // Case 1: First interaction with this zone (no timing data yet)
        if (!zoneData.ContainsKey(zone))
        {
            // Set up initial timing data
            zoneData[zone] = (Time.time, true);

            // For initial entry, check zone settings
            return zone.AOEData.triggerOnEnter;
        }

        // Get stored data for this zone
        var (lastTickTime, isInZone) = zoneData[zone];

        // Case 2: Player not in zone, can't trigger
        if (!isInZone)
            return false;

        // Case 3: Regular tick check for over-time effects
        if (zone.AOEData.triggerOverTime)
        {
            float timeSinceLastTick = Time.time - lastTickTime;
            float tickInterval = 1f / zone.AOEData.tickRate;

            // Is it time for next tick?
            if (timeSinceLastTick >= tickInterval)
            {
                // Update last tick time
                zoneData[zone] = (Time.time, true);
                return true;
            }
        }

        // Default: not ready yet
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
