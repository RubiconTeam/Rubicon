namespace Rubicon.Story;

/// <summary>
/// Serves as an event for the story mode instance to execute when reached.
/// </summary>
[GlobalClass]
public abstract partial class StorySequence : Resource
{
    /// <summary>
    /// Executes upon <see cref="StoryModeInstance"/> reaching its index.
    /// </summary>
    public abstract void Execute();
}