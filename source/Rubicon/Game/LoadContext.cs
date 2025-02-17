using Rubicon.Core.Meta;

namespace Rubicon.Game;

/// <summary>
/// An object that helps in defining what song to load in <see cref="RubiconGameInstance"/>.
/// </summary>
[GlobalClass]
public partial class LoadContext : RefCounted
{
    /// <summary>
    /// The name of the song to load.
    /// </summary>
    [Export] public string Name = ProjectSettings.GetSetting("rubicon/general/fallback/song").AsString();
    
    /// <summary>
    /// The difficulty to load.
    /// </summary>
    [Export] public string Difficulty = ProjectSettings.GetSetting("rubicon/general/fallback/difficulty").AsString();

    /// <summary>
    /// The rule set to play with.
    /// </summary>
    [Export] public string RuleSet = ProjectSettings.GetSetting("rubicon/rulesets/default_ruleset").AsString();

    /// <summary>
    /// Defines which bar line is chosen in <see cref="SongMeta.PlayableCharts"/>. Chooses the first index by default.
    /// </summary>
    [Export] public int TargetIndex = 0;

    /// <summary>
    /// Checks for any errors and reports any errors to the Godot console if any are found.
    /// </summary>
    /// <returns>Whether the context is fully valid.</returns>
    public bool IsValid()
    {
        /*
        if (!DirAccess.DirExistsAbsolute($"res://Songs/{Name}/"))
        {
            GD.Print($"Song {Name} does not exist. Falling back to default.");
            string fallBackSong = ProjectSettings.GetSetting("rubicon/general/fallback/song").AsString();
            if (!DirAccess.DirExistsAbsolute($"res://Songs/{fallBackSong}/")) // You fucked up bro
            {
                GD.PrintErr("Song fallback failed to load. Please check your Project Settings at \"rubicon/general/fallback/song\"");
                return false;
            }

            Name = fallBackSong;
        }

        if (!ResourceLoader.Exists($"res://Songs/{Name}/Data/Meta.tres"))
        {
            GD.PrintErr($"Metadata for song {Name} was not found.");
            return false;
        }

        if (!FileAccess.FileExists($"res://Songs/{Name}/Data/{RuleSet}-{Difficulty}.rbc") && !FileAccess.FileExists($"res://Songs/{Name}/Data/{RuleSet}-{Difficulty}.trbc"))
        {
            GD.Print($"Chart for difficulty {RuleSet}-{Difficulty} was not found for song {Name}. Falling back to default.");
            string fallBackSet = ProjectSettings.GetSetting("rubicon/rulesets/default_ruleset").AsString();
            string fallBackDiff = ProjectSettings.GetSetting("rubicon/general/fallback/difficulty").AsString();
            string fallBackChart = $"{fallBackSet}-{fallBackDiff}";
            if (!ResourceLoader.Exists($"res://Songs/{Name}/Data/{fallBackChart}.tres")) // You fucked up again bro
            {
                GD.PrintErr("Chart fallback failed to load. Please check your Project Settings at \"rubicon/general/fallback/difficulty\" and \"rubicon/rulesets/default_ruleset\"");
                return false;
            }

            Difficulty = fallBackDiff;
            RuleSet = fallBackSet;
        }
        */

        return true;
    }
}