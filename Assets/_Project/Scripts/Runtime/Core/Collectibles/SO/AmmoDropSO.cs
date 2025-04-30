using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoDrop", menuName = "Collectibles/AmmoDrop")]
public class AmmoDropSO : CollectibleDropSO
{
    [SerializeField]
    private List<ProjectileTypeSO> ammoAvailableForUnlock;

    public override void HandleCollection()
    {
        List<ProjectileTypeSO> canBeUnlocked = new List<ProjectileTypeSO>();
        foreach (ProjectileTypeSO ammo in ammoAvailableForUnlock)
        {
            if (!CollectibleManager.Instance.WeaponHandler.HasAmmo(ammo.ammoType))
            {
                canBeUnlocked.Add(ammo);
            }
        }

        if (canBeUnlocked.Count == 0)
        {
            Debug.Log("All Ammo already obtained...");
            return;
        }

        int randomIndex = RandomUtils.Range(0, canBeUnlocked.Count);
        ProjectileTypeSO projectileTypeSO = canBeUnlocked[randomIndex];

        CollectibleManager.Instance.WeaponHandler.InitializeAmmo(projectileTypeSO.ammoType);

        UIManager.RefreshWeaponDisplay();

        GameManager.Instance.GameData.AddUnlockedAmmo(projectileTypeSO.ammoType);
    }
}
