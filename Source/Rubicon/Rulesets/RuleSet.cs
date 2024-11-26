using Rubicon.Core.Data;

namespace Rubicon.Rulesets;

/// <summary>
/// A base ruleset for any Rubicon ruleset
/// </summary>
[GlobalClass]
public partial class RuleSet : Resource
{
    /// <summary>
    /// The unique identifier for this ruleset.
    /// </summary>
    [Export] public string UniqueId;
    
    /// <summary>
    /// The name of the ruleset.
    /// </summary>
    [Export] public string Name;

    /// <summary>
    /// The shortened name of this ruleset.
    /// </summary>
    [Export] public string ShortName;

    /// <summary>
    /// The version this ruleset is on.
    /// </summary>
    [Export] public VersionInfo Version = RubiconEngineInstance.Version;

    /// <summary>
    /// Mainly for Discord RPC, will display this verb while you are playing.
    /// </summary>
    [Export] public string PlayingVerb;

    /// <summary>
    /// The <see cref="PlayField"/> script that this ruleset is associated with.
    /// </summary>
    [Export] public CSharpScript PlayFieldScript; // Maybe in the future we can support GDScript
}