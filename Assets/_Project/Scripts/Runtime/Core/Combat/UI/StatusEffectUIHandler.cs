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
    private RectTransform debuffAnchor;

    [SerializeField]
    private StatusEffectEvent onStatusEffectApplied;

    [SerializeField]
    private StatusEffectEvent onStatusEffectRemoved;

    [SerializeField]
    private StatusEffectEvent onStatusEffectTick;

    [SerializeField]
    private GameObject targetEntity;

    [SerializeField]
    private StatusEffectIconRegistry iconRegistry;

    [Tooltip("The spacing between status effect icons")]
    [SerializeField]
    private float spacing = 5f;

    [Tooltip("The size of each status effect icon")]
    [SerializeField]
    private Vector2 iconSize = new Vector2(40f, 40f);

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
        // Use the effect ID directly from the event data
        string effectID = eventData.EffectID;
        Debug.Log($"StatusEffectUIHandler: Received effect with ID {effectID}");

        // Check if we already have a UI element for this effect
        if (activeUIElements.TryGetValue(effectID, out var uiElement))
        {
            // Update existing UI element
            uiElement.UpdateStackCount(eventData.CurrentStacks);
            // uiElement.UpdateDuration(eventData.RemainingDuration);
        }
        else
        {
            // Create new UI element
            GameObject newUIElement = Instantiate(statusEffectIconPrefab, debuffAnchor);
            StatusEffectUIElement newElement = newUIElement.GetComponent<StatusEffectUIElement>();

            if (newElement != null)
            {
                var (configuredIcon, tint) = iconRegistry.GetConfiguredIcon(effectID);
                Debug.Log(
                    $"StatusEffectUIHandler: Got icon {configuredIcon != null} and tint {tint} for effect {effectID}"
                );

                if (configuredIcon == null)
                {
                    Debug.LogError($"StatusEffectUIHandler: No icon found for effect {effectID}");
                    return;
                }

                newElement.Initialize(
                    configuredIcon,
                    tint,
                    eventData.EffectData.EffectType.ToString(),
                    eventData.EffectData.Description,
                    eventData.CurrentStacks,
                    eventData.RemainingDuration
                );

                activeUIElements.Add(effectID, newElement);
                RearrangeIcons();
            }
        }
    }

    private void OnStatusEffectRemoved(StatusEffectEventData eventData)
    {
        // Use the effect ID directly from the event data
        string effectID = eventData.EffectID;

        // Remove the UI element if it exists
        if (activeUIElements.TryGetValue(effectID, out var uiElement))
        {
            Destroy(uiElement.gameObject);
            activeUIElements.Remove(effectID);

            // Rearrange remaining icons
            RearrangeIcons();
        }
    }

    private void OnStatusEffectTick(StatusEffectEventData eventData)
    {
        // Use the effect ID directly from the event data
        string effectID = eventData.EffectID;

        // Update the UI element if it exists
        // if (activeUIElements.TryGetValue(effectID, out var uiElement))
        // {
        //     uiElement.UpdateDuration(eventData.RemainingDuration);
        // }
    }

    /// <summary>
    /// Rearranges all active status effect icons from right to left
    /// </summary>
    private void RearrangeIcons()
    {
        float currentX = 0;

        // Sort the effects if needed (can be alphabetically, by duration, etc.)
        // For now, we'll just use the dictionary order
        foreach (var element in activeUIElements.Values)
        {
            RectTransform rectTransform = element.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                // Set icon size
                rectTransform.sizeDelta = iconSize;

                // Position from right to left (negative X moves left from anchor)
                rectTransform.anchoredPosition = new Vector2(-currentX, 0);

                // Update for next icon
                currentX += iconSize.x + spacing;
            }
        }
    }
}
