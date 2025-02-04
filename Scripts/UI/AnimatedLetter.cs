namespace Rubicon.Extras.UI;

public class AnimatedLetter
{
    /// <summary>
    /// The current letter.
    /// Can be either a letter or a special character
    /// that will later be replaced with its alias inside <see cref="AnimatedFont"/>.
    /// </summary>
    public string Letter;
    
    public Texture2D[] Texture;
    public Rect2 Rect;
    public Rect2[] SourceRect;
    public float FrameSpeed = 24f;
    public int FrameIndex = 0;

    
}