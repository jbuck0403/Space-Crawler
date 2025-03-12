using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 100f;

    [SerializeField]
    private float lowHealthPercent = 20f;

    [SerializeField]
    private float currentHealth;
    private bool isDead = false;

    [SerializeField]
    public FloatEvent OnHealthChanged;

    [SerializeField]
    public FloatEvent OnHealthPercentChanged;

    [SerializeField]
    public VoidEvent OnDamageTaken;

    [SerializeField]
    public VoidEvent OnHealingReceived;

    [SerializeField]
    public Vector2Event OnDeath;

    [SerializeField]
    public VoidEvent OnLowHealth;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    // positive amount heals, negative amount damages
    public void ModifyHealth(float amount)
    {
        if (currentHealth <= 0)
            return;

        if (amount > 0)
        {
            OnHealingReceived.Raise();
        }
        else if (amount < 0)
        {
            OnDamageTaken.Raise();
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        if (CurrentHealth / MaxHealth <= lowHealthPercent / 100)
        {
            OnLowHealth.Raise();
        }

        OnHealthChanged.Raise(currentHealth);
        OnHealthPercentChanged.Raise(currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            isDead = true;
            Die();
        }
    }

    public void Die()
    {
        OnDeath.Raise(transform.position);
        Destroy(gameObject);
    }

    public void Onestroy() { }

    public void ModifyMaxHealth(float amount)
    {
        maxHealth += amount;
    }
}
