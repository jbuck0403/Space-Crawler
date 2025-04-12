using System;
using System.Collections.Generic;

/// <summary>
/// Interface for objects that can receive modifiers
/// </summary>
public interface IModifiable
{
    /// <summary>
    /// Gets the dictionary of modifiers for this object
    /// </summary>
    Dictionary<ModifierType, List<(object Source, Delegate Modifier)>> Modifiers { get; }
}
