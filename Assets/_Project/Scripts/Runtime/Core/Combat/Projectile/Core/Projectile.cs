using UnityEngine;

public class Projectile : MonoBehaviour
{
    public readonly DamageData damageData;

    public Projectile(DamageData damageData)
    {
        this.damageData = damageData;
    }
}
