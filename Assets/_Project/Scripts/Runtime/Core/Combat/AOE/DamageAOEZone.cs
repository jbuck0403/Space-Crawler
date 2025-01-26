using UnityEngine;

public class DamageAOEZone : BaseAOEZone
{
    [SerializeField]
    private DamageTypeEvent onDamageOverTimeTick;

    protected override void ApplyEffect(Collider2D target)
    {
        onDamageOverTimeTick.Raise(damageType);

        // rest of code
    }
}
