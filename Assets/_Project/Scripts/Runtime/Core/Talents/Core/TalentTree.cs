using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages a collection of talents for a character
/// </summary>
public class TalentTree : MonoBehaviour
{
    // Dictionary to store runtime instances of talents
    private Dictionary<BaseTalent, BaseTalent> runtimeTalentInstances =
        new Dictionary<BaseTalent, BaseTalent>();

    [SerializeField]
    private int totalPoints = 0;

    [SerializeField]
    private int spentPoints = 0;

    public int AvailablePoints => totalPoints - spentPoints;
    public int TotalPoints => totalPoints;
    public int SpentPoints => spentPoints;

    /// <summary>
    /// Try to unlock a talent if prerequisites are met and points are available
    /// </summary>
    public bool TryUnlockTalent(BaseTalent talentTemplate)
    {
        // Get runtime instance of the talent
        if (!runtimeTalentInstances.TryGetValue(talentTemplate, out var talent))
        {
            Debug.LogWarning($"Talent {talentTemplate.name} is not available in this talent tree");
            return false;
        }

        // Verify talent is not already at max points
        if (talent.pointsDesignated >= talent.maxDesignatedPoints)
        {
            Debug.LogWarning($"Talent {talent.name} is already at max level");
            return false;
        }

        // Check prerequisites
        if (!ArePrerequisitesMet(talentTemplate))
        {
            Debug.LogWarning($"Prerequisites for talent {talent.name} are not met");
            return false;
        }

        // Check if enough points
        if (AvailablePoints < 1)
        {
            Debug.LogWarning($"Not enough points to unlock talent {talent.name}");
            return false;
        }

        // Activate the talent
        if (talent.TryActivate(gameObject))
        {
            // Update points
            spentPoints++;
            return true;
        }

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
            return false;
        }

        // Check if talent is active
        if (talent.pointsDesignated <= 0)
        {
            return false;
        }

        // new implementation
        foreach (var activeTalent in GetActiveTalents())
        {
            if (activeTalent.requiredTalents != null)
            {
                foreach (var requiredTalent in activeTalent.requiredTalents)
                {
                    if (
                        requiredTalent.talent == talentTemplate
                        && talentTemplate.pointsDesignated > 0
                    )
                    {
                        return false;
                    }
                }
            }
        }

        // old implementation
        // foreach (var activeTalent in GetActiveTalents())
        // {
        //     if (
        //         activeTalent.requiredTalents != null
        //         && activeTalent.requiredTalents.Contains(talentTemplate)
        //         && activeTalent.pointsDesignated > 0
        //     )
        //     {
        //         Debug.LogWarning(
        //             $"Cannot remove talent {talent.name} as other talents depend on it"
        //         );
        //         return false;
        //     }
        // }

        // Deactivate talent
        talent.Deactivate(gameObject);

        // Restore points
        spentPoints--;
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
    /// Check if prerequisites for a talent are met
    /// </summary>
    public bool ArePrerequisitesMet(BaseTalent talentTemplate)
    {
        if (!runtimeTalentInstances.TryGetValue(talentTemplate, out var talent))
            return false;

        return talent.ArePrerequisitesMet(GetActiveTalents());
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
