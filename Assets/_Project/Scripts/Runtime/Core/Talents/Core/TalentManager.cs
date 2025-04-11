using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Central manager for all talent-related delegates and modifications
/// </summary>
public class TalentManager : MonoBehaviour
{
    private static TalentManager _instance;

    /// <summary>
    /// Singleton instance
    /// </summary>
    public static TalentManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("TalentManager");
                _instance = go.AddComponent<TalentManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    // All registered modifiers
    private List<TalentModifierData> _modifiers = new List<TalentModifierData>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Register a modifier with the talent system
    /// </summary>
    /// <typeparam name="T">The delegate type</typeparam>
    /// <param name="type">The modification point</param>
    /// <param name="owner">The GameObject this modifier applies to</param>
    /// <param name="talent">The talent registering the modifier</param>
    /// <param name="modifierFn">The delegate function</param>
    public void RegisterModifier<T>(
        ModifierType type,
        GameObject owner,
        BaseTalent talent,
        T modifierFn
    )
        where T : Delegate
    {
        // Remove any existing modifiers of the same type from this talent for this owner
        _modifiers.RemoveAll(m => m.Type == type && m.Owner == owner && m.SourceTalent == talent);

        // Add the new modifier
        _modifiers.Add(
            new TalentModifierData
            {
                Type = type,
                Owner = owner,
                SourceTalent = talent,
                ModifierDelegate = modifierFn,
                TalentLevel = talent.pointsSpent
            }
        );

        Debug.Log(
            $"Registered {type} modifier for {owner.name} from talent {talent.talentName} at level {talent.pointsSpent}"
        );
    }

    /// <summary>
    /// Remove all modifiers from a specific talent for a specific owner
    /// </summary>
    /// <param name="talent">The talent to remove modifiers for</param>
    /// <param name="owner">The GameObject the modifiers apply to</param>
    public void RemoveModifiers(BaseTalent talent, GameObject owner)
    {
        int count = _modifiers.RemoveAll(m => m.SourceTalent == talent && m.Owner == owner);
        if (count > 0)
        {
            Debug.Log(
                $"Removed {count} modifiers for {owner.name} from talent {talent.talentName}"
            );
        }
    }

    /// <summary>
    /// Get all modifiers of a specific type for a specific owner
    /// </summary>
    /// <typeparam name="T">The delegate type to return</typeparam>
    /// <param name="type">The modification point to get modifiers for</param>
    /// <param name="owner">The GameObject the modifiers apply to</param>
    /// <returns>List of matching delegate functions</returns>
    public List<T> GetModifiers<T>(ModifierType type, GameObject owner)
        where T : Delegate
    {
        return _modifiers
            .Where(m => m.Type == type && m.Owner == owner)
            .Select(m => m.ModifierDelegate as T)
            .Where(d => d != null)
            .ToList();
    }

    /// <summary>
    /// Apply all float modifier functions to a value
    /// </summary>
    /// <param name="type">The modification point</param>
    /// <param name="owner">The GameObject the modifiers apply to</param>
    /// <param name="baseValue">The starting value to modify</param>
    /// <returns>The modified value after all modifiers are applied</returns>
    public float ApplyFloatModifiers(ModifierType type, GameObject owner, float baseValue)
    {
        var modifierFuncs = GetModifiers<Func<float, float>>(type, owner);
        float result = baseValue;

        foreach (var modFunc in modifierFuncs)
        {
            result = modFunc(result);
        }

        return result;
    }

    /// <summary>
    /// Invoke all action modifiers
    /// </summary>
    /// <typeparam name="T">The parameter type for the action</typeparam>
    /// <param name="type">The modification point</param>
    /// <param name="owner">The GameObject the modifiers apply to</param>
    /// <param name="arg">The argument to pass to the actions</param>
    public void InvokeActionModifiers<T>(ModifierType type, GameObject owner, T arg)
    {
        var actions = GetModifiers<Action<T>>(type, owner);
        foreach (var action in actions)
        {
            action(arg);
        }
    }

    /// <summary>
    /// Invoke all parameterless action modifiers
    /// </summary>
    /// <param name="type">The modification point</param>
    /// <param name="owner">The GameObject the modifiers apply to</param>
    public void InvokeActionModifiers(ModifierType type, GameObject owner)
    {
        var actions = GetModifiers<Action>(type, owner);
        foreach (var action in actions)
        {
            action();
        }
    }

    /// <summary>
    /// Check if any modifier of a specific type exists for an owner
    /// </summary>
    /// <param name="type">The modification point</param>
    /// <param name="owner">The GameObject the modifiers apply to</param>
    /// <returns>True if at least one matching modifier exists</returns>
    public bool HasModifiers(ModifierType type, GameObject owner)
    {
        return _modifiers.Any(m => m.Type == type && m.Owner == owner);
    }

    /// <summary>
    /// Remove all modifiers
    /// </summary>
    public void ClearAllModifiers()
    {
        _modifiers.Clear();
        Debug.Log("Cleared all talent modifiers");
    }
}
