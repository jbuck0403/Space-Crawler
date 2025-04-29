using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDrop", menuName = "Collectibles/WeaponDrop")]
public class WeaponDropSO : CollectibleDropSO
{
    [SerializeField]
    private List<BaseWeaponSO> weapons;

    public override void HandleCollection()
    {
        Debug.Log("WEAPONHUD: WeaponDropSO.HandleCollection called");

        List<BaseWeaponSO> availableWeapons = new List<BaseWeaponSO>();
        foreach (BaseWeaponSO weapon in weapons)
        {
            if (!CollectibleManager.Instance.WeaponHandler.HasWeapon(weapon))
            {
                availableWeapons.Add(weapon);
            }
        }

        if (availableWeapons.Count == 0)
        {
            Debug.Log("All Weapons already obtained...");
            return;
        }

        int randomIndex = Random.Range(0, availableWeapons.Count);
        BaseWeaponSO weaponSO = availableWeapons[randomIndex];

        Debug.Log($"WEAPONHUD: Initializing new weapon of type {weaponSO.weaponType}");
        CollectibleManager.Instance.WeaponHandler.InitializeWeapon(weaponSO);

        // Force a refresh of the weapon HUD
        UIManager.RefreshWeaponDisplay();
        Debug.Log("WEAPONHUD: Requested UI refresh after weapon initialization");

        GameManager.Instance.GameData.AddUnlockedWeapon(weaponSO.weaponType);
    }
}
