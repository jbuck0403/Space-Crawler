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
    public int pointCost = 1;

    [Header("Talent Requirements")]
    public List<BaseTalent> requiredTalents;

    // Runtime state - not serialized
    [HideInInspector]
    public bool isActivated = false;

    // Dictionary to track GameObject instances this talent is applied to
    protected Dictionary<GameObject, bool> appliedInstances = new Dictionary<GameObject, bool>();

    /// <summary>
    /// Activates this talent on the specified GameObject
    /// </summary>
    /// <param name="owner">The GameObject that will receive talent effects</param>
    /// <returns>True if activation was successful</returns>
    public virtual bool TryActivate(GameObject owner)
    {
        if (appliedInstances.ContainsKey(owner) && appliedInstances[owner])
            return false; // Already activated on this object

        // Apply talent effects
        appliedInstances[owner] = true;
        OnActivate(owner);

        return true;
    }

    /// <summary>
    /// Deactivates this talent on the specified GameObject
    /// </summary>
    /// <param name="owner">The GameObject to remove talent effects from</param>
    public virtual void Deactivate(GameObject owner)
    {
        if (!appliedInstances.ContainsKey(owner) || !appliedInstances[owner])
            return; // Not active on this object

        // Remove talent effects
        OnDeactivate(owner);
        appliedInstances[owner] = false;
    }

    /// <summary>
    /// Implemented by derived classes to apply talent effects
    /// </summary>
    protected abstract void OnActivate(GameObject owner);

    /// <summary>
    /// Implemented by derived classes to clean up talent effects
    /// </summary>
    protected abstract void OnDeactivate(GameObject owner);

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
}
