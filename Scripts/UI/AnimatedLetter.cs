namespace Rubicon.Extras.UI;

public class AnimatedLetter
{
    /// <summary>
    /// The current letter.
    /// Can be either a letter or a special character
    /// that will later be replaced with its alias inside <see cref="AnimatedFont"/>.
    /// </summary>
    public string Letter;
    
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
    /// The region rect for the texture to be sampled from.
    /// </summary>
    public Rect2[] SourceRect;
    
    /// <summary>
    /// Frames per second of the animation.
    /// Automatically taken from the SpriteFrames
    /// </summary>
    public double FrameSpeed = 24f;
    
    /// <summary>
    /// The frame that should be currently playing.
    /// </summary>
    public int FrameIndex = 0;
}