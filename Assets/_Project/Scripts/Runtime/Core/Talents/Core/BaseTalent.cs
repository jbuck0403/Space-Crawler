using System;
using System.Collections;
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

    // Runtime state - not serialized
    [System.NonSerialized]
    protected GameObject currentOwner;

    [System.NonSerialized]
    protected MonoBehaviour coroutineRunner;

    /// <summary>
    /// Activates this talent on the specified GameObject
    /// </summary>
    /// <param name="owner">The GameObject that will receive talent effects</param>
    /// <returns>True if activation was successful</returns>
    public virtual bool TryActivate(GameObject owner)
    {
        if (maxDesignatedPoints <= pointsDesignated)
            return false; // Already max level

        currentOwner = owner;

        // Find a MonoBehaviour to run coroutines if needed
        coroutineRunner = owner.GetComponent<TalentTree>();

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
        // Apply all modifiers for this talent
        var modifierData = GetModifierData();
        foreach (var data in modifierData)
        {
            var modifiable = GetModifiable(owner, data.TargetComponent);
            if (modifiable == null)
            {
                Debug.LogWarning(
                    $"Talent {talentName} requires an IModifiable component of type {data.TargetComponent} on {owner.name}"
                );
                continue;
            }

            ModifierHelper.AddModifier(modifiable, data.ModifierType, this, data.Modifier);
        }
    }

    /// <summary>
    /// Handles common talent deactivation logic
    /// </summary>
    protected virtual void OnDeactivate(GameObject owner)
    {
        // Find all IModifiable components on the owner and its children
        var modifiables = owner.GetComponentsInChildren<IModifiable>();
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
    /// Override to return the list of modifiers and their types for this talent
    /// </summary>
    protected virtual List<TalentModifierData> GetModifierData()
    {
        return new List<TalentModifierData>();
    }

    /// <summary>
    /// Gets the appropriate IModifiable component from the owner
    /// </summary>
    protected virtual IModifiable GetModifiable(GameObject owner, Type targetComponent)
    {
        if (targetComponent == null)
            return owner.GetComponent<IModifiable>();

        // Get component of specific type
        var component = owner.GetComponent(targetComponent);
        if (component is IModifiable modifiable)
            return modifiable;

        return null;
    }

    /// <summary>
    /// Data structure for talent modifiers
    /// </summary>
    protected class TalentModifierData
    {
        public ModifierType ModifierType { get; set; }
        public Delegate Modifier { get; set; }
        public Type TargetComponent { get; set; }

        public TalentModifierData(ModifierType type, Delegate modifier, Type targetComponent = null)
        {
            ModifierType = type;
            Modifier = modifier;
            TargetComponent = targetComponent;
        }
    }
}
