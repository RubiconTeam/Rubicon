namespace Rubicon.Extras.UI;

public enum AnimationStyles
{
    /// <summary>
    /// All the letters will play at the same time,
    /// controlled only by the <see cref="AnimatedFont"/>.
    /// 
    /// If an animation finishes before the longest one,
    /// it will pause until the other animations are finished.
    /// 
    /// This is the best performant method.
    /// </summary>
    Synchronized,
    
    /// <summary>
    /// If a letter's animation finishes before the longest one
    /// it will instantly loop back and desynchronize from the others.
    /// 
    /// This method will be less performant since it has to track
    /// the current frame for every letter.
    /// </summary>
    InstantLoop,
}