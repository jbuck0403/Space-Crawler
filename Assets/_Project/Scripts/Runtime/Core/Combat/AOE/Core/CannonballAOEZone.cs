public class CannonballAOEZone : DamageAOEZone
{
    protected override void OnTargetStayEffect(AOEReceiver target) { }

    protected override void OnTargetExitEffect(AOEReceiver target) { }

    protected override void AOEDamage(AOEReceiver target)
    {
        target.ReceiveDamage(damageData);
        // launch player
    }
}
