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
        ValidateReferences();
    }

    private void ValidateReferences()
    {
        if (weaponIconPrefab == null)
            return;

        if (iconContainer == null)
            return;

        if (weaponIconData == null)
            return;
    }

    public void Initialize(WeaponHandler weaponHandler)
    {
        if (weaponHandler == null)
        {
            return;
        }

        playerWeaponHandler = weaponHandler;
        AddListeners();
        RefreshDisplay();
    }

    private void AddListeners()
    {
        if (playerWeaponHandler != null)
        {
            if (playerWeaponHandler.OnWeaponInitialized != null)
            {
                playerWeaponHandler.OnWeaponInitialized.AddListener(
                    gameObject,
                    OnWeaponInitialized
                );
            }

            if (playerWeaponHandler.OnWeaponSwapped != null)
            {
                playerWeaponHandler.OnWeaponSwapped.AddListener(
                    playerWeaponHandler.gameObject,
                    OnWeaponSwapped
                );
            }
        }
    }

    private void RemoveListeners()
    {
        if (playerWeaponHandler != null)
        {
            if (playerWeaponHandler.OnWeaponInitialized != null)
            {
                playerWeaponHandler.OnWeaponInitialized.RemoveListener(
                    gameObject,
                    OnWeaponInitialized
                );
            }

            if (playerWeaponHandler.OnWeaponSwapped != null)
            {
                playerWeaponHandler.OnWeaponSwapped.RemoveListener(
                    playerWeaponHandler.gameObject,
                    OnWeaponSwapped
                );
            }
        }
    }

    private void OnWeaponInitialized(WeaponType weaponType)
    {
        InstantiateWeaponIcon(weaponType);
    }

    private void OnWeaponSwapped(WeaponType weaponType)
    {
        foreach (var icon in weaponIcons)
        {
            bool isSelected = icon.Key == weaponType;
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
            }
        }
    }

    public void InstantiateWeaponIcon(WeaponType weaponType)
    {
        if (weaponIcons.ContainsKey(weaponType))
        {
            return;
        }

        if (weaponIconPrefab == null)
        {
            return;
        }

        if (iconContainer == null)
        {
            return;
        }

        GameObject newIcon = Instantiate(weaponIconPrefab, iconContainer);

        WeaponIconUI weaponIconUI = newIcon.GetComponent<WeaponIconUI>();
        if (weaponIconUI == null)
        {
            Destroy(newIcon);
            return;
        }

        Image iconImage = weaponIconUI.GetIcon();
        if (iconImage != null)
        {
            if (weaponIconData != null)
            {
                Sprite weaponSprite = weaponIconData.GetIconForWeaponType(weaponType);
                if (weaponSprite != null)
                {
                    iconImage.sprite = weaponSprite;
                }
            }
        }

        weaponIconUI.EnableBackground(false);

        weaponIcons.Add(weaponType, newIcon);

        RepositionIcons();
    }

    public void RepositionIcons()
    {
        if (weaponIcons.Count == 0)
        {
            return;
        }

        if (iconContainer == null)
        {
            return;
        }

        List<GameObject> icons = new List<GameObject>(weaponIcons.Values);
        float totalWidth = (icons.Count - 1) * spacingBetweenIcons;
        float startX = -totalWidth / 2;

        for (int i = 0; i < icons.Count; i++)
        {
            RectTransform rectTransform = icons[i].GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                float posX = startX + i * spacingBetweenIcons;
                rectTransform.anchoredPosition = new Vector2(posX, 0);
            }
        }
    }

    void OnEnable()
    {
        if (playerWeaponHandler != null)
        {
            AddListeners();
        }
    }

    void OnDisable()
    {
        RemoveListeners();
    }

    public void RefreshDisplay()
    {
        foreach (var icon in weaponIcons.Values)
        {
            Destroy(icon);
        }
        weaponIcons.Clear();

        if (playerWeaponHandler != null && playerWeaponHandler.WeaponInstances != null)
        {
            foreach (var weapon in playerWeaponHandler.WeaponInstances)
            {
                InstantiateWeaponIcon(weapon.weaponType);
            }

            if (playerWeaponHandler.CurrentWeapon != null)
            {
                WeaponType currentType = playerWeaponHandler.CurrentWeapon.weaponType;
                OnWeaponSwapped(currentType);
            }
        }
    }
}
