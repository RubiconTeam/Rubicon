namespace Rubicon.Data;

/// <summary>
/// Determines how a character holds notes animation-wise.
/// </summary>
public enum CharacterHold : uint
{
    /// <summary>
    /// The character will just not hold a note at all, similar to V-Slice.
    /// </summary>
    None,
    
    /// <summary>
    /// The character will jitter similar to how Fantasy Engine does (HoloFunk)
    /// </summary>
    Jitter,
    
    /// <summary>
    /// The character will jitter every step, similar to every other Funkin' engine.
    /// </summary>
    StepJitter,
    
    /// <summary>
    /// The character will freeze in place until the hold note is completed.
    /// </summary>
    Freeze
}