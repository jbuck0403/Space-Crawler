using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHUD : MonoBehaviour
{
    [Header("Weapon Icons")]
    [SerializeField]
    private GameObject weaponIconPrefab;

    [SerializeField]
    private Transform weaponIconContainer;

    [SerializeField]
    private float weaponIconSpacing = 150f;

    [SerializeField]
    private WeaponIconDataSO weaponIconData;

    [Header("Ammo Icons")]
    [SerializeField]
    private GameObject ammoIconPrefab;

    [SerializeField]
    private Transform ammoIconContainer;

    [SerializeField]
    private float ammoIconSpacing = 150f;

    [SerializeField]
    private AmmoIconDataSO ammoIconData;

    private WeaponHandler playerWeaponHandler;
    private Dictionary<WeaponType, GameObject> weaponIcons =
        new Dictionary<WeaponType, GameObject>();
    private Dictionary<AmmoType, GameObject> ammoIcons = new Dictionary<AmmoType, GameObject>();

    private void Awake()
    {
        ValidateReferences();
    }

    private void ValidateReferences()
    {
        if (weaponIconPrefab == null || weaponIconContainer == null || weaponIconData == null)
        {
            Debug.LogWarning("Weapon icon references missing in WeaponHUD");
        }

        if (ammoIconPrefab == null || ammoIconContainer == null || ammoIconData == null)
        {
            Debug.LogWarning("Ammo icon references missing in WeaponHUD");
        }
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

            if (playerWeaponHandler.OnAmmoSwapped != null)
            {
                playerWeaponHandler.OnAmmoSwapped.AddListener(
                    playerWeaponHandler.gameObject,
                    OnAmmoSwapped
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

            if (playerWeaponHandler.OnAmmoSwapped != null)
            {
                playerWeaponHandler.OnAmmoSwapped.RemoveListener(
                    playerWeaponHandler.gameObject,
                    OnAmmoSwapped
                );
            }
        }
    }

    #region Weapon Icon Handling
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

    private void OnAmmoSwapped(AmmoType ammoType)
    {
        foreach (var icon in ammoIcons)
        {
            bool isSelected = icon.Key == ammoType;
            UpdateIconSelection(icon.Value, isSelected);
        }
    }

    public void InstantiateWeaponIcon(WeaponType weaponType)
    {
        if (
            weaponIcons.ContainsKey(weaponType)
            || weaponIconPrefab == null
            || weaponIconContainer == null
        )
        {
            return;
        }

        GameObject newIcon = InstantiateIcon(weaponIconPrefab, weaponIconContainer);
        if (newIcon == null)
            return;

        WeaponIconUI weaponIconUI = newIcon.GetComponent<WeaponIconUI>();
        if (weaponIconUI == null)
        {
            Destroy(newIcon);
            return;
        }

        Image iconImage = weaponIconUI.GetIcon();
        if (iconImage != null && weaponIconData != null)
        {
            Sprite weaponSprite = weaponIconData.GetIconForWeaponType(weaponType);
            if (weaponSprite != null)
            {
                iconImage.sprite = weaponSprite;
            }
        }

        weaponIconUI.EnableBackground(false);
        weaponIcons.Add(weaponType, newIcon);
        RepositionIcons(weaponIcons.Values, weaponIconContainer, weaponIconSpacing);
    }

    public void InstantiateAmmoIcon(AmmoType ammoType)
    {
        if (ammoIcons.ContainsKey(ammoType) || ammoIconPrefab == null || ammoIconContainer == null)
        {
            return;
        }

        GameObject newIcon = InstantiateIcon(ammoIconPrefab, ammoIconContainer);
        if (newIcon == null)
            return;

        WeaponIconUI iconUI = newIcon.GetComponent<WeaponIconUI>();
        if (iconUI == null)
        {
            Destroy(newIcon);
            return;
        }

        Image iconImage = iconUI.GetIcon();
        if (iconImage != null && ammoIconData != null)
        {
            Sprite ammoSprite = ammoIconData.GetIconForAmmoType(ammoType);
            if (ammoSprite != null)
            {
                iconImage.sprite = ammoSprite;
            }
        }

        iconUI.EnableBackground(false);
        ammoIcons.Add(ammoType, newIcon);
        RepositionIcons(ammoIcons.Values, ammoIconContainer, ammoIconSpacing);
    }
    #endregion

    #region Generic Icon Handling
    private GameObject InstantiateIcon(GameObject prefab, Transform container)
    {
        if (prefab == null || container == null)
        {
            return null;
        }

        return Instantiate(prefab, container);
    }

    private void UpdateIconSelection(GameObject icon, bool isSelected)
    {
        if (icon != null)
        {
            WeaponIconUI iconUI = icon.GetComponent<WeaponIconUI>();
            if (iconUI != null)
            {
                iconUI.EnableBackground(isSelected);
            }
        }
    }

    private void RepositionIcons(ICollection<GameObject> icons, Transform container, float spacing)
    {
        if (icons.Count == 0 || container == null)
        {
            return;
        }

        List<GameObject> iconList = new List<GameObject>(icons);
        float totalWidth = (iconList.Count - 1) * spacing;
        float startX = -totalWidth / 2;

        for (int i = 0; i < iconList.Count; i++)
        {
            RectTransform rectTransform = iconList[i].GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                float posX = startX + i * spacing;
                rectTransform.anchoredPosition = new Vector2(posX, 0);
            }
        }
    }
    #endregion

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
        RefreshWeaponIcons();
        RefreshAmmoIcons();
    }

    private void RefreshWeaponIcons()
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

    private void RefreshAmmoIcons()
    {
        foreach (var icon in ammoIcons.Values)
        {
            Destroy(icon);
        }
        ammoIcons.Clear();

        if (playerWeaponHandler != null)
        {
            foreach (AmmoType ammoType in System.Enum.GetValues(typeof(AmmoType)))
            {
                if (playerWeaponHandler.HasAmmo(ammoType))
                {
                    InstantiateAmmoIcon(ammoType);
                }
            }

            if (playerWeaponHandler.CurrentWeapon != null)
            {
                AmmoType currentAmmoType = playerWeaponHandler.GetCurrentAmmoType();
                OnAmmoSwapped(currentAmmoType);
            }
        }
    }
}
