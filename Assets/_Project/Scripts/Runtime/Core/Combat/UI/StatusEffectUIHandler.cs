using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles UI updates for status effects based on events.
/// This class is completely decoupled from the status effect implementation,
/// receiving all necessary data through events.
/// </summary>
public class StatusEffectUIHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject statusEffectIconPrefab;

    [SerializeField]
    private Transform statusEffectContainer;

    [SerializeField]
    private StatusEffectEvent onStatusEffectApplied;

    [SerializeField]
    private StatusEffectEvent onStatusEffectRemoved;

    [SerializeField]
    private StatusEffectEvent onStatusEffectTick;

    [SerializeField]
    private GameObject targetEntity;

    // Dictionary to keep track of active status effect UI elements
    private Dictionary<string, StatusEffectUIElement> activeUIElements =
        new Dictionary<string, StatusEffectUIElement>();

    private void OnEnable()
    {
        // Subscribe to events
        onStatusEffectApplied.AddListener(targetEntity, OnStatusEffectApplied);
        onStatusEffectRemoved.AddListener(targetEntity, OnStatusEffectRemoved);
        onStatusEffectTick.AddListener(targetEntity, OnStatusEffectTick);
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        onStatusEffectApplied.RemoveListener(targetEntity, OnStatusEffectApplied);
        onStatusEffectRemoved.RemoveListener(targetEntity, OnStatusEffectRemoved);
        onStatusEffectTick.RemoveListener(targetEntity, OnStatusEffectTick);
    }

    private void OnStatusEffectApplied(StatusEffectEventData eventData)
    {
        string effectID = eventData.EffectData.EffectType.ToString();

        // Check if we already have a UI element for this effect
        if (activeUIElements.TryGetValue(effectID, out var uiElement))
        {
            // Update existing UI element
            uiElement.UpdateStackCount(eventData.CurrentStacks);
            uiElement.UpdateDuration(eventData.RemainingDuration);
        }
        else
        {
            // Create new UI element
            GameObject newUIElement = Instantiate(statusEffectIconPrefab, statusEffectContainer);
            StatusEffectUIElement newElement = newUIElement.GetComponent<StatusEffectUIElement>();

            if (newElement != null)
            {
                // Initialize with data from the event
                newElement.Initialize(
                    eventData.EffectData.Icon,
                    eventData.EffectData.EffectType.ToString(),
                    eventData.EffectData.Description,
                    eventData.CurrentStacks,
                    eventData.RemainingDuration
                );

                // Add to our tracking dictionary
                activeUIElements.Add(effectID, newElement);
            }
        }
    }

    private void OnStatusEffectRemoved(StatusEffectEventData eventData)
    {
        string effectID = eventData.EffectData.EffectType.ToString();

        // Remove the UI element if it exists
        if (activeUIElements.TryGetValue(effectID, out var uiElement))
        {
            Destroy(uiElement.gameObject);
            activeUIElements.Remove(effectID);
        }
    }

    private void OnStatusEffectTick(StatusEffectEventData eventData)
    {
        string effectID = eventData.EffectData.EffectType.ToString();

        // Update the UI element if it exists
        if (activeUIElements.TryGetValue(effectID, out var uiElement))
        {
            uiElement.UpdateDuration(eventData.RemainingDuration);
        }
    }
}

/// <summary>
/// Component for individual status effect UI elements.
/// Handles displaying a single status effect's icon, stack count, and duration.
/// </summary>
public class StatusEffectUIElement : MonoBehaviour
{
    [SerializeField]
    private Image iconImage;

    [SerializeField]
    private Text stackCountText;

    [SerializeField]
    private Text durationText;

    [SerializeField]
    private Text nameText;

    private string effectDescription;

    public void Initialize(
        Sprite icon,
        string name,
        string description,
        int stackCount,
        float duration
    )
    {
        iconImage.sprite = icon;
        nameText.text = name;
        effectDescription = description;
        UpdateStackCount(stackCount);
        UpdateDuration(duration);
    }

    public void UpdateStackCount(int stackCount)
    {
        if (stackCount > 1)
        {
            stackCountText.text = stackCount.ToString();
            stackCountText.gameObject.SetActive(true);
        }
        else
        {
            stackCountText.gameObject.SetActive(false);
        }
    }

    public void UpdateDuration(float duration)
    {
        if (duration > 0)
        {
            durationText.text = Mathf.Ceil(duration).ToString();
            durationText.gameObject.SetActive(true);
        }
        else
        {
            durationText.gameObject.SetActive(false);
        }
    }
}
