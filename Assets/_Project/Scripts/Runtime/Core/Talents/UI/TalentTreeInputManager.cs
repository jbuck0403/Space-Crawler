using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles user interaction with the talent tree UI
/// </summary>
public class TalentTreeInputManager : MonoBehaviour
{
    [SerializeField]
    private TalentTree talentTree;

    [Header("UI References")]
    [SerializeField]
    private Text availablePointsText;

    [SerializeField]
    private GameObject tierLockedOverlayPrefab;

    [Serializable]
    public class TierConfig
    {
        public string tierName;
        public GameObject tierPanel;
        public List<TalentButtonUI> talentButtons;

        [HideInInspector]
        public GameObject lockedOverlay;
    }

    [SerializeField]
    private List<TierConfig> tiers = new List<TierConfig>();

    private void Awake()
    {
        if (talentTree == null)
        {
            talentTree = GetComponent<TalentTree>();
        }

        if (talentTree == null)
        {
            Debug.LogError("TalentTreeInputManager requires a TalentTree component!");
            enabled = false;
            return;
        }

        // Initialize tier locked overlays
        for (int i = 0; i < tiers.Count; i++)
        {
            if (tiers[i].tierPanel != null && tierLockedOverlayPrefab != null)
            {
                tiers[i].lockedOverlay = Instantiate(
                    tierLockedOverlayPrefab,
                    tiers[i].tierPanel.transform
                );
                tiers[i].lockedOverlay.SetActive(!IsTierUnlocked(i));

                // Add tier requirement text if available
                Text requirementText = tiers[i].lockedOverlay.GetComponentInChildren<Text>();
                if (requirementText != null && i > 0)
                {
                    requirementText.text = $"Requires {i * 5} points in previous tiers";
                }
            }
        }

        // Initialize all talent buttons
        for (int i = 0; i < tiers.Count; i++)
        {
            foreach (var button in tiers[i].talentButtons)
            {
                button.Initialize(talentTree, this, i);
            }
        }

        // Initial UI update
        UpdateUI();
    }

    /// <summary>
    /// Update all UI elements
    /// </summary>
    public void UpdateUI()
    {
        // Update available points text
        if (availablePointsText != null)
        {
            availablePointsText.text =
                $"Available Points: {talentTree.AvailablePoints}/{talentTree.TotalPoints}";
        }

        // Update tier unlocked status
        for (int i = 0; i < tiers.Count; i++)
        {
            bool tierUnlocked = IsTierUnlocked(i);
            if (tiers[i].lockedOverlay != null)
            {
                tiers[i].lockedOverlay.SetActive(!tierUnlocked);
            }
        }

        // Update all talent buttons
        for (int i = 0; i < tiers.Count; i++)
        {
            foreach (var button in tiers[i].talentButtons)
            {
                button.UpdateVisuals();
            }
        }
    }

    /// <summary>
    /// Check if a tier is unlocked (requires 5 points per previous tier)
    /// </summary>
    public bool IsTierUnlocked(int tierIndex)
    {
        if (tierIndex == 0)
            return true; // First tier is always unlocked

        int requiredPoints = tierIndex * 5;
        int pointsInPreviousTiers = 0;

        // Count points spent in all previous tiers
        for (int i = 0; i < tierIndex; i++)
        {
            foreach (var button in tiers[i].talentButtons)
            {
                BaseTalent runtimeTalent = talentTree.GetRuntimeTalent(button.Talent);
                if (runtimeTalent != null)
                {
                    pointsInPreviousTiers += runtimeTalent.pointsDesignated;
                }
            }
        }

        return pointsInPreviousTiers >= requiredPoints;
    }

    /// <summary>
    /// Check if a talent can be activated based on tier rules
    /// </summary>
    public bool CanActivateTalent(BaseTalent talent, int tierIndex)
    {
        // Check if tier is unlocked
        if (!IsTierUnlocked(tierIndex))
        {
            return false;
        }

        // Check prerequisites
        if (!talentTree.ArePrerequisitesMet(talent))
        {
            return false;
        }

        // Check if we have available points
        if (talentTree.AvailablePoints <= 0)
        {
            return false;
        }

        // Check if talent is already at max points
        BaseTalent runtimeTalent = talentTree.GetRuntimeTalent(talent);
        if (
            runtimeTalent != null
            && runtimeTalent.pointsDesignated >= runtimeTalent.maxDesignatedPoints
        )
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Get a flat array of all talent buttons
    /// </summary>
    public List<TalentButtonUI> GetAllTalentButtons()
    {
        List<TalentButtonUI> allButtons = new List<TalentButtonUI>();
        foreach (var tier in tiers)
        {
            allButtons.AddRange(tier.talentButtons);
        }
        return allButtons;
    }

    /// <summary>
    /// Find the TalentButtonUI for a given talent
    /// </summary>
    public TalentButtonUI FindButtonForTalent(BaseTalent talent)
    {
        foreach (var tier in tiers)
        {
            foreach (var button in tier.talentButtons)
            {
                if (button.Talent == talent)
                {
                    return button;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Reset all talents
    /// </summary>
    public void ResetAllTalents()
    {
        talentTree.ResetAllTalents();
        UpdateUI();
    }
}
