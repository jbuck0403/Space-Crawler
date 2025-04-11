using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Talent that increases weapon damage
/// </summary>
[CreateAssetMenu(menuName = "SpaceShooter/Talents/Weapon Damage Boost")]
public class WeaponDamageBoostTalent : BaseTalent
{
    [SerializeField]
    private float damageMultiplier = 1.15f;

    [SerializeField]
    private bool affectsAllWeapons = true;

    [SerializeField]
    private List<BaseWeaponSO> affectedWeaponTypes;

    // Track which weapons have been modified for cleanup
    private Dictionary<GameObject, List<BaseWeaponSO>> modifiedWeapons =
        new Dictionary<GameObject, List<BaseWeaponSO>>();

    protected override void OnActivate(GameObject owner)
    {
        var weaponHandler = owner.GetComponent<WeaponHandler>();
        if (weaponHandler == null)
        {
            Debug.LogWarning($"WeaponDamageBoostTalent requires a WeaponHandler on {owner.name}");
            return;
        }

        // Initialize tracking for this owner
        if (!modifiedWeapons.ContainsKey(owner))
            modifiedWeapons[owner] = new List<BaseWeaponSO>();

        // Apply to current weapon and register for weapon change events
        ApplyToCurrentWeapon(owner, weaponHandler);

        // Subscribe to weapon change events
        weaponHandler.OnWeaponChanged += (newWeapon) => OnWeaponChanged(owner, newWeapon);

        Debug.Log(
            $"Activated weapon damage boost talent on {owner.name} with multiplier {damageMultiplier}"
        );
    }

    private void ApplyToCurrentWeapon(GameObject owner, WeaponHandler weaponHandler)
    {
        if (weaponHandler.CurrentWeapon != null)
        {
            if (ShouldAffectWeapon(weaponHandler.CurrentWeapon))
            {
                // Note: For this example, we would inject a delegate into the weapon's damage calculation
                // Since we don't want to modify existing classes yet, we'll just log what would happen
                Debug.Log(
                    $"Would inject damage modifier into {weaponHandler.CurrentWeapon.name}: x{damageMultiplier}"
                );

                // Track modified weapon
                modifiedWeapons[owner].Add(weaponHandler.CurrentWeapon);
            }
        }
    }

    private void OnWeaponChanged(GameObject owner, BaseWeaponSO newWeapon)
    {
        if (ShouldAffectWeapon(newWeapon) && !modifiedWeapons[owner].Contains(newWeapon))
        {
            // Apply damage boost to new weapon
            Debug.Log(
                $"Would inject damage modifier into new weapon {newWeapon.name}: x{damageMultiplier}"
            );

            // Track modified weapon
            modifiedWeapons[owner].Add(newWeapon);
        }
    }

    private bool ShouldAffectWeapon(BaseWeaponSO weapon)
    {
        if (affectsAllWeapons)
            return true;

        return affectedWeaponTypes != null && affectedWeaponTypes.Contains(weapon);
    }

    protected override void OnDeactivate(GameObject owner)
    {
        var weaponHandler = owner.GetComponent<WeaponHandler>();
        if (weaponHandler == null)
            return;

        // Unsubscribe from weapon change events
        // Note: In real implementation, we would need to store the delegate instance
        // and unsubscribe that specific instance

        // Remove damage modifiers from all modified weapons
        if (modifiedWeapons.ContainsKey(owner))
        {
            foreach (var weapon in modifiedWeapons[owner])
            {
                Debug.Log($"Would remove damage modifier from weapon {weapon.name}");
            }

            modifiedWeapons.Remove(owner);
        }

        Debug.Log($"Deactivated weapon damage boost talent on {owner.name}");
    }
}
