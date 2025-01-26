using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 100f;
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
    public VoidEvent OnDeath;

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
            OnHealingReceived.Raise(default);
        }
        else if (amount < 0)
        {
            OnDamageTaken.Raise(default);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        OnHealthChanged.Raise(currentHealth);
        OnHealthPercentChanged.Raise(currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            isDead = true;
            OnDeath.Raise(default);
        }
    }

    public void ModifyMaxHealth(float amount)
    {
        maxHealth += amount;
    }
}
