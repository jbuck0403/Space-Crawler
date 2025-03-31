using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "New Status Effect Icon Registry",
    menuName = "UI/Status Effect Icon Registry"
)]
public class StatusEffectIconRegistry : ScriptableObject
{
    [System.Serializable]
    public class IconMapping
    {
        public StatusEffect effectType;
        public Sprite baseIcon;
        public Color defaultTint = Color.white;
    }

    [SerializeField]
    private List<IconMapping> effectIcons = new List<IconMapping>();
    private Dictionary<StatusEffect, IconMapping> iconLookup;

    private void OnEnable()
    {
        iconLookup = new Dictionary<StatusEffect, IconMapping>();
        foreach (var mapping in effectIcons)
        {
            iconLookup[mapping.effectType] = mapping;
        }
    }

    public (Sprite icon, Color tint) GetConfiguredIcon(string effectID)
    {
        // Extract the base effect type and damage type
        var (baseEffect, damageType) = ParseEffectID(effectID);
        Debug.Log(
            $"StatusEffectIconRegistry: Looking up icon for effect {effectID} (base: {baseEffect}, damageType: {damageType})"
        );

        if (iconLookup.TryGetValue(baseEffect, out var mapping))
        {
            Color tint = damageType.HasValue
                ? GetTintForDamageType(damageType.Value)
                : mapping.defaultTint;

            Debug.Log(
                $"StatusEffectIconRegistry: Found mapping for {baseEffect}, icon: {mapping.baseIcon != null}, tint: {tint}"
            );
            return (mapping.baseIcon, tint);
        }

        Debug.LogError($"StatusEffectIconRegistry: No mapping found for effect {baseEffect}");
        return (null, Color.white);
    }

    private (StatusEffect effect, DamageType? damageType) ParseEffectID(string effectID)
    {
        // Example: "FireBurning" -> (StatusEffect.Burning, DamageType.Fire)
        if (effectID.EndsWith("Burning"))
        {
            string prefix = effectID.Replace("Burning", "");
            if (System.Enum.TryParse(prefix, out DamageType damageType))
            {
                return (StatusEffect.Burning, damageType);
            }
        }
        // Add other effect type parsing as needed
        return (StatusEffect.Burning, null);
    }

    public Color GetTintForDamageType(DamageType damageType)
    {
        return damageType switch
        {
            DamageType.Fire => new Color(1f, 0.5f, 0.5f), // Red tint
            DamageType.Magic => new Color(0.5f, 0.5f, 1f), // Blue tint
            DamageType.Lightning => new Color(0.7f, 0.9f, 1f), // Light blue tint
            _ => Color.white
        };
    }
}
