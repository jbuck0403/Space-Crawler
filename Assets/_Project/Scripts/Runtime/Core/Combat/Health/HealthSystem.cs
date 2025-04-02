using System.Collections.Generic;
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
    public DamageTakenEvent OnDamageTaken;

    [SerializeField]
    public FloatEvent OnHealingReceived;

    [SerializeField]
    public Vector2Event OnDeath;

    [SerializeField]
    public VoidEvent OnLowHealth;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => isDead;

    // Delegate for healing modification
    public delegate float HealingModifier(float amount);

    // Dictionary to track modifiers by source
    private Dictionary<GameObject, HealingModifier> healingModifiers =
        new Dictionary<GameObject, HealingModifier>();

    // Add/replace a healing modifier from a source
    public void AddHealingModifier(GameObject source, HealingModifier modifier)
    {
        healingModifiers[source] = modifier; // This replaces any existing modifier from this source
    }

    // Remove a specific source's modifier
    public void RemoveHealingModifier(GameObject source)
    {
        if (healingModifiers.ContainsKey(source))
            healingModifiers.Remove(source);
    }

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    // positive amount heals, negative amount damages
    private void ModifyHealth(float amount)
    {
        if (currentHealth <= 0)
            return;

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        if (CurrentHealth / MaxHealth <= lowHealthPercent / 100)
        {
            OnLowHealth.Raise(gameObject);
        }

        OnHealthChanged.Raise(gameObject, currentHealth);
        OnHealthPercentChanged.Raise(gameObject, currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            isDead = true;
            Die();
        }
    }

    public void Damage(float amount)
    {
        ModifyHealth(amount);
    }

    public void Heal(float amount)
    {
        float modifiedAmount = amount;
        foreach (var modifier in healingModifiers.Values)
        {
            modifiedAmount = modifier(modifiedAmount);
        }

        // Ensure healing is positive
        modifiedAmount = Mathf.Max(0, modifiedAmount);

        // Apply healing through existing system
        OnHealingReceived.Raise(gameObject, modifiedAmount);
        ModifyHealth(modifiedAmount);
    }

    public void Die()
    {
        OnDeath.Raise(gameObject, transform.position);
        Destroy(gameObject);
    }

    public void ModifyMaxHealth(float amount)
    {
        maxHealth += amount;
    }

    public void SetLowHealthPercent(float newPercent)
    {
        lowHealthPercent = newPercent;
    }

    public void ModifyLowHealthPercent(float percentToAdd)
    {
        lowHealthPercent = Mathf.Clamp(lowHealthPercent + percentToAdd, 5, 100);
    }
}
