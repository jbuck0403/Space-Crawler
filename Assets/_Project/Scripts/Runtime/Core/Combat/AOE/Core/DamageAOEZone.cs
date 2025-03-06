using UnityEngine;

public class DamageAOEZone : BaseAOEZone
{
    [SerializeField]
    private DamageTypeEvent onDamageOverTimeTick;

    protected override void OnTargetEnterEffect(AOEDamageReceiver target)
    {
        if (AOEData != null && AOEData.triggerOnEnter)
            AOEDamage(target);
    }

    protected override void OnTargetStayEffect(AOEDamageReceiver target)
    {
        if (AOEData != null && AOEData.triggerOverTime)
            AOEDamage(target);
    }

    protected override void OnTargetExitEffect(AOEDamageReceiver target)
    {
        if (AOEData != null && AOEData.triggerOnExit)
            AOEDamage(target);
    }

    private void AOEDamage(AOEDamageReceiver target)
    {
        if (target.CanTriggerEffect(this))
        {
            onDamageOverTimeTick.Raise(damageType);

            target.ReceiveDamage(damageData);
        }
    }
}
