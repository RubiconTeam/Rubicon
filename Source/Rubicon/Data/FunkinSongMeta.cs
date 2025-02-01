using Rubicon.Core.Meta;

namespace Rubicon.Data;

/// <summary>
/// Used to hold important information about a Funkin' song.
/// </summary>
[GlobalClass] public partial class FunkinSongMeta : SongMeta
{
    /// <summary>
    /// The vocals for this song.
    /// </summary>
    [Export] public AudioStream Vocals;
    
    /// <summary>
    /// The characters to spawn in the song.
    /// </summary>
    [Export] public CharacterMeta[] Characters = [];

    /// <summary>
    /// Determines what type of backend the engine will use when loading into a song.
    /// </summary>
    [Export] public GameEnvironment Environment = GameEnvironment.None;

    /// <summary>
    /// The stage to spawn in for this song.
    /// </summary>
    [Export] public string Stage = "stage";
}