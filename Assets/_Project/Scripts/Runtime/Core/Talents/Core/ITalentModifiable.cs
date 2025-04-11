using System;
using System.Collections.Generic;

/// <summary>
/// Interface for objects that can receive talent modifiers
/// </summary>
public interface ITalentModifiable
{
    /// <summary>
    /// Gets the dictionary of modifiers for this object
    /// </summary>
    Dictionary<ModifierType, List<(BaseTalent Source, Delegate Modifier)>> TalentModifiers { get; }
}
