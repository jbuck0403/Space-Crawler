using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Image healthSlider;

    [SerializeField]
    private Image shieldSlider;

    [SerializeField]
    private Image nextShotSlider;

    [SerializeField]
    private Image weaponAbilityCooldownSlider;

    private float fireRate;
    private bool canFire = true;
    private float lastShotTime;
    private bool isCharging = false;
    private float weaponAbilityCooldown;
    private float lastAbilityTime;

    private void Awake()
    {
        nextShotSlider.fillAmount = 1f;
    }

    private void Update()
    {
        HandleNextFireTimeBarFill();
        HandleWeaponAbilityCooldownBarFill();
    }

    private void OnHealthPercentChanged(float healthPercent)
    {
        healthSlider.fillAmount = healthPercent;
    }

    private void OnShieldPercentChanged(float shieldPercent)
    {
        shieldSlider.fillAmount = shieldPercent;
    }

    private void OnNextFireTimeUpdated(float newFireRate)
    {
        fireRate = newFireRate;
        nextShotSlider.fillAmount = 0f;
        canFire = false;
        lastShotTime = Time.time;
        isCharging = false;
    }

    private void OnChargingWeaponUpdated(float chargeProgress)
    {
        isCharging = true;
        // Invert the charge progress so the bar decreases as charge increases
        nextShotSlider.fillAmount = 1f - chargeProgress;
    }

    private void HandleNextFireTimeBarFill()
    {
        if (nextShotSlider == null || fireRate <= 0f)
            return;

        if (!isCharging && !canFire)
        {
            // Normal fire rate fill
            float elapsedTime = Time.time - lastShotTime;
            float fillAmount = Mathf.Clamp01(elapsedTime / fireRate);
            nextShotSlider.fillAmount = fillAmount;

            if (fillAmount >= 1f)
            {
                canFire = true;
            }
        }
    }

    private void HandleWeaponAbilityCooldownBarFill()
    {
        float elapsedTime = Time.time - lastAbilityTime;
        float fillAmount = Mathf.Clamp01(elapsedTime / weaponAbilityCooldown);
        weaponAbilityCooldownSlider.fillAmount = fillAmount;
    }

    private void OnWeaponAbilityCooldownUpdated(float cooldown)
    {
        weaponAbilityCooldown = cooldown;
        lastAbilityTime = Time.time;
        weaponAbilityCooldownSlider.fillAmount = 0f;
    }

    private void OnEnable()
    {
        HealthSystem healthSystem = GetComponent<HealthSystem>();
        ShieldHandler shieldHandler = GetComponent<ShieldHandler>();
        WeaponHandler weaponHandler = GetComponent<WeaponHandler>();

        // Subscribe to percentage events
        healthSystem.OnHealthPercentChanged.AddListener(gameObject, OnHealthPercentChanged);
        shieldHandler.OnShieldPercentChanged.AddListener(gameObject, OnShieldPercentChanged);
        weaponHandler.OnNextFireTime.AddListener(gameObject, OnNextFireTimeUpdated);
        weaponHandler.OnChargingWeapon.AddListener(gameObject, OnChargingWeaponUpdated);
        weaponHandler.OnWeaponAbilityCooldown.AddListener(
            gameObject,
            OnWeaponAbilityCooldownUpdated
        );
    }

    private void OnDisable()
    {
        HealthSystem healthSystem = GetComponent<HealthSystem>();
        ShieldHandler shieldHandler = GetComponent<ShieldHandler>();
        WeaponHandler weaponHandler = GetComponent<WeaponHandler>();

        healthSystem.OnHealthPercentChanged.RemoveListener(gameObject, OnHealthPercentChanged);
        shieldHandler.OnShieldPercentChanged.RemoveListener(gameObject, OnShieldPercentChanged);
        weaponHandler.OnNextFireTime.RemoveListener(gameObject, OnNextFireTimeUpdated);
        weaponHandler.OnChargingWeapon.RemoveListener(gameObject, OnChargingWeaponUpdated);
        weaponHandler.OnWeaponAbilityCooldown.RemoveListener(
            gameObject,
            OnWeaponAbilityCooldownUpdated
        );
    }
}
