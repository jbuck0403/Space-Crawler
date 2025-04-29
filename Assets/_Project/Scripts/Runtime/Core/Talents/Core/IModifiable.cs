using System;
using System.Collections.Generic;

/// <summary>
/// Interface for objects that can receive modifiers
/// </summary>
public interface IModifiable
{
    Dictionary<ModifierType, List<(object Source, Delegate Modifier)>> Modifiers { get; }
}
