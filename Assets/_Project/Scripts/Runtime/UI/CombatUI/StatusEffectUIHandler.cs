using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
    private Vector2Event onDeath;

    [Tooltip("The spacing between status effect icons")]
    [SerializeField]
    private float spacing = 5f;

    [Tooltip("The size of each status effect icon")]
    [SerializeField]
    private Vector2 iconSize = new Vector2(40f, 40f);

    // Dictionary to keep track of active status effect UI elements
    private Dictionary<string, StatusEffectUIElement> activeUIElements =
        new Dictionary<string, StatusEffectUIElement>();

    public void Initialize(GameObject targetEntity)
    {
        this.targetEntity = targetEntity;
        ClearAllStatusEffects();
        SubscribeToListeners();
    }

    private void SubscribeToListeners()
    {
        // Subscribe to events
        onStatusEffectApplied.AddListener(targetEntity, OnStatusEffectApplied);
        onStatusEffectRemoved.AddListener(targetEntity, OnStatusEffectRemoved);
        onStatusEffectTick.AddListener(targetEntity, OnStatusEffectTick);

        // Subscribe to death event
        if (onDeath != null)
        {
            onDeath.AddListener(targetEntity, (x) => OnDeath(x));
        }
    }

    private void UnSubscribeFromListeners()
    {
        // Unsubscribe from events
        onStatusEffectApplied.RemoveListener(targetEntity, OnStatusEffectApplied);
        onStatusEffectRemoved.RemoveListener(targetEntity, OnStatusEffectRemoved);
        onStatusEffectTick.RemoveListener(targetEntity, OnStatusEffectTick);

        // Unsubscribe from death event
        if (onDeath != null)
        {
            onDeath.RemoveListener(targetEntity, (x) => OnDeath(x));
        }
    }

    private void OnDeath(Vector2 pos)
    {
        // when the entity dies, clear all status effects
        ClearAllStatusEffects();

        // disable this component
        this.enabled = false;
    }

    /// <summary>
    /// Clears all status effect UI elements and empties the tracking dictionary.
    /// </summary>
    public void ClearAllStatusEffects()
    {
        // Destroy all UI elements
        foreach (var element in activeUIElements.Values)
        {
            if (element != null && element.gameObject != null)
            {
                Destroy(element.gameObject);
            }
        }

        // Clear the dictionary
        activeUIElements.Clear();

        Debug.Log($"Cleared all status effects for {targetEntity.name}");
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
                // Get icon directly from the StatusEffectData
                Sprite icon = eventData.EffectData.Icon;

                if (icon == null)
                {
                    Debug.LogError($"StatusEffectUIHandler: No icon found for effect {effectID}");
                    return;
                }

                newElement.Initialize(
                    icon,
                    Color.white, // Use default white color (no tint)
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
        if (activeUIElements.TryGetValue(effectID, out var uiElement))
        {
            uiElement.UpdateDuration(eventData.RemainingDuration);
        }
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

    private void OnDisable()
    {
        UnSubscribeFromListeners();
    }
}
