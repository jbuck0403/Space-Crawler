using UnityEngine;

public class DamageAOEZone : BaseAOEZone
{
    protected override void OnTargetEnterEffect(AOEReceiver target) { }

    protected override void OnTargetStayEffect(AOEReceiver target)
    {
        if (AOEData != null && AOEData.triggerOverTime)
        {
            AOEDamage(target);
        }
    }

    protected override void OnTargetExitEffect(AOEReceiver target) { }

    protected virtual void AOEDamage(AOEReceiver target)
    {
        if (target.CanTriggerEffect(this))
        {
            try
            {
                aoeProfile.onAOETick.Raise(gameObject, damageData.Type);
            }
            catch (System.Exception)
            {
                // Exception handling
            }

            try
            {
                if (aoeData.applyDamage)
                    target.ReceiveDamage(damageData);

                if (aoeData.applyEffects)
                    damageData.ApplyAllStatusEffects(target.gameObject);
            }
            catch (System.Exception)
            {
                // Exception handling
            }
        }
    }
}
