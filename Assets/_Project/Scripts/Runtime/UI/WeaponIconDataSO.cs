using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponIconData", menuName = "UI/Weapon Icon Data")]
public class WeaponIconDataSO : ScriptableObject
{
    [Serializable]
    public class WeaponIconMapping
    {
        public WeaponType weaponType;
        public Sprite iconSprite;
    }

    [SerializeField]
    private List<WeaponIconMapping> weaponIcons = new List<WeaponIconMapping>();

    [SerializeField]
    private Sprite defaultIcon; // Fallback sprite if no specific icon is found

    public Sprite GetIconForWeaponType(WeaponType weaponType)
    {
        Debug.Log(
            $"WEAPONHUD: GetIconForWeaponType called for {weaponType}, icons count: {weaponIcons.Count}"
        );

        foreach (var mapping in weaponIcons)
        {
            if (mapping.weaponType == weaponType)
            {
                if (mapping.iconSprite != null)
                {
                    Debug.Log($"WEAPONHUD: Found icon for {weaponType}");
                    return mapping.iconSprite;
                }
                else
                {
                    Debug.LogWarning(
                        $"WEAPONHUD: Mapping found for {weaponType} but sprite is null"
                    );
                }
            }
        }

        Debug.LogWarning(
            $"WEAPONHUD: No icon found for weapon type {weaponType}, using default icon: {(defaultIcon != null ? "Available" : "NULL")}"
        );
        return defaultIcon; // Return default icon instead of null
    }

    private void OnValidate()
    {
        // Validate in the editor that we have sprites assigned
        foreach (var mapping in weaponIcons)
        {
            if (mapping.iconSprite == null)
            {
                Debug.LogWarning(
                    $"WEAPONHUD: No sprite assigned for weapon type {mapping.weaponType} in {name}"
                );
            }
        }

        if (defaultIcon == null)
        {
            Debug.LogWarning($"WEAPONHUD: No default icon assigned in {name}");
        }
    }
}
