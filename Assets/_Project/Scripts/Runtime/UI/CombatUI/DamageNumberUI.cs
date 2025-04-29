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

        startPos = transform.position;
        textColor = color;
        movementCurve = movement;
        opacityCurve = opacity;
        duration = animDuration;
        maxXMovement = maxX;
        maxYMovement = maxY;

        textComponent.text = text;

        textComponent.fontMaterial = new Material(textComponent.fontMaterial);

        SetTextOpacity(1f);

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

            float curveValue = movementCurve.Evaluate(normalizedTime);

            Vector3 offset = new Vector3(
                normalizedTime * maxXMovement,
                curveValue * maxYMovement,
                0
            );

            transform.position = startPos + offset;

            float opacity = opacityCurve.Evaluate(normalizedTime);
            SetTextOpacity(opacity);

            yield return null;
        }

        SetTextOpacity(0f);

        Destroy(gameObject);
    }
}
