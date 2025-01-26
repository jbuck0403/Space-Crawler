using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ProjectileDamageReceiver : BaseDamageReceiver
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Projectile>(out var projectile))
        {
            ReceiveDamage(projectile.damageData);
        }
    }
}
