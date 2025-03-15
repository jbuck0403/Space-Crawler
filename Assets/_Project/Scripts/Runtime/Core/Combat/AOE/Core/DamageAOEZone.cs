using UnityEngine;

public class DamageAOEZone : BaseAOEZone
{
    [SerializeField]
    private DamageTypeEvent onDamageOverTimeTick;

    protected override void OnTargetEnterEffect(AOEReceiver target)
    {
        if (AOEData != null && AOEData.triggerOnEnter)
            AOEDamage(target);
    }

    protected override void OnTargetStayEffect(AOEReceiver target)
    {
        if (AOEData != null && AOEData.triggerOverTime)
            AOEDamage(target);
    }

    protected override void OnTargetExitEffect(AOEReceiver target)
    {
        if (AOEData != null && AOEData.triggerOnExit)
            AOEDamage(target);
    }

    protected virtual void AOEDamage(AOEReceiver target)
    {
        if (target.CanTriggerEffect(this))
        {
            onDamageOverTimeTick.Raise(gameObject, damageType);

            target.ReceiveDamage(damageData);
        }
    }
}
