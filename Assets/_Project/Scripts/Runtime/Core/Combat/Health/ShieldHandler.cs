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

    // [SerializeField]
    // private FloatEvent OnShieldChanged;

    // [SerializeField]
    // private FloatEvent OnShieldPercentChanged;

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
            RechargeShield(rechargeAmount);
        }
    }

    private IEnumerator CountdownToShieldRecharge()
    {
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
        float adjustedAmount = Mathf.Abs(amount);
        if (!HasShield)
            return 0f;

        if (damageType == DamageType.Lightning)
        {
            adjustedAmount *= 2f;
        }

        float damageAbsorbed = Mathf.Min(currentShield, adjustedAmount);

        ModifyShield(-damageAbsorbed);

        float overflowDamage = adjustedAmount - damageAbsorbed;
        print($"Overflow Damage Returned {overflowDamage}");

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
        print($"ModifyShield {amount} Current Shield {currentShield}");
        currentShield = Mathf.Min(currentShield + amount, maxShield);
        print($"New Shield {currentShield}");

        // OnShieldChanged.Raise(gameObject, currentShield);
        // OnShieldPercentChanged.Raise(gameObject, currentShield / maxShield);

        // Check if shield was just depleted
        // if (previousShield > 0 && currentShield <= 0)
        // {
        //     OnShieldDepleted.Raise(gameObject);
        // }
    }

    public void RechargeShield(float amount)
    {
        ModifyShield(Mathf.Max(0, amount));
    }

    public void SetMaxShield(float newMax)
    {
        maxShield = Mathf.Max(0, newMax);
        currentShield = Mathf.Min(currentShield, maxShield);

        // OnShieldChanged?.Raise(gameObject, currentShield);
        // OnShieldPercentChanged?.Raise(gameObject, currentShield / maxShield);
    }

    public void ModifyMaxShield(float amount)
    {
        SetMaxShield(maxShield + amount);
    }

    public void OnDestroy()
    {
        StopCoroutine(rechargeCountdown);
        rechargeCountdown = null;
    }
}
