using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoIconData", menuName = "UI/Ammo Icon Data")]
public class AmmoIconDataSO : ScriptableObject
{
    [Serializable]
    public class AmmoIconMapping
    {
        public AmmoType ammoType;
        public Sprite iconSprite;
    }

    [SerializeField]
    private List<AmmoIconMapping> ammoIcons = new List<AmmoIconMapping>();

    public Sprite GetIconForAmmoType(AmmoType ammoType)
    {
        foreach (var mapping in ammoIcons)
        {
            if (mapping.ammoType == ammoType)
            {
                if (mapping.iconSprite != null)
                {
                    return mapping.iconSprite;
                }
            }
        }

        return null;
    }
}
