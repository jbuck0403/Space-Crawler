using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Talent that increases healing received by a percentage
/// </summary>
[CreateAssetMenu(menuName = "SpaceShooter/Talents/Healing Boost")]
public class HealingBoostTalent : BaseTalent
{
    [SerializeField]
    private float healingMultiplier = 1.25f;

    // Track delegate instances for cleanup
    private Dictionary<GameObject, HealthSystem.HealingModifier> healingModifiers =
        new Dictionary<GameObject, HealthSystem.HealingModifier>();

    protected override void OnActivate(GameObject owner)
    {
        var healthSystem = owner.GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            Debug.LogWarning($"HealingBoostTalent requires a HealthSystem on {owner.name}");
            return;
        }

        // Create delegate that will boost healing
        HealthSystem.HealingModifier healingBooster = (amount) => amount * healingMultiplier;

        // Register with health system
        healthSystem.AddHealingModifier(owner, healingBooster);

        // Store for cleanup
        healingModifiers[owner] = healingBooster;

        Debug.Log(
            $"Activated healing boost talent on {owner.name} with multiplier {healingMultiplier}"
        );
    }

    protected override void OnDeactivate(GameObject owner)
    {
        var healthSystem = owner.GetComponent<HealthSystem>();
        if (healthSystem == null)
            return;

        // Cleanup delegate registration
        healthSystem.RemoveHealingModifier(owner);

        // Remove from tracking
        healingModifiers.Remove(owner);

        Debug.Log($"Deactivated healing boost talent on {owner.name}");
    }
}
