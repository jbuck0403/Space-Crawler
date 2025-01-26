using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DamageCollider : MonoBehaviour, IDamageReceiver
{
    private DamageHandler damageHandler;

    private void Awake()
    {
        damageHandler = GetComponent<DamageHandler>();
        if (damageHandler == null)
        {
            Debug.LogError($"No DamageHandler found on {gameObject.name}");
        }
    }

    public void ReceiveDamage(DamageData damageData)
    {
        if (damageHandler == null)
        {
            Debug.LogError($"Cannot receive damage on {gameObject.name} - DamageHandler is null");
            return;
        }
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
