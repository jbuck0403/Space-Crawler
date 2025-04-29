using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject weaponIconPrefab;

    [SerializeField]
    private Transform iconContainer;

    [Header("Layout Settings")]
    [SerializeField]
    private float spacingBetweenIcons = 150f;

    [SerializeField]
    private WeaponIconDataSO weaponIconData;

    private WeaponHandler playerWeaponHandler;
    private Dictionary<WeaponType, GameObject> weaponIcons =
        new Dictionary<WeaponType, GameObject>();

    private void Awake()
    {
        Debug.Log("WEAPONHUD: Awake called, checking references");
        ValidateReferences();
    }

    private void ValidateReferences()
    {
        if (weaponIconPrefab == null)
            Debug.LogError("WEAPONHUD: weaponIconPrefab is null");
        else
            Debug.Log("WEAPONHUD: weaponIconPrefab is assigned");

        if (iconContainer == null)
            Debug.LogError("WEAPONHUD: iconContainer is null");
        else
            Debug.Log("WEAPONHUD: iconContainer is assigned");

        if (weaponIconData == null)
            Debug.LogError("WEAPONHUD: weaponIconData SO is null");
        else
            Debug.Log("WEAPONHUD: weaponIconData SO is assigned");
    }

    public void Initialize(WeaponHandler weaponHandler)
    {
        Debug.Log(
            $"WEAPONHUD: Initialize called with weaponHandler: {(weaponHandler != null ? "Valid" : "NULL")}"
        );

        if (weaponHandler == null)
        {
            Debug.LogError("WEAPONHUD: WeaponHandler passed to Initialize is NULL!");
            return;
        }

        playerWeaponHandler = weaponHandler;

        // Validate that events exist
        Debug.Log(
            $"WEAPONHUD: OnWeaponInitialized event exists: {playerWeaponHandler.OnWeaponInitialized != null}"
        );
        Debug.Log(
            $"WEAPONHUD: OnWeaponSwapped event exists: {playerWeaponHandler.OnWeaponSwapped != null}"
        );

        AddListeners();

        // Refresh display to show any existing weapons
        RefreshDisplay();
    }

    private void AddListeners()
    {
        Debug.Log("WEAPONHUD: Adding event listeners");

        if (playerWeaponHandler != null)
        {
            if (playerWeaponHandler.OnWeaponInitialized != null)
            {
                playerWeaponHandler.OnWeaponInitialized.AddListener(
                    gameObject,
                    OnWeaponInitialized
                );
                Debug.Log("WEAPONHUD: Successfully added OnWeaponInitialized listener");
            }
            else
            {
                Debug.LogError(
                    "WEAPONHUD: Cannot add listener - OnWeaponInitialized event is null"
                );
            }

            if (playerWeaponHandler.OnWeaponSwapped != null)
            {
                playerWeaponHandler.OnWeaponSwapped.AddListener(gameObject, OnWeaponSwapped);
                Debug.Log("WEAPONHUD: Successfully added OnWeaponSwapped listener");
            }
            else
            {
                Debug.LogError("WEAPONHUD: Cannot add listener - OnWeaponSwapped event is null");
            }
        }
        else
        {
            Debug.LogError("WEAPONHUD: Cannot add listeners - playerWeaponHandler is null");
        }
    }

    private void RemoveListeners()
    {
        Debug.Log("WEAPONHUD: Removing event listeners");

        if (playerWeaponHandler != null)
        {
            if (playerWeaponHandler.OnWeaponInitialized != null)
            {
                playerWeaponHandler.OnWeaponInitialized.RemoveListener(
                    gameObject,
                    OnWeaponInitialized
                );
                Debug.Log("WEAPONHUD: Removed OnWeaponInitialized listener");
            }

            if (playerWeaponHandler.OnWeaponSwapped != null)
            {
                playerWeaponHandler.OnWeaponSwapped.RemoveListener(gameObject, OnWeaponSwapped);
                Debug.Log("WEAPONHUD: Removed OnWeaponSwapped listener");
            }
        }
    }

    // Check if method signature matches the event parameter types
    private void OnWeaponInitialized(WeaponType weaponType)
    {
        Debug.Log($"WEAPONHUD: OnWeaponInitialized event received for weapon type {weaponType}");
        InstantiateWeaponIcon(weaponType);
    }

    private void OnWeaponSwapped(WeaponType weaponType)
    {
        Debug.Log($"WEAPONHUD: OnWeaponSwapped event received for weapon type {weaponType}");

        foreach (var icon in weaponIcons)
        {
            bool isSelected = icon.Key == weaponType;
            Debug.Log($"WEAPONHUD: Updating selection state for {icon.Key} to {isSelected}");
            UpdateIconSelection(icon.Value, isSelected);
        }
    }

    private void UpdateIconSelection(GameObject icon, bool isSelected)
    {
        if (icon != null)
        {
            WeaponIconUI weaponIconUI = icon.GetComponent<WeaponIconUI>();
            if (weaponIconUI != null)
            {
                weaponIconUI.EnableBackground(isSelected);
                Debug.Log($"WEAPONHUD: Set weapon icon {icon.name} background to {isSelected}");
            }
            else
            {
                Debug.LogError($"WEAPONHUD: WeaponIconUI component not found on {icon.name}");
            }
        }
        else
        {
            Debug.LogError("WEAPONHUD: Cannot update selection for null icon");
        }
    }

    public void InstantiateWeaponIcon(WeaponType weaponType)
    {
        Debug.Log($"WEAPONHUD: InstantiateWeaponIcon called for {weaponType}");

        if (weaponIcons.ContainsKey(weaponType))
        {
            Debug.Log($"WEAPONHUD: Icon for {weaponType} already exists, skipping creation");
            return;
        }

        if (weaponIconPrefab == null)
        {
            Debug.LogError("WEAPONHUD: Cannot instantiate icon - weaponIconPrefab is null");
            return;
        }

        if (iconContainer == null)
        {
            Debug.LogError("WEAPONHUD: Cannot instantiate icon - iconContainer is null");
            return;
        }

        GameObject newIcon = Instantiate(weaponIconPrefab, iconContainer);
        Debug.Log($"WEAPONHUD: Created new icon GameObject: {newIcon.name}");

        WeaponIconUI weaponIconUI = newIcon.GetComponent<WeaponIconUI>();
        if (weaponIconUI == null)
        {
            Debug.LogError("WEAPONHUD: WeaponIconUI component not found on icon prefab");
            Destroy(newIcon);
            return;
        }

        // Get the icon image component from the WeaponIconUI
        Image iconImage = weaponIconUI.GetIcon();
        if (iconImage != null)
        {
            Debug.Log("WEAPONHUD: Found Image component on icon");

            if (weaponIconData != null)
            {
                Sprite weaponSprite = weaponIconData.GetIconForWeaponType(weaponType);
                if (weaponSprite != null)
                {
                    iconImage.sprite = weaponSprite;
                    Debug.Log($"WEAPONHUD: Set sprite for {weaponType} icon");
                }
                else
                {
                    Debug.LogError($"WEAPONHUD: No sprite found for weapon type {weaponType}");
                }
            }
            else
            {
                Debug.LogError("WEAPONHUD: weaponIconData is null");
            }
        }
        else
        {
            Debug.LogError("WEAPONHUD: No Image component found in WeaponIconUI.GetIcon()");
        }

        // Disable background by default (only enabled when selected)
        weaponIconUI.EnableBackground(false);

        weaponIcons.Add(weaponType, newIcon);
        Debug.Log($"WEAPONHUD: Added icon to dictionary, total count: {weaponIcons.Count}");

        RepositionIcons();
    }

    public void RepositionIcons()
    {
        Debug.Log($"WEAPONHUD: RepositionIcons called, icons count: {weaponIcons.Count}");

        if (weaponIcons.Count == 0)
        {
            Debug.Log("WEAPONHUD: No icons to reposition");
            return;
        }

        if (iconContainer == null)
        {
            Debug.LogError("WEAPONHUD: Cannot reposition icons - iconContainer is null");
            return;
        }

        List<GameObject> icons = new List<GameObject>(weaponIcons.Values);
        float totalWidth = (icons.Count - 1) * spacingBetweenIcons;
        float startX = -totalWidth / 2;

        Debug.Log(
            $"WEAPONHUD: Spacing between icons: {spacingBetweenIcons}, total width: {totalWidth}, startX: {startX}"
        );

        for (int i = 0; i < icons.Count; i++)
        {
            RectTransform rectTransform = icons[i].GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                float posX = startX + i * spacingBetweenIcons;
                rectTransform.anchoredPosition = new Vector2(posX, 0);
                Debug.Log($"WEAPONHUD: Positioned icon {i} at X: {posX}");
            }
            else
            {
                Debug.LogError($"WEAPONHUD: Icon {i} does not have a RectTransform component");
            }
        }
    }

    void OnEnable()
    {
        Debug.Log("WEAPONHUD: OnEnable called");
        if (playerWeaponHandler != null)
        {
            AddListeners();
        }
    }

    void OnDisable()
    {
        Debug.Log("WEAPONHUD: OnDisable called");
        RemoveListeners();
    }

    // Public method to force a complete refresh of the weapon icons
    public void RefreshDisplay()
    {
        Debug.Log("WEAPONHUD: RefreshDisplay called");

        // Clear existing icons
        foreach (var icon in weaponIcons.Values)
        {
            Destroy(icon);
        }
        weaponIcons.Clear();

        // Recreate icons for all existing weapons
        if (playerWeaponHandler != null && playerWeaponHandler.WeaponInstances != null)
        {
            Debug.Log(
                $"WEAPONHUD: Recreating icons for {playerWeaponHandler.WeaponInstances.Count} weapons"
            );
            foreach (var weapon in playerWeaponHandler.WeaponInstances)
            {
                InstantiateWeaponIcon(weapon.weaponType);
            }

            // Update selection for current weapon
            if (playerWeaponHandler.CurrentWeapon != null)
            {
                WeaponType currentType = playerWeaponHandler.CurrentWeapon.weaponType;
                Debug.Log($"WEAPONHUD: Setting current weapon UI selection to {currentType}");
                OnWeaponSwapped(currentType);
            }
        }
        else
        {
            Debug.LogWarning("WEAPONHUD: Cannot refresh - no weapon handler or weapon instances");
        }
    }
}
