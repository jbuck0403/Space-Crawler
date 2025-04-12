using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all talents that can be unlocked in the skill tree
/// </summary>
[CreateAssetMenu(menuName = "SpaceShooter/Talents/Base Talent")]
public abstract class BaseTalent : ScriptableObject
{
    [Header("Talent Information")]
    public string talentName;
    public string description;
    public Sprite icon;
    public int maxDesignatedPoints = 1;
    public int pointsDesignated = 0;

    [Header("Talent Requirements")]
    public List<BaseTalent> requiredTalents;

    /// <summary>
    /// Activates this talent on the specified GameObject
    /// </summary>
    /// <param name="owner">The GameObject that will receive talent effects</param>
    /// <returns>True if activation was successful</returns>
    public virtual bool TryActivate(GameObject owner)
    {
        if (maxDesignatedPoints <= pointsDesignated)
            return false; // Already max level

        OnPointAdded(owner);

        return true;
    }

    /// <summary>
    /// Deactivates this talent on the specified GameObject
    /// </summary>
    /// <param name="owner">The GameObject to remove talent effects from</param>
    public virtual void Deactivate(GameObject owner)
    {
        if (pointsDesignated <= 0)
            return; // Not active

        OnPointRemoved(owner);
    }

    protected virtual void OnPointAdded(GameObject owner)
    {
        if (maxDesignatedPoints > pointsDesignated)
        {
            if (pointsDesignated > 0)
            {
                OnDeactivate(owner);
                pointsDesignated++;
                OnActivate(owner);
            }
            else
            {
                OnActivate(owner);
                pointsDesignated = 1;
            }
        }
    }

    protected virtual void OnPointRemoved(GameObject owner)
    {
        if (pointsDesignated > 1)
        {
            OnDeactivate(owner);
            pointsDesignated--;
            OnActivate(owner);
        }
        else if (pointsDesignated == 1)
        {
            OnDeactivate(owner);
            pointsDesignated = 0;
        }
    }

    /// <summary>
    /// Handles common talent activation logic
    /// </summary>
    protected virtual void OnActivate(GameObject owner)
    {
        // Get the ITalentModifiable component
        var modifiable = owner.GetComponent<ITalentModifiable>();
        if (modifiable == null)
        {
            Debug.LogWarning(
                $"Talent {talentName} requires an ITalentModifiable component on {owner.name}"
            );
            return;
        }

        // Apply all modifiers for this talent
        var modifierData = GetModifierData();
        foreach (var data in modifierData)
        {
            TalentModifierHelper.AddModifier(modifiable, data.ModifierType, this, data.Modifier);
        }
    }

    /// <summary>
    /// Handles common talent deactivation logic
    /// </summary>
    protected virtual void OnDeactivate(GameObject owner)
    {
        // Get the ITalentModifiable component
        var modifiable = owner.GetComponent<ITalentModifiable>();
        if (modifiable == null)
            return;

        // Remove all modifiers from this talent
        TalentModifierHelper.RemoveModifiersFromTalent(modifiable, this);
    }

    /// <summary>
    /// Override to return the list of modifiers and their types for this talent
    /// </summary>
    protected virtual List<TalentModifierData> GetModifierData()
    {
        return new List<TalentModifierData>();
    }

    /// <summary>
    /// Checks if all prerequisites for this talent are met
    /// </summary>
    public bool ArePrerequisitesMet(List<BaseTalent> activeTalents)
    {
        if (requiredTalents == null || requiredTalents.Count == 0)
            return true; // No prerequisites

        foreach (var prerequisite in requiredTalents)
        {
            if (!activeTalents.Contains(prerequisite))
                return false; // Missing a required talent
        }

        return true;
    }

    /// <summary>
    /// Data structure for talent modifiers
    /// </summary>
    protected class TalentModifierData
    {
        public ModifierType ModifierType { get; set; }
        public Delegate Modifier { get; set; }

        public TalentModifierData(ModifierType type, Delegate modifier)
        {
            ModifierType = type;
            Modifier = modifier;
        }
    }
}
