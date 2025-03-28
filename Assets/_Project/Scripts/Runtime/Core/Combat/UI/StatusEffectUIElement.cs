using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component for individual status effect UI elements.
/// Handles displaying a single status effect's icon, stack count, and duration.
/// </summary>
public class StatusEffectUIElement : MonoBehaviour
{
    private Image iconImage;
    private TextMeshProUGUI stackCountText;
    private Text nameText;

    private string effectDescription;

    private void Awake()
    {
        iconImage = GetComponent<Image>();
        stackCountText = GetComponentInChildren<TextMeshProUGUI>();
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
        // UpdateDuration(duration);
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

    // public void UpdateDuration(float duration)
    // {
    //     if (duration > 0 && durationText != null)
    //     {
    //         durationText.text = Mathf.Ceil(duration).ToString();
    //         durationText.gameObject.SetActive(true);
    //     }
    //     else if (durationText != null)
    //     {
    //         durationText.gameObject.SetActive(false);
    //     }
    // }
}
