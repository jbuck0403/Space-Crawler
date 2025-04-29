using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component for individual status effect UI elements.
/// Handles displaying a single status effect's icon, stack count, and duration.
/// </summary>
public class StatusEffectUIElement : MonoBehaviour
{
    [SerializeField]
    private Image durationFill;
    private Image iconImage;
    private TextMeshProUGUI stackCountText;
    private Text nameText;

    private float maxDuration;
    private float currentDuration;

    private string effectDescription;

    private void Awake()
    {
        iconImage = GetComponent<Image>();
        stackCountText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void InitOverlay(float duration, bool activate = true)
    {
        maxDuration = duration;
        currentDuration = duration;

        if (durationFill != null)
        {
            durationFill.type = Image.Type.Filled;
            durationFill.fillMethod = Image.FillMethod.Radial360;
            durationFill.fillOrigin = (int)Image.Origin360.Top;
            durationFill.fillClockwise = false;

            durationFill.fillAmount = 1.0f;
            durationFill.gameObject.SetActive(activate);
        }
    }

    private void Update()
    {
        if (
            durationFill != null
            && durationFill.gameObject.activeInHierarchy
            && currentDuration > 0
        )
        {
            UpdateDurationVisual();
        }
    }

    private void UpdateDurationVisual()
    {
        currentDuration -= Time.deltaTime;

        if (currentDuration <= 0)
        {
            currentDuration = 0;
            durationFill.gameObject.SetActive(false);
            return;
        }

        float fillPercentage = currentDuration / maxDuration;
        durationFill.fillAmount = fillPercentage;
    }

    public void Initialize(
        Sprite icon,
        Color tint,
        string name,
        string description,
        int stackCount,
        float duration
    )
    {
        if (iconImage == null)
        {
            Debug.LogError("StatusEffectUIElement: iconImage is not assigned!");
            return;
        }

        iconImage.sprite = icon;
        iconImage.color = tint;

        if (nameText != null)
        {
            nameText.text = name;
        }

        effectDescription = description;
        UpdateStackCount(stackCount);

        InitOverlay(duration);
    }

    public void UpdateStackCount(int stackCount)
    {
        if (stackCount > 1 && stackCountText != null)
        {
            stackCountText.text = stackCount.ToString();
            stackCountText.gameObject.SetActive(true);
        }
        else if (stackCountText != null)
        {
            stackCountText.gameObject.SetActive(false);
        }
    }

    public void UpdateDuration(float duration)
    {
        currentDuration = duration;

        if (durationFill != null)
        {
            durationFill.gameObject.SetActive(true);
        }
    }
}
