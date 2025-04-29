using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDrop", menuName = "Collectibles/WeaponDrop")]
public class WeaponDropSO : CollectibleDropSO
{
    [SerializeField]
    private List<BaseWeaponSO> weapons;

    public override void HandleCollection()
    {
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
        CollectibleManager.Instance.WeaponHandler.InitializeWeapon(weaponSO);

        GameManager.Instance.GameData.AddUnlockedWeapon(weaponSO.weaponType);
    }
}
