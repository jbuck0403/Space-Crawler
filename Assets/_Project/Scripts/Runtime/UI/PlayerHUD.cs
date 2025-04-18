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

    public GameObject player;
    public bool initialized = false;

    public void Initialize(GameObject player)
    {
        nextShotSlider.fillAmount = 1f;
        this.player = player;

        SubscribeToEvents();

        initialized = true;
        UIManager.Instance.playerHUDInitialized = initialized;
    }

    private void Update()
    {
        if (initialized)
        {
            HandleNextFireTimeBarFill();
            HandleWeaponAbilityCooldownBarFill();
        }
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

    private void SubscribeToEvents()
    {
        if (player == null)
        {
            return;
        }

        HealthSystem healthSystem = player.GetComponent<HealthSystem>();
        ShieldHandler shieldHandler = player.GetComponent<ShieldHandler>();
        WeaponHandler weaponHandler = player.GetComponent<WeaponHandler>();

        // Subscribe to percentage events
        healthSystem.OnHealthPercentChanged.AddListener(player, OnHealthPercentChanged);
        shieldHandler.OnShieldPercentChanged.AddListener(player, OnShieldPercentChanged);
        weaponHandler.OnNextFireTime.AddListener(player, OnNextFireTimeUpdated);
        weaponHandler.OnChargingWeapon.AddListener(player, OnChargingWeaponUpdated);
        weaponHandler.OnWeaponAbilityCooldown.AddListener(player, OnWeaponAbilityCooldownUpdated);
    }

    private void UnSubscribeFromEvents()
    {
        if (player == null)
        {
            return;
        }

        HealthSystem healthSystem = player.GetComponent<HealthSystem>();
        ShieldHandler shieldHandler = player.GetComponent<ShieldHandler>();
        WeaponHandler weaponHandler = player.GetComponent<WeaponHandler>();

        healthSystem.OnHealthPercentChanged.RemoveListener(player, OnHealthPercentChanged);
        shieldHandler.OnShieldPercentChanged.RemoveListener(player, OnShieldPercentChanged);
        weaponHandler.OnNextFireTime.RemoveListener(player, OnNextFireTimeUpdated);
        weaponHandler.OnChargingWeapon.RemoveListener(player, OnChargingWeaponUpdated);
        weaponHandler.OnWeaponAbilityCooldown.RemoveListener(
            player,
            OnWeaponAbilityCooldownUpdated
        );
    }

    private void OnDisable()
    {
        UnSubscribeFromEvents();
    }
}
