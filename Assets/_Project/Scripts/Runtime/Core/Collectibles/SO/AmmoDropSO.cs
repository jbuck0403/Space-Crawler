using UnityEngine;

[CreateAssetMenu(fileName = "AmmoDrop", menuName = "Collectibles/AmmoDrop")]
public class AmmoDropSO : CollectibleDropSO
{
    public override void HandleCollection()
    {
        // add ammo to weaponhandler.projectiletypes
        // refer to weapondrop for filtering
    }
}
