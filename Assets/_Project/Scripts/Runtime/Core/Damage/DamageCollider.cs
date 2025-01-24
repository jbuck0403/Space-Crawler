using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DamageCollider : MonoBehaviour, IDamageReceiver
{
    private DamageHandler damageHandler;

    private void Awake()
    {
        damageHandler = GetComponent<DamageHandler>();
    }

    public void ReceiveDamage(DamageData damageData)
    {
        damageHandler.HandleDamage(damageData);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Projectile>(out var projectile))
        {
            ReceiveDamage(projectile.damageData);
        }
    }
}
