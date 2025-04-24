using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Base class for all talents that can be unlocked in the skill tree
/// </summary>
public abstract class BaseTalent : ScriptableObject
{
    [Header("Talent Information")]
    public string talentName;
    public string description;
    public Sprite icon;
    public int maxDesignatedPoints = 1;
    public int pointsDesignated = 0;

    [Header("Talent Requirements")]
    [SerializeField]
    public List<TalentPreRequisiteData> requiredTalents;

    // Runtime state - not serialized
    [NonSerialized]
    protected GameObject currentOwner;

    [NonSerialized]
    protected MonoBehaviour coroutineRunner;

    /// <summary>
    /// Activates this talent on the specified GameObject
    /// </summary>
    /// <param name="owner">The GameObject that will receive talent effects</param>
    /// <returns>True if activation was successful</returns>
    public virtual bool TryActivate(GameObject owner)
    {
        Debug.Log($"%%% BaseTalent: Trying to activate talent {talentName} ({name})");

        if (maxDesignatedPoints <= pointsDesignated)
        {
            Debug.LogWarning($"%%% BaseTalent: Talent {talentName} already at max level");
            return false; // Already max level
        }

        currentOwner = owner;

        // Find a MonoBehaviour to run coroutines if needed
        coroutineRunner = owner.GetComponent<TalentTreeHandler>();

        Debug.Log($"%%% BaseTalent: Adding point to talent {talentName}");
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

        currentOwner = null;
        coroutineRunner = null;
    }

    protected virtual void OnPointAdded(GameObject owner)
    {
        Debug.Log(
            $"%%% BaseTalent: OnPointAdded for {talentName}, current points: {pointsDesignated}, max: {maxDesignatedPoints}"
        );

        if (maxDesignatedPoints > pointsDesignated)
        {
            int oldPoints = pointsDesignated;
            pointsDesignated++;
            Debug.Log($"%%% POINTS CHANGED: {talentName} from {oldPoints} to {pointsDesignated}");
            GameManager.Instance.GameData.ModifyAllocatedTalents(this, pointsDesignated);

            if (pointsDesignated > 1)
            {
                Debug.Log($"%%% BaseTalent: Reactivating talent {talentName} to add point");
                OnDeactivate(owner);
                OnActivate(owner);
            }
            else
            {
                Debug.Log($"%%% BaseTalent: First activation of talent {talentName}");
                OnActivate(owner);
            }
        }
        else
        {
            Debug.Log(
                $"%%% NOT ADDING POINT: {talentName} at max {pointsDesignated}/{maxDesignatedPoints}"
            );
        }

        Debug.Log(
            $"%%% BaseTalent: Talent {talentName} now at {pointsDesignated}/{maxDesignatedPoints} points"
        );
    }

    protected virtual void OnPointRemoved(GameObject owner)
    {
        int pointsDesignatedBefore = pointsDesignated;
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

        if (pointsDesignatedBefore != pointsDesignated)
        {
            GameManager.Instance.GameData.ModifyAllocatedTalents(this, pointsDesignated);
        }
    }

    /// <summary>
    /// Handles common talent activation logic
    /// </summary>
    protected virtual void OnActivate(GameObject owner)
    {
        Debug.Log($"%%% BaseTalent: OnActivate for {talentName}");

        foreach (var data in GetModifierData(owner))
        {
            if (data.Modifiable != null)
            {
                // Add to specific modifiable only
                ModifierHelper.AddModifier(data.Modifiable, data.ModifierType, this, data.Modifier);
            }
            else
            {
                // Backwards compatibility - add to all modifiables
                var modifiables = GetModifiable(owner);
                ModifierHelper.AddModifiers(modifiables, data.ModifierType, this, data.Modifier);
            }
        }
    }

    /// <summary>
    /// Handles common talent deactivation logic
    /// </summary>
    protected virtual void OnDeactivate(GameObject owner)
    {
        // Find all IModifiable components on the owner and its children
        var modifiables = GetModifiable(owner);
        foreach (var modifiable in modifiables)
        {
            // Remove all modifiers from this talent
            ModifierHelper.RemoveModifiersFromSource(modifiable, this);
        }
    }

    /// <summary>
    /// Starts a coroutine on the owner GameObject
    /// </summary>
    protected Coroutine StartCoroutine(IEnumerator routine)
    {
        if (coroutineRunner != null)
            return coroutineRunner.StartCoroutine(routine);

        Debug.LogWarning($"No coroutine runner available for talent {talentName}");
        return null;
    }

    /// <summary>
    /// Stops a coroutine on the owner GameObject
    /// </summary>
    protected void StopCoroutine(Coroutine coroutine)
    {
        if (coroutineRunner != null && coroutine != null)
            coroutineRunner.StopCoroutine(coroutine);
    }

    /// <summary>
    /// Checks if all prerequisites for this talent are met
    /// </summary>
    public bool ArePrerequisitesMet(List<BaseTalent> activeTalents)
    {
        Debug.Log(
            $"%%% BaseTalent: Checking prerequisites for {talentName}, required talents: {(requiredTalents != null ? requiredTalents.Count : 0)}"
        );

        if (requiredTalents == null || requiredTalents.Count == 0)
        {
            Debug.Log($"%%% BaseTalent: No prerequisites for {talentName}");
            return true; // No prerequisites
        }

        foreach (var prerequisite in requiredTalents)
        {
            bool foundTalent = activeTalents.Any(t =>
                t.name == prerequisite.talent.name
                && t.pointsDesignated >= prerequisite.requiredPointsInvested
            );

            Debug.Log(
                $"%%% BaseTalent: Prerequisite {prerequisite.talent.name} found? {foundTalent}, required points: {prerequisite.requiredPointsInvested}"
            );

            if (!foundTalent)
            {
                Debug.LogWarning(
                    $"%%% BaseTalent: Missing prerequisite {prerequisite.talent.name} for {talentName}"
                );
                return false; // Missing a required talent
            }
        }

        Debug.Log($"%%% BaseTalent: All prerequisites met for {talentName}");
        return true;
    }

    /// <summary>
    /// Override to return the list of modifiers and their types for this talent
    /// </summary>
    protected abstract List<TalentModifierData> GetModifierData(GameObject gameObject);

    /// <summary>
    /// Gets the appropriate IModifiable component from the owner
    /// </summary>
    protected abstract List<IModifiable> GetModifiable(GameObject owner);

    /// <summary>
    /// Data structure for talent modifiers
    /// </summary>
    protected class TalentModifierData
    {
        public ModifierType ModifierType { get; set; }
        public Delegate Modifier { get; set; }
        public IModifiable Modifiable;

        public TalentModifierData(ModifierType type, Delegate modifier, IModifiable modifiable)
        {
            ModifierType = type;
            Modifier = modifier;
            Modifiable = modifiable;
        }
    }
}

[Serializable]
public class TalentPreRequisiteData
{
    public BaseTalent talent;
    public int requiredPointsInvested;

    public TalentPreRequisiteData(BaseTalent requiredTalent, int requiredPointsInvested)
    {
        this.talent = requiredTalent;
        this.requiredPointsInvested = requiredPointsInvested;
    }
}
