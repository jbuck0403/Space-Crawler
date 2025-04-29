using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Main handler that listens for damage events
public class DamageNumbersUIHandler : MonoBehaviour
{
    [Header("References")]
    // [SerializeField]
    // private StatusEffectIconRegistry registry;

    [SerializeField]
    private DamageTakenEvent OnDamageTaken;

    [SerializeField]
    private FloatEvent OnHealingReceived;

    [SerializeField]
    private GameObject damageTextPrefab;

    [SerializeField]
    public List<DamageColorData> damageColors = new List<DamageColorData>();

    [Header("Movement Settings")]
    [Tooltip(
        "The animation curve that defines the path. X and Y values (0-1) represent percentage of max movement."
    )]
    [SerializeField]
    private AnimationCurve movementCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Tooltip("Maximum distance the damage number can move horizontally")]
    [SerializeField]
    private float maxXMovement = 100f;

    [Tooltip("Maximum distance the damage number can move vertically")]
    [SerializeField]
    private float maxYMovement = 50f;

    [Header("Opacity Settings")]
    [Tooltip(
        "The animation curve that controls opacity. Y value of 0 = fully visible, 1 = fully transparent"
    )]
    [SerializeField]
    private AnimationCurve opacityCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [SerializeField]
    private float animationDuration = 1.0f;

    [Header("Settings")]
    [SerializeField]
    private Vector3 offset = new Vector3(0, 1, 0);

    private Transform canvasTransform;
    private Camera mainCamera;
    private bool shouldMoveRight = true; // Toggle for alternating X direction

    private void Awake()
    {
        if (canvasTransform == null)
        {
            GameObject canvas = UIManager.Instance.DamageNumbersUIPanel;
            if (canvas != null)
            {
                canvasTransform = canvas.transform;
            }
        }

        mainCamera = Camera.main;
    }

    private Color GetColorByDamageType(DamageType damageType)
    {
        foreach (DamageColorData damageColorData in damageColors)
        {
            if (damageColorData.damageType == damageType)
            {
                return damageColorData.color;
            }
        }

        return Color.gray;
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
        Color damageColor = GetColorByDamageType(data.DamageType);
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
            textColor,
            movementCurve,
            opacityCurve,
            animationDuration,
            shouldMoveRight ? maxXMovement : -maxXMovement,
            maxYMovement
        );

        // Toggle the direction for the next damage number
        shouldMoveRight = !shouldMoveRight;
    }
}

[Serializable]
public class DamageColorData
{
    public DamageType damageType;
    public Color color;

    public DamageColorData(DamageType damageType, Color color)
    {
        this.damageType = damageType;
        this.color = color;
    }
}
