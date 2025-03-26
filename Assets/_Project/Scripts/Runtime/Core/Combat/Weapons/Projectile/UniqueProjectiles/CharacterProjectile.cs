using System.Collections;

public class CharacterProjectile : Projectile
{
    public override void DestroyProjectile() { }

    public void Initialize(DamageData damageData)
    {
        hasDealtDamage = false;
        this.damageData = damageData;
    }

    protected override IEnumerator SelfDestruct()
    {
        yield return null;
    }
}
