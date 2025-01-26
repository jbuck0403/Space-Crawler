using UnityEngine;

public class DamageAOEZone : BaseAOEZone
{
    [SerializeField]
    private DamageTypeEvent onDamageOverTimeTick;

    public override void OnTargetEnter(AOEDamageReceiver target)
    {
        if (AOEData != null && AOEData.triggerOnEnter)
            AOEDamage(target);
    }

    public override void OnTargetStay(AOEDamageReceiver target)
    {
        if (AOEData != null && AOEData.triggerOverTime)
            AOEDamage(target);
    }

    public override void OnTargetExit(AOEDamageReceiver target)
    {
        if (AOEData != null && AOEData.triggerOnExit)
            AOEDamage(target);
    }

    private void AOEDamage(AOEDamageReceiver target)
    {
        if (CanTriggerEffect())
        {
            onDamageOverTimeTick.Raise(damageType);

            target.ReceiveDamage(damageData);
        }
    }
}
