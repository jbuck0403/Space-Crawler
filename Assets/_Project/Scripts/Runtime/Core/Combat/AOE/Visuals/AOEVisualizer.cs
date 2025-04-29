using UnityEngine;

/// <summary>
/// Handles the visual representation of AOE zones
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class AOEVisualizer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        spriteRenderer.sprite = CreateCircleSprite();

        spriteRenderer.color = new Color(1f, 0f, 0f, 0.3f);

        spriteRenderer.sortingOrder = -1;

        UpdateSize(circleCollider.radius);
    }

    private Sprite CreateCircleSprite()
    {
        int resolution = 256;
        Texture2D texture = new Texture2D(resolution, resolution);

        Color transparent = new Color(1, 1, 1, 0);
        Color white = Color.white;

        float centerX = resolution / 2f;
        float centerY = resolution / 2f;
        float radius = resolution / 2f;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float distance = Mathf.Sqrt(
                    (x - centerX) * (x - centerX) + (y - centerY) * (y - centerY)
                );

                if (distance < radius)
                {
                    texture.SetPixel(x, y, white);
                }
                else
                {
                    texture.SetPixel(x, y, transparent);
                }
            }
        }

        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0, 0, resolution, resolution),
            new Vector2(0.5f, 0.5f)
        );
    }

    public void UpdateSize(float radius)
    {
        if (spriteRenderer == null)
            return;

        float diameter = radius * 2;
        transform.localScale = new Vector3(diameter, diameter, 1);
    }

    public void SetColor(Color color)
    {
        if (spriteRenderer == null)
            return;

        spriteRenderer.color = new Color(color.r, color.g, color.b, 0.3f);
    }

    public void SetAlpha(float alpha)
    {
        if (spriteRenderer == null)
            return;

        Color currentColor = spriteRenderer.color;
        spriteRenderer.color = new Color(
            currentColor.r,
            currentColor.g,
            currentColor.b,
            Mathf.Clamp01(alpha)
        );
    }

    public void StartPulsing(float minAlpha = 0.1f, float maxAlpha = 0.4f, float pulseSpeed = 1f)
    {
        StartCoroutine(PulseCoroutine(minAlpha, maxAlpha, pulseSpeed));
    }

    private System.Collections.IEnumerator PulseCoroutine(
        float minAlpha,
        float maxAlpha,
        float pulseSpeed
    )
    {
        float t = 0f;
        bool increasing = true;

        while (gameObject != null && enabled)
        {
            if (increasing)
            {
                t += Time.deltaTime * pulseSpeed;
                if (t >= 1f)
                {
                    t = 1f;
                    increasing = false;
                }
            }
            else
            {
                t -= Time.deltaTime * pulseSpeed;
                if (t <= 0f)
                {
                    t = 0f;
                    increasing = true;
                }
            }

            float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
            SetAlpha(alpha);

            yield return null;
        }
    }
}
