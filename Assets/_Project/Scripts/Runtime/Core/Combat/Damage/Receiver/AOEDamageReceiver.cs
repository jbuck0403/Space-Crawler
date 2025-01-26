using UnityEngine;

public class AOEDamageReceiver : BaseDamageReceiver
{
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        var aoeZone = other.GetComponent<BaseAOEZone>();
        if (aoeZone != null && aoeZone.AOEData.triggerOnEnter)
        {
            // ReceiveDamage(aoeZone.GetDamageData());
            aoeZone.OnTargetEnter(this);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        var aoeZone = other.GetComponent<BaseAOEZone>();
        if (aoeZone != null && aoeZone.AOEData.triggerOnExit)
        {
            // ReceiveDamage(aoeZone.GetDamageData());
            aoeZone.OnTargetEnter(this);
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        var aoeZone = other.GetComponent<BaseAOEZone>();
        if (aoeZone != null && aoeZone.AOEData.triggerOverTime && aoeZone.CanTriggerEffect())
        {
            // ReceiveDamage(aoeZone.GetDamageData());
            aoeZone.OnTargetEnter(this);
        }
    }
}
