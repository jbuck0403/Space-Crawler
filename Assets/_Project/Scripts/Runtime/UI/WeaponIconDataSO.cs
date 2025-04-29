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
        foreach (var mapping in weaponIcons)
        {
            if (mapping.weaponType == weaponType)
            {
                if (mapping.iconSprite != null)
                {
                    return mapping.iconSprite;
                }
            }
        }

        return defaultIcon; // Return default icon instead of null
    }

    private void OnValidate()
    {
        // Validate in the editor that we have sprites assigned
        foreach (var mapping in weaponIcons)
        {
            if (mapping.iconSprite == null)
            {
                // Missing sprite
            }
        }

        if (defaultIcon == null)
        {
            Debug.LogWarning($"WEAPONHUD: No default icon assigned in {name}");
        }
    }
}
