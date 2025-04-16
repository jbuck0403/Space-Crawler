using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages a character's talent progression using a TalentTreeSO for configuration
/// </summary>
public class TalentTreeHandler : MonoBehaviour
{
    // Dictionary to store runtime instances of talents
    private Dictionary<BaseTalent, BaseTalent> runtimeTalentInstances =
        new Dictionary<BaseTalent, BaseTalent>();

    [SerializeField]
    private TalentTreeSO talentTreeSO;

    [SerializeField]
    private int totalPoints = 0;

    [SerializeField]
    private int spentPoints = 0;

    public int AvailablePoints => totalPoints - spentPoints;
    public int TotalPoints => totalPoints;
    public int SpentPoints => spentPoints;
    public TalentTreeSO TalentTreeSO => talentTreeSO;

    private void Awake()
    {
        if (talentTreeSO == null)
        {
            Debug.LogError("%%% TalentTreeHandler: No TalentTreeSO assigned!");
            enabled = false;
            return;
        }

        InitializeTalents();
    }

    /// <summary>
    /// Initialize runtime instances of all available talents
    /// </summary>
    private void InitializeTalents()
    {
        Debug.Log("%%% TalentTreeHandler: Initializing talents");
        runtimeTalentInstances.Clear();

        // Initialize all talents from the TalentTreeSO
        List<BaseTalent> allTalents = talentTreeSO.GetAllTalents();
        foreach (BaseTalent talentTemplate in allTalents)
        {
            // Create a runtime instance by cloning the template
            BaseTalent runtimeTalent = Instantiate(talentTemplate);
            runtimeTalentInstances[talentTemplate] = runtimeTalent;
            Debug.Log(
                $"%%% TalentTreeHandler: Added talent {talentTemplate.name} to runtime instances"
            );
        }

        Debug.Log($"%%% TalentTreeHandler: Initialized {runtimeTalentInstances.Count} talents");
    }

    /// <summary>
    /// Check if a tier is unlocked (requires 5 points per previous tier)
    /// </summary>
    public bool IsTierUnlocked(int tierIndex)
    {
        Debug.Log($"%%% TalentTreeHandler: Checking if tier {tierIndex} is unlocked");

        if (tierIndex == 0)
        {
            Debug.Log("%%% TalentTreeHandler: Tier 0 is always unlocked");
            return true; // First tier is always unlocked
        }

        if (tierIndex < 0 || tierIndex >= talentTreeSO.GetTierCount())
        {
            Debug.LogWarning($"%%% TalentTreeHandler: Invalid tier index {tierIndex}");
            return false;
        }

        int requiredPoints = tierIndex * 5;
        int pointsInPreviousTiers = 0;

        // Count points spent in all previous tiers
        for (int i = 0; i < tierIndex; i++)
        {
            foreach (var talentTemplate in talentTreeSO.GetTalentsInTier(i))
            {
                BaseTalent runtimeTalent = GetRuntimeTalent(talentTemplate);
                if (runtimeTalent != null)
                {
                    pointsInPreviousTiers += runtimeTalent.pointsDesignated;
                }
            }
        }

        bool unlocked = pointsInPreviousTiers >= requiredPoints;
        Debug.Log(
            $"%%% TalentTreeHandler: Tier {tierIndex} unlocked: {unlocked}, points in previous tiers: {pointsInPreviousTiers}, required: {requiredPoints}"
        );
        return unlocked;
    }

    /// <summary>
    /// Try to unlock a talent if prerequisites are met and points are available
    /// </summary>
    public bool TryUnlockTalent(BaseTalent talentTemplate)
    {
        Debug.Log($"%%% TalentTreeHandler: Trying to unlock talent {talentTemplate.name}");

        // Get runtime instance of the talent
        if (!runtimeTalentInstances.TryGetValue(talentTemplate, out var talent))
        {
            Debug.LogWarning(
                $"%%% TalentTreeHandler: Talent {talentTemplate.name} is not available in this talent tree"
            );
            return false;
        }

        Debug.Log(
            $"%%% TalentTreeHandler: Found runtime talent {talent.name}, points: {talent.pointsDesignated}/{talent.maxDesignatedPoints}"
        );

        // Verify talent is not already at max points
        if (talent.pointsDesignated >= talent.maxDesignatedPoints)
        {
            Debug.LogWarning(
                $"%%% TalentTreeHandler: Talent {talent.name} is already at max level"
            );
            return false;
        }

        // Check if we have available points
        if (AvailablePoints < 1)
        {
            Debug.LogWarning(
                $"%%% TalentTreeHandler: Not enough points to unlock talent {talent.name}"
            );
            return false;
        }

        // Check if talent's tier is unlocked
        int tierIndex = talentTreeSO.GetTierIndex(talentTemplate);
        if (tierIndex < 0)
        {
            Debug.LogWarning($"%%% TalentTreeHandler: Talent {talent.name} is not in any tier");
            return false;
        }

        if (!IsTierUnlocked(tierIndex))
        {
            Debug.LogWarning(
                $"%%% TalentTreeHandler: Tier {tierIndex} for talent {talent.name} is locked"
            );
            return false;
        }

        // Check prerequisites (if tier > 0)
        if (tierIndex > 0 && !talent.ArePrerequisitesMet(GetActiveTalents()))
        {
            Debug.LogWarning($"%%% TalentTreeHandler: Prerequisites not met for {talent.name}");
            return false;
        }

        Debug.Log($"%%% TalentTreeHandler: All checks passed, activating talent {talent.name}");

        // Activate the talent
        if (talent.TryActivate(gameObject))
        {
            // Update points
            spentPoints++;
            Debug.Log(
                $"%%% TalentTreeHandler: Talent {talent.name} activated successfully, points spent: {spentPoints}"
            );
            return true;
        }

        Debug.LogWarning($"%%% TalentTreeHandler: Failed to activate talent {talent.name}");
        return false;
    }

    /// <summary>
    /// Removes a talent if it's currently active
    /// </summary>
    public bool TryRemoveTalent(BaseTalent talentTemplate)
    {
        // Get runtime instance of the talent
        if (!runtimeTalentInstances.TryGetValue(talentTemplate, out var talent))
        {
            Debug.LogWarning(
                $"%%% TalentTreeHandler: Talent {talentTemplate.name} not found in runtime instances"
            );
            return false;
        }

        // Check if talent is active
        if (talent.pointsDesignated <= 0)
        {
            Debug.LogWarning($"%%% TalentTreeHandler: Talent {talent.name} is not active");
            return false;
        }

        // Check if other talents depend on this one
        foreach (var activeTalent in GetActiveTalents())
        {
            if (activeTalent.requiredTalents != null)
            {
                foreach (var requiredTalent in activeTalent.requiredTalents)
                {
                    if (
                        requiredTalent.talent == talentTemplate
                        && activeTalent.pointsDesignated > 0
                    )
                    {
                        Debug.LogWarning(
                            $"%%% TalentTreeHandler: Cannot remove talent {talent.name} as {activeTalent.name} depends on it"
                        );
                        return false;
                    }
                }
            }
        }

        // Deactivate talent
        talent.Deactivate(gameObject);

        // Restore points
        spentPoints--;
        Debug.Log(
            $"%%% TalentTreeHandler: Talent {talent.name} deactivated, points spent: {spentPoints}"
        );
        return true;
    }

    /// <summary>
    /// Adds skill points to the talent tree
    /// </summary>
    public void AddPoints(int points)
    {
        if (points <= 0)
            return;

        totalPoints += points;
        Debug.Log($"%%% TalentTreeHandler: Added {points} points, total: {totalPoints}");
    }

    /// <summary>
    /// Gets a list of all active talents
    /// </summary>
    public List<BaseTalent> GetActiveTalents()
    {
        return runtimeTalentInstances.Values.Where(talent => talent.pointsDesignated > 0).ToList();
    }

    /// <summary>
    /// Get the runtime instance of a talent template
    /// </summary>
    public BaseTalent GetRuntimeTalent(BaseTalent talentTemplate)
    {
        if (runtimeTalentInstances.TryGetValue(talentTemplate, out var instance))
            return instance;
        return null;
    }

    /// <summary>
    /// Check if a specific talent is currently unlocked
    /// </summary>
    public bool IsTalentUnlocked(BaseTalent talentTemplate)
    {
        if (!runtimeTalentInstances.TryGetValue(talentTemplate, out var talent))
            return false;

        return talent.pointsDesignated > 0;
    }

    /// <summary>
    /// Reset all talents, refunding points
    /// </summary>
    public void ResetAllTalents()
    {
        // Get all active talents
        var activeTalents = GetActiveTalents();

        // Remove each talent
        foreach (var talent in activeTalents)
        {
            talent.Deactivate(gameObject);
        }

        // Reset spent points
        spentPoints = 0;
        Debug.Log($"%%% TalentTreeHandler: All talents reset, points refunded");
    }

    private void OnDestroy()
    {
        // Clean up all talent registrations when destroyed
        foreach (var talent in GetActiveTalents())
        {
            talent.Deactivate(gameObject);
        }
    }
}
