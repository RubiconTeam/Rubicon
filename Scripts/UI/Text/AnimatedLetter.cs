namespace Rubicon.Extras.UI;

public class AnimatedLetter(string letter)
{
    
    /// <summary>
    /// The current letter.
    /// Can be either a letter or a special character
    /// that will later be replaced with its alias inside <see cref="AnimatedFont"/>.
    /// </summary>
    public readonly string Letter = letter;
    
    /// <summary>
    /// The texture array that caches every frame of the animation.
    /// </summary>
    public Texture2D[] Texture;
    
    /// <summary>
    /// The letter's Rect2
    /// Represents position and scale of the letter.
    /// </summary>
    public Rect2 Rect;
    
    /// <summary>
    /// Frames per second of the animation.
    /// Automatically taken from the SpriteFrames.
    /// Only works if the animation style is set to InstantLoop.
    /// </summary>
    public float FrameSpeed = 24f;
    
    /// <summary>
    /// The frame that should be currently playing.
    /// Only works if the animation style is set to InstantLoop.
    /// </summary>
    public int FrameIndex = 0;
}