using System.Collections;
using TMPro;
using UnityEngine;

public class DamageNumberUI : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    private Color textColor;
    private Vector3 startPos;
    private float moveDistance = 50f;
    private float fadeDuration = 1.0f;

    public void Initialize(string text, Color color)
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

        // Store the starting position
        startPos = transform.position;
        textColor = color;

        // Set up the text
        textComponent.text = text;

        // Create a material instance to avoid affecting other text
        textComponent.fontMaterial = new Material(textComponent.fontMaterial);

        // Set both the Face and Outline colors
        textComponent.fontMaterial.SetColor("_FaceColor", textColor);
        textComponent.fontMaterial.SetColor("_OutlineColor", textColor);

        // Start the animation
        StartCoroutine(AnimateDamageNumber());
    }

    private IEnumerator AnimateDamageNumber()
    {
        float elapsed = 0f;
        Vector3 targetPos = startPos + new Vector3(0, moveDistance, 0);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / fadeDuration;

            // Move text upward
            transform.position = Vector3.Lerp(startPos, targetPos, normalizedTime);

            // Calculate the alpha for this frame
            float currentAlpha = Mathf.Lerp(1, 0, normalizedTime);

            // Apply alpha to both face and outline while maintaining the color
            textComponent.fontMaterial.SetColor(
                "_FaceColor",
                new Color(textColor.r, textColor.g, textColor.b, currentAlpha)
            );
            textComponent.fontMaterial.SetColor(
                "_OutlineColor",
                new Color(textColor.r, textColor.g, textColor.b, currentAlpha)
            );

            yield return null;
        }

        // Self-destruct when animation is complete
        Destroy(gameObject);
    }
}
