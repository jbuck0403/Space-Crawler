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

    private float fireRate;
    private bool canFire = true;
    private float lastShotTime;

    private void Awake()
    {
        nextShotSlider.fillAmount = 1f;
    }

    private void Update()
    {
        HandleNextFireTimeBarFill();
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
    }

    private void HandleNextFireTimeBarFill()
    {
        if (!canFire)
        {
            if (nextShotSlider != null && fireRate > 0f)
            {
                float elapsedTime = Time.time - lastShotTime;
                float fillAmount = Mathf.Clamp01(elapsedTime / fireRate);

                nextShotSlider.fillAmount = fillAmount;
                if (fillAmount >= 1f)
                {
                    canFire = true;
                }
            }
        }
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
    }

    private void OnDisable()
    {
        HealthSystem healthSystem = GetComponent<HealthSystem>();
        ShieldHandler shieldHandler = GetComponent<ShieldHandler>();
        WeaponHandler weaponHandler = GetComponent<WeaponHandler>();

        healthSystem.OnHealthPercentChanged.RemoveListener(gameObject, OnHealthPercentChanged);
        shieldHandler.OnShieldPercentChanged.RemoveListener(gameObject, OnShieldPercentChanged);
        weaponHandler.OnNextFireTime.RemoveListener(gameObject, OnNextFireTimeUpdated);
    }
}
