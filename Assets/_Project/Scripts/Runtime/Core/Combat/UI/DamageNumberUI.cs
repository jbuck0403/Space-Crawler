using System.Collections;
using TMPro;
using UnityEngine;

public class DamageNumberUI : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    private Color textColor;
    private Vector3 startPos;
    private AnimationCurve movementCurve;
    private AnimationCurve opacityCurve;
    private float duration;
    private float maxXMovement;
    private float maxYMovement;

    public void Initialize(
        string text,
        Color color,
        AnimationCurve movement,
        AnimationCurve opacity,
        float animDuration,
        float maxX,
        float maxY
    )
    {
        // Cache components
        textComponent = GetComponent<TextMeshProUGUI>();
        if (textComponent == null)
        {
            textComponent = GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent == null)
            {
                Debug.LogError("TextMeshProUGUI component not found on damage number");
                Destroy(gameObject);
                return;
            }
        }

        // Store the starting position and animation parameters
        startPos = transform.position;
        textColor = color;
        movementCurve = movement;
        opacityCurve = opacity;
        duration = animDuration;
        maxXMovement = maxX;
        maxYMovement = maxY;

        // Set up the text
        textComponent.text = text;

        // Create a material instance to avoid affecting other text
        textComponent.fontMaterial = new Material(textComponent.fontMaterial);

        // Set initial colors with full opacity
        SetTextOpacity(1f);

        // Start the animation
        StartCoroutine(AnimateDamageNumber());
    }

    private void SetTextOpacity(float opacity)
    {
        Color colorWithOpacity = new Color(textColor.r, textColor.g, textColor.b, opacity);
        textComponent.fontMaterial.SetColor("_FaceColor", colorWithOpacity);
        textComponent.fontMaterial.SetColor("_OutlineColor", colorWithOpacity);
    }

    private IEnumerator AnimateDamageNumber()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / duration;

            // Evaluate the movement curve at this time
            // The curve's Y value represents the percentage of maxYMovement to apply
            float curveValue = movementCurve.Evaluate(normalizedTime);

            // Calculate position based on curve evaluation
            // X position is determined by normalizedTime * maxXMovement
            // Y position is determined by the curve's Y value * maxYMovement
            Vector3 offset = new Vector3(
                normalizedTime * maxXMovement, // X moves based on time
                curveValue * maxYMovement, // Y moves based on curve height
                0
            );

            // Apply the position
            transform.position = startPos + offset;

            // Get opacity directly from opacity curve (curve value of 0 = transparent, 1 = opaque)
            float opacity = opacityCurve.Evaluate(normalizedTime);
            SetTextOpacity(opacity);

            yield return null;
        }

        // Ensure we're fully transparent before destroying
        SetTextOpacity(0f);

        // Self-destruct when animation is complete
        Destroy(gameObject);
    }
}
