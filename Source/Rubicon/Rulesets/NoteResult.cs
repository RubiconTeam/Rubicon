using Rubicon.Core.Data;

namespace Rubicon.Rulesets;

/// <summary>
/// Flags for NoteResult. Will prevent the action from being activated.
/// </summary>
[Flags] public enum NoteResultFlags : uint
{
    None = 0b00000000,
    Health = 0b00000001,
    Score = 0b00000010,
    Splash = 0b00000100,
    Animation = 0b00001000
}

/// <summary>
/// An object one can modify to prevent actions from passing through in, or modify the rating of the note just hit.
/// </summary>
[GlobalClass] public partial class NoteResult : GodotObject
{
    /// <summary>
    /// An easily accessible NoteEventResult that does nothing.
    /// </summary>
    public static NoteResult Nothing { get; private set; } = new(NoteResultFlags.None);
        
    /// <summary>
    /// An easily accessible NoteEventResult that does nothing BUT will grant you a miss.
    /// </summary>
    public static NoteResult NothingMiss { get; private set; } = new(NoteResultFlags.None, HitType.Miss);

    /// <summary>
    /// Essentially the rating of the note hit. You can modify what type of rating this is before the note goes through ChartController!
    /// </summary>
    [Export] public HitType Hit;
        
    /// <summary>
    /// Flags for the ChartController to handle. Adding flags onto this will PREVENT the action from happening!
    /// </summary>
    [Export] public uint ProcessFlags;

    /// <summary>
    /// Empty initializer, will essentially create NoteEventResult.Nothing.
    /// </summary>
    public NoteResult() : this(NoteResultFlags.None, HitType.None) { }
        
    /// <summary>
    /// Creates a new NoteResult that does nothing, but will pass the hit type / rating on.
    /// </summary>
    /// <param name="hitType"></param>
    public NoteResult(HitType hitType) : this(NoteResultFlags.None, hitType) { }
        
    /// <summary>
    /// Creates a new result to be used on NoteManagers.
    /// </summary>
    /// <param name="flags">Flags for the game to handle.</param>
    /// <param name="hitType">The type of note hit, or the rating</param>
    public NoteResult(NoteResultFlags flags, HitType hitType = HitType.None) : this((uint)flags, hitType) { }
    
    /// <summary>
    /// Creates a new result to be used on NoteManagers.
    /// </summary>
    /// <param name="flags">Flags for the game to handle, with a custom enumerator. Make sure it relies on uint.</param>
    /// <param name="hitType">The type of note hit, or the ratinf</param>
    public NoteResult(Enum flags, HitType hitType = HitType.None) : this(Convert.ToUInt32(flags), hitType) { }
    
    /// <summary>
    /// Creates a new result to be used on NoteManagers.
    /// </summary>
    /// <param name="flags">Flags for the game to handle, in raw uint.</param>
    /// <param name="hitType">The type of note hit, or the rating</param>
    public NoteResult(uint flags, HitType hitType = HitType.None)
    {
        Hit = hitType;
        ProcessFlags = flags;
    }

    /// <summary>
    /// Checks whether the result has the specified flag.
    /// </summary>
    /// <param name="flag">The flag to check for</param>
    /// <returns>If the process flags contains this flag.</returns>
    public bool HasFlag(NoteResultFlags flag) => HasFlag(Convert.ToUInt32(flag));

    /// <summary>
    /// Checks whether the result has the specified flag.
    /// </summary>
    /// <param name="flag">The flag to check for</param>
    /// <returns>If the process flags contains this flag.</returns>
    public bool HasFlag(uint flag) => (ProcessFlags & flag) == flag;

    public bool Equals(NoteResult other)
    {
        return Hit == other.Hit && ProcessFlags == other.ProcessFlags;
    }
}