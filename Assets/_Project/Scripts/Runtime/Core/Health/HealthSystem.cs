using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 100f;
    private float currentHealth;

    public UnityEvent<float> OnHealthChanged;
    public UnityEvent<float> OnHealthPercentChanged;
    public UnityEvent OnDamageTaken;
    public UnityEvent OnHealingReceived;
    public UnityEvent OnDeath;

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
            OnHealingReceived.Invoke();
        }
        else if (amount < 0)
        {
            OnDamageTaken.Invoke();
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        OnHealthChanged.Invoke(currentHealth);
        OnHealthPercentChanged.Invoke(currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            OnDeath.Invoke();
        }
    }

    public void ModifyMaxHealth(float amount)
    {
        maxHealth += amount;
    }
}
