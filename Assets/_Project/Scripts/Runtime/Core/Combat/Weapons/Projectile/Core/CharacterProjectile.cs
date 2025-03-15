using System.Collections;
using UnityEngine;

public class CharacterProjectile : Projectile
{
    public CharacterProjectile(DamageData damageData)
        : base(damageData)
    {
        this.damageData = damageData;
    }

    public override void DestroyProjectile() { }

    public void Initialize(DamageData damageData)
    {
        hasDealtDamage = false;
        this.damageData = damageData;
    }

    protected override IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(0f);
    }

    public override void OnHit(Collider2D other)
    {
        hasDealtDamage = true;
    }
}
