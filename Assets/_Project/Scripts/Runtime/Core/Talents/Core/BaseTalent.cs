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
            return false; // Already activated on this object

        OnPointAdded(owner);

        return true;
    }

    /// <summary>
    /// Deactivates this talent on the specified GameObject
    /// </summary>
    /// <param name="owner">The GameObject to remove talent effects from</param>
    public virtual void Deactivate(GameObject owner)
    {
        if (pointsDesignated > 0)
            return; // Not active on this object

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
