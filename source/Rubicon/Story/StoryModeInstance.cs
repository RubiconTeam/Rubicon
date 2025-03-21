using PukiTools.GodotSharp;
using Rubicon.Core;

namespace Rubicon.Story;

/// <summary>
/// Meant to control story mode related sequences here. Contains only songs for now, but should contain
/// things such as dialogue and video playing too.
/// </summary>
[GlobalClass, Autoload("StoryMode")]
public partial class StoryModeInstance : Node
{
    [Export] public int Index = 0;

    [Export] public StorySequence[] Playlist = [];
    
    [Export] public string Difficulty = ProjectSettings.GetSetting("rubicon/general/fallback/difficulty").AsString();

    public void Advance()
    {
        Index++;
        if (Index == Playlist.Length)
            return;
        
        Playlist[Index].Execute();
    }

    /// <summary>
    /// Gets the next queued song's name.
    /// </summary>
    /// <returns>The song name</returns>
    public string GetNextSong()
    {
        for (int i = Index; i < Playlist.Length; i++)
            if (Playlist[i] is SongStorySequence song)
                return song.Name;

        return null;
    }
}