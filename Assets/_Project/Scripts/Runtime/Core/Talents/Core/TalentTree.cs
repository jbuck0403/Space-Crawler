using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a collection of talents for a character
/// </summary>
public class TalentTree : MonoBehaviour
{
    [SerializeField]
    private List<BaseTalent> availableTalentTemplates = new List<BaseTalent>();

    // Dictionary to store runtime instances of talents
    private Dictionary<BaseTalent, BaseTalent> runtimeTalentInstances =
        new Dictionary<BaseTalent, BaseTalent>();
    private List<BaseTalent> activeTalents = new List<BaseTalent>();

    [SerializeField]
    private int totalPoints = 0;

    [SerializeField]
    private int spentPoints = 0;

    public int AvailablePoints => totalPoints - spentPoints;
    public int TotalPoints => totalPoints;
    public int SpentPoints => spentPoints;

    private void Awake()
    {
        // Create runtime instances of all available talents
        foreach (var talentTemplate in availableTalentTemplates)
        {
            runtimeTalentInstances[talentTemplate] = Instantiate(talentTemplate);
        }
    }

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

        // Verify talent is in available talents
        if (!availableTalentTemplates.Contains(talentTemplate))
        {
            Debug.LogWarning($"Talent {talent.name} is not available in this talent tree");
            return false;
        }

        // Verify talent is not already active
        if (activeTalents.Contains(talent) && talent.pointsDesignated >= talent.maxDesignatedPoints)
        {
            Debug.LogWarning($"Talent {talent.name} is already unlocked");
            return false;
        }

        // Check prerequisites
        if (!talent.ArePrerequisitesMet(activeTalents))
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
            // Add to active talents
            activeTalents.Add(talent);

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
        if (!activeTalents.Contains(talent))
        {
            return false;
        }

        // Check if any active talents require this one
        foreach (var activeTalent in activeTalents)
        {
            if (
                activeTalent.requiredTalents != null
                && activeTalent.requiredTalents.Contains(talentTemplate)
            )
            {
                Debug.LogWarning(
                    $"Cannot remove talent {talent.name} as other talents depend on it"
                );
                return false;
            }
        }

        // Deactivate talent
        talent.Deactivate(gameObject);

        // Remove from active talents
        activeTalents.Remove(talent);

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
        return new List<BaseTalent>(activeTalents);
    }

    /// <summary>
    /// Gets a list of all available talents
    /// </summary>
    public List<BaseTalent> GetAvailableTalents()
    {
        return new List<BaseTalent>(availableTalentTemplates);
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

        return activeTalents.Contains(talent);
    }

    /// <summary>
    /// Reset all talents, refunding points
    /// </summary>
    public void ResetAllTalents()
    {
        // Make a copy since we'll be modifying the list
        var talentsCopy = new List<BaseTalent>(activeTalents);

        // Remove each talent
        foreach (var talent in talentsCopy)
        {
            talent.Deactivate(gameObject);
        }

        // Clear active talents
        activeTalents.Clear();

        // Reset spent points
        spentPoints = 0;
    }

    private void OnDestroy()
    {
        // Clean up all talent registrations when destroyed
        foreach (var talent in activeTalents)
        {
            talent.Deactivate(gameObject);
        }
    }
}
