using System;
using UnityEngine;

/// <summary>
/// Data container for talent modifiers
/// </summary>
public class TalentModifierData
{
    /// <summary>
    /// The type of modifier
    /// </summary>
    public ModifierType Type { get; set; }

    /// <summary>
    /// The delegate instance that performs the modification
    /// </summary>
    public Delegate ModifierDelegate { get; set; }

    /// <summary>
    /// The GameObject this modifier applies to
    /// </summary>
    public GameObject Owner { get; set; }

    /// <summary>
    /// The talent that created this modifier
    /// </summary>
    public BaseTalent SourceTalent { get; set; }

    /// <summary>
    /// The current level of the talent when this modifier was created
    /// </summary>
    public int TalentLevel { get; set; }
}
