using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Main handler that listens for damage events
public class DamageNumbersUIHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private StatusEffectIconRegistry registry;

    [SerializeField]
    private DamageTakenEvent OnDamageTaken;

    [SerializeField]
    private FloatEvent OnHealingReceived;

    [SerializeField]
    private GameObject damageTextPrefab;

    [Header("Settings")]
    [SerializeField]
    private float fadeDuration = 1.0f;

    [SerializeField]
    private float moveDistance = 50f;

    [SerializeField]
    private Vector3 offset = new Vector3(0, 1, 0);

    private Transform canvasTransform;
    private Camera mainCamera;

    private void Awake()
    {
        if (canvasTransform == null)
        {
            GameObject canvas = GameObject.FindGameObjectWithTag("DamageNumbersUIPanel");
            if (canvas != null)
            {
                canvasTransform = canvas.transform;
            }
        }

        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        // Subscribe to events
        OnDamageTaken.AddListener(gameObject, (data) => HandleDamageEvent(gameObject, data));
        OnHealingReceived.AddListener(gameObject, (data) => HandleHealingEvent(gameObject, data));
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        OnDamageTaken.RemoveListener(gameObject, (data) => HandleDamageEvent(gameObject, data));
        OnHealingReceived.RemoveListener(
            gameObject,
            (data) => HandleHealingEvent(gameObject, data)
        );
    }

    private void HandleDamageEvent(GameObject target, DamageTakenEventData data)
    {
        // Use red color for damage
        Color damageColor = registry.GetTintForDamageType(data.DamageType);
        SpawnDamageNumber(target.transform.position, data.DamageAmount, damageColor, true);
    }

    private void HandleHealingEvent(GameObject target, float amount)
    {
        // Use green color for healing
        Color healColor = Color.green;
        SpawnDamageNumber(target.transform.position, amount, healColor, false);
    }

    private void SpawnDamageNumber(
        Vector3 worldPosition,
        float amount,
        Color textColor,
        bool isDamage
    )
    {
        if (canvasTransform == null)
        {
            Debug.LogError("Canvas transform is missing for damage numbers");
            return;
        }

        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPosition + offset);

        GameObject damageTextObj = Instantiate(
            damageTextPrefab,
            screenPos,
            Quaternion.identity,
            canvasTransform
        );

        DamageNumberUI damageNumber = damageTextObj.GetComponent<DamageNumberUI>();
        damageNumber.Initialize(
            isDamage ? $"{Mathf.Round(amount)}" : $"+{Mathf.Round(amount)}",
            textColor
        );
    }
}
