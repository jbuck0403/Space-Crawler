using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Main handler that listens for damage events
public class DamageNumbersUIHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private FloatEvent OnDamageTaken;

    [SerializeField]
    private FloatEvent OnHealingReceived;

    [SerializeField]
    private GameObject damageTextPrefab;

    [SerializeField]
    private Transform canvasTransform;

    [Header("Settings")]
    [SerializeField]
    private float fadeDuration = 1.0f;

    [SerializeField]
    private float moveDistance = 50f;

    [SerializeField]
    private Vector3 offset = new Vector3(0, 1, 0);

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
    }

    private void OnEnable()
    {
        // Subscribe to events
        OnDamageTaken.AddListener(gameObject, (amount) => HandleDamageEvent(gameObject, amount));
        OnHealingReceived.AddListener(
            gameObject,
            (amount) => HandleHealingEvent(gameObject, amount)
        );
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        OnDamageTaken.RemoveListener(gameObject, (amount) => HandleDamageEvent(gameObject, amount));
        OnHealingReceived.RemoveListener(
            gameObject,
            (amount) => HandleHealingEvent(gameObject, amount)
        );
    }

    private void HandleDamageEvent(GameObject target, float amount)
    {
        // Use red color for damage
        Color damageColor = Color.red;
        SpawnDamageNumber(target.transform.position, amount, damageColor, true);
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

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition + offset);

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
