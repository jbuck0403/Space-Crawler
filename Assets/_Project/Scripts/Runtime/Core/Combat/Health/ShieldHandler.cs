using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ShieldHandler : MonoBehaviour
{
    [SerializeField]
    private float maxShield = 100f;

    [SerializeField]
    private float rechargeAmount = 0.1f;

    [SerializeField]
    private float timeUntilRecharge = 3f;

    [SerializeField]
    private float currentShield;

    [SerializeField]
    public FloatEvent OnShieldPercentChanged;

    // [SerializeField]
    // private VoidEvent OnShieldDepleted;

    private HealthSystem healthSystem;
    private bool recharging = false;
    private Coroutine rechargeCountdown;

    public float MaxShield => maxShield;
    public float CurrentShield => currentShield;
    public bool HasShield => currentShield > 0;

    private void Awake()
    {
        currentShield = maxShield;
        healthSystem = GetComponent<HealthSystem>();

        if (healthSystem == null)
        {
            Debug.LogError($"ShieldHandler requires a HealthSystem component on {gameObject.name}");
        }
    }

    private void FixedUpdate()
    {
        if (recharging)
        {
            // TBI Skill Point Delegate: SHIELD_RECHARGE_RATE
            RechargeShield(rechargeAmount);
        }
    }

    private IEnumerator CountdownToShieldRecharge()
    {
        // TBI Skill Point Delegate: SHIELD_RECHARGE_DELAY
        yield return new WaitForSeconds(timeUntilRecharge);

        TriggerShieldRecharge();
    }

    public void TriggerShieldRecharge(bool shouldRecharge = true)
    {
        if (shouldRecharge)
        {
            SetRecharging();
        }
    }

    public void SetRecharging(bool recharging = true)
    {
        this.recharging = recharging;
    }

    public float Damage(float amount, DamageType damageType)
    {
        // TBI Skill Point Delegate: BEFORE_SHIELD_DAMAGE
        float adjustedAmount = Mathf.Abs(amount);
        if (!HasShield)
            return 0f;

        if (damageType == DamageType.Lightning)
        {
            // TBI Skill Point Delegate: DAMAGE_TYPE_SHIELD_MODIFIER
            adjustedAmount *= 2f;
        }

        float damageAbsorbed = Mathf.Min(currentShield, adjustedAmount);

        ModifyShield(-damageAbsorbed);

        float overflowDamage = adjustedAmount - damageAbsorbed;
        print($"Overflow Damage Returned {overflowDamage}");

        // TBI Skill Point Delegate: AFTER_SHIELD_DAMAGE
        return -overflowDamage;
    }

    public void HandleDelayedShieldRecharge()
    {
        // reset the shield recharge timer on damage taken
        if (rechargeCountdown != null)
        {
            StopCoroutine(rechargeCountdown);
        }

        rechargeCountdown = StartCoroutine(CountdownToShieldRecharge());
    }

    // positive amount adds shield, negative amount damages shield
    private void ModifyShield(float amount)
    {
        float previousShield = currentShield;
        currentShield = Mathf.Min(currentShield + amount, maxShield);

        // Raise events
        OnShieldPercentChanged.Raise(gameObject, currentShield / maxShield);

        if (previousShield > 0 && currentShield <= 0)
        {
            // TBI Skill Point Delegate: ON_SHIELD_DEPLETED
        }
    }

    public void RechargeShield(float amount)
    {
        ModifyShield(Mathf.Max(0, amount));
    }

    public void SetMaxShield(float newMax)
    {
        // TBI Skill Point Delegate: MODIFY_MAX_SHIELD
        maxShield = Mathf.Max(0, newMax);
        currentShield = Mathf.Min(currentShield, maxShield);

        // OnShieldChanged?.Raise(gameObject, currentShield);
        OnShieldPercentChanged.Raise(gameObject, currentShield / maxShield);
    }

    public void ModifyMaxShield(float amount)
    {
        SetMaxShield(maxShield + amount);
    }

    public void OnDestroy()
    {
        if (rechargeCountdown != null)
            StopCoroutine(rechargeCountdown);

        rechargeCountdown = null;
    }
}
