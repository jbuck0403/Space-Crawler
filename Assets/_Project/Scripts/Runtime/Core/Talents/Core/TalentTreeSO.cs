using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that defines a complete talent tree configuration
/// </summary>
[CreateAssetMenu(fileName = "NewTalentTree", menuName = "Talents/Talent Tree")]
public class TalentTreeSO : ScriptableObject
{
    [Serializable]
    public class TierData
    {
        public string tierName;
        public List<BaseTalent> talents = new List<BaseTalent>();
    }

    [SerializeField]
    private List<TierData> talentTiers = new List<TierData>();

    /// <summary>
    /// Get the tier index for a specific talent
    /// </summary>
    public int GetTierIndex(BaseTalent talent)
    {
        for (int i = 0; i < talentTiers.Count; i++)
        {
            if (talentTiers[i].talents.Contains(talent))
            {
                return i;
            }
        }

        return -1; // Talent not found in any tier
    }

    /// <summary>
    /// Get all talents from all tiers
    /// </summary>
    public List<BaseTalent> GetAllTalents()
    {
        List<BaseTalent> allTalents = new List<BaseTalent>();
        foreach (var tier in talentTiers)
        {
            allTalents.AddRange(tier.talents);
        }
        return allTalents;
    }

    /// <summary>
    /// Get talents in a specific tier
    /// </summary>
    public List<BaseTalent> GetTalentsInTier(int tierIndex)
    {
        if (tierIndex >= 0 && tierIndex < talentTiers.Count)
        {
            return talentTiers[tierIndex].talents;
        }
        return new List<BaseTalent>();
    }

    /// <summary>
    /// Get the total number of tiers
    /// </summary>
    public int GetTierCount()
    {
        return talentTiers.Count;
    }

    /// <summary>
    /// Get the name of a specific tier
    /// </summary>
    public string GetTierName(int tierIndex)
    {
        if (tierIndex >= 0 && tierIndex < talentTiers.Count)
        {
            return talentTiers[tierIndex].tierName;
        }
        return string.Empty;
    }
}
