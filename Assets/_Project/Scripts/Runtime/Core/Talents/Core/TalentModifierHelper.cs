using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Static helper class for working with talent modifiers
/// </summary>
public static class TalentModifierHelper
{
    /// <summary>
    /// Adds a modifier to a talent-modifiable object
    /// </summary>
    /// <param name="modifiable">The object to modify</param>
    /// <param name="type">The type of modification</param>
    /// <param name="source">The talent adding the modifier</param>
    /// <param name="modifier">The delegate function</param>
    public static void AddModifier(
        ITalentModifiable modifiable,
        ModifierType type,
        BaseTalent source,
        Delegate modifier
    )
    {
        var modifiers = modifiable.TalentModifiers;

        // Create the list if it doesn't exist
        if (!modifiers.ContainsKey(type))
            modifiers[type] = new List<(BaseTalent, Delegate)>();

        // Add the modifier
        modifiers[type].Add((source, modifier));

        Debug.Log(
            $"Added {type} modifier from talent {source.talentName} (Level {source.pointsSpent})"
        );
    }

    /// <summary>
    /// Removes all modifiers from a specific talent
    /// </summary>
    /// <param name="modifiable">The object to modify</param>
    /// <param name="source">The talent to remove modifiers from</param>
    public static void RemoveModifiersFromTalent(ITalentModifiable modifiable, BaseTalent source)
    {
        var modifiers = modifiable.TalentModifiers;

        int totalRemoved = 0;
        foreach (var type in modifiers.Keys.ToList())
        {
            int countBefore = modifiers[type].Count;
            modifiers[type].RemoveAll(entry => entry.Source == source);
            totalRemoved += (countBefore - modifiers[type].Count);
        }

        if (totalRemoved > 0)
        {
            Debug.Log($"Removed {totalRemoved} modifiers from talent {source.talentName}");
        }
    }

    /// <summary>
    /// Gets all modifiers of a specific type
    /// </summary>
    /// <typeparam name="T">The delegate type</typeparam>
    /// <param name="modifiable">The object with modifiers</param>
    /// <param name="type">The modification point</param>
    /// <returns>List of delegates</returns>
    public static List<T> GetModifiers<T>(ITalentModifiable modifiable, ModifierType type)
        where T : Delegate
    {
        var modifiers = modifiable.TalentModifiers;

        if (!modifiers.TryGetValue(type, out var modifierList))
            return new List<T>();

        return modifierList
            .Where(entry => entry.Modifier is T)
            .Select(entry => entry.Modifier as T)
            .ToList();
    }

    /// <summary>
    /// Checks if any modifiers exist for a specific type
    /// </summary>
    /// <param name="modifiable">The object to check</param>
    /// <param name="type">The modification point to check</param>
    /// <returns>True if any modifiers exist</returns>
    public static bool HasModifiers(ITalentModifiable modifiable, ModifierType type)
    {
        var modifiers = modifiable.TalentModifiers;
        return modifiers.ContainsKey(type) && modifiers[type].Count > 0;
    }

    /// <summary>
    /// Clears all modifiers
    /// </summary>
    /// <param name="modifiable">The object to clear modifiers from</param>
    public static void ClearAllModifiers(ITalentModifiable modifiable)
    {
        modifiable.TalentModifiers.Clear();
    }
}
