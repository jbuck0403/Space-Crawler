using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Static helper class for working with modifiers
/// </summary>
public static class ModifierHelper
{
    /// <summary>
    /// Adds a modifier to a modifiable object
    /// </summary>
    /// <param name="modifiable">The object to modify</param>
    /// <param name="type">The type of modification</param>
    /// <param name="source">The source adding the modifier</param>
    /// <param name="modifier">The delegate function</param>
    public static void AddModifier(
        IModifiable modifiable,
        ModifierType type,
        object source,
        Delegate modifier
    )
    {
        var modifiers = modifiable.Modifiers;

        // Create the list if it doesn't exist
        if (!modifiers.ContainsKey(type))
            modifiers[type] = new List<(object, Delegate)>();

        // Add the modifier
        modifiers[type].Add((source, modifier));

        // Get source name for logging
        string sourceName = GetSourceName(source);
        Debug.Log($"Added {type} modifier from {sourceName}");
    }

    /// <summary>
    /// Removes all modifiers from a specific source
    /// </summary>
    /// <param name="modifiable">The object to modify</param>
    /// <param name="source">The source to remove modifiers from</param>
    public static void RemoveModifiersFromSource(IModifiable modifiable, object source)
    {
        var modifiers = modifiable.Modifiers;

        int totalRemoved = 0;
        foreach (var type in modifiers.Keys.ToList())
        {
            int countBefore = modifiers[type].Count;
            modifiers[type].RemoveAll(entry => entry.Source.Equals(source));
            totalRemoved += (countBefore - modifiers[type].Count);
        }

        if (totalRemoved > 0)
        {
            string sourceName = GetSourceName(source);
            Debug.Log($"Removed {totalRemoved} modifiers from {sourceName}");
        }
    }

    /// <summary>
    /// Gets all modifiers of a specific type
    /// </summary>
    /// <typeparam name="T">The delegate type</typeparam>
    /// <param name="modifiable">The object with modifiers</param>
    /// <param name="type">The modification point</param>
    /// <returns>List of delegates</returns>
    public static List<T> GetModifiers<T>(IModifiable modifiable, ModifierType type)
        where T : Delegate
    {
        var modifiers = modifiable.Modifiers;

        if (!modifiers.TryGetValue(type, out var modifierList))
            return new List<T>();

        List<T> result = new List<T>(modifierList.Count);

        foreach (var entry in modifierList)
        {
            if (entry.Modifier is T typedModifier)
                result.Add(typedModifier);
        }

        return result;
    }

    // /// <summary>
    // /// Gets all modifiers of a specific type without requiring type specification
    // /// </summary>
    // /// <param name="modifiable">The object with modifiers</param>
    // /// <param name="type">The modification point</param>
    // /// <returns>List of raw delegates</returns>
    // public static List<Delegate> GetModifiers(IModifiable modifiable, ModifierType type)
    // {
    //     var modifiers = modifiable.Modifiers;

    //     if (!modifiers.TryGetValue(type, out var modifierList))
    //         return new List<Delegate>();

    //     return modifierList.Select(entry => entry.Modifier).ToList();
    // }

    /// <summary>
    /// Checks if any modifiers exist for a specific type
    /// </summary>
    /// <param name="modifiable">The object to check</param>
    /// <param name="type">The modification point to check</param>
    /// <returns>True if any modifiers exist</returns>
    public static bool HasModifiers(IModifiable modifiable, ModifierType type)
    {
        var modifiers = modifiable.Modifiers;
        return modifiers.ContainsKey(type) && modifiers[type].Count > 0;
    }

    /// <summary>
    /// Clears all modifiers
    /// </summary>
    /// <param name="modifiable">The object to clear modifiers from</param>
    public static void ClearAllModifiers(IModifiable modifiable)
    {
        modifiable.Modifiers.Clear();
    }

    /// <summary>
    /// Gets a user-friendly name for a source object
    /// </summary>
    private static string GetSourceName(object source)
    {
        if (source == null)
            return "null";

        if (source is BaseTalent talent)
            return $"talent {talent.talentName}";

        if (source is UnityEngine.Object unityObj)
            return unityObj.name;

        return source.GetType().Name;
    }
}
