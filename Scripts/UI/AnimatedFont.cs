
using System.Linq;
using Godot.Collections;

namespace Rubicon.Extras.UI;
#if TOOLS
[Tool]
#endif
[GlobalClass] public partial class AnimatedFont : ReferenceRect
{
    [Export(PropertyHint.MultilineText)] public string Text = "Text Here";
    [Export] private Dictionary<string,string> _characterAliases = new();
    private AnimatedLetter[] _letterArray;

    [ExportGroup("Style"), Export(PropertyHint.File)] private SpriteFrames SpriteFrames;
    [Export] private float _separation = 3f;
    [Export] private float _fontSize = 24f;

    private AnimatedLetter[] GetLetterArray()
    {
        string[] splitText = Text.Split('\n');
        AnimatedLetter[] letterArray = new AnimatedLetter[splitText.Length];
        for (int i = 0; i < splitText.Length; i++)
        {
            string letter = splitText[i];
            AnimatedLetter animatedLetter = new AnimatedLetter()
            {
                Letter = letter
            };
            letterArray[i] = animatedLetter;
        }
        return letterArray;
    }

    public void UpdateSpriteFrames(SpriteFrames newSpriteFrames)
    {
        SpriteFrames = newSpriteFrames;
        UpdateLetters();
    }
    
    private void UpdateLetters()
    {
        _letterArray = GetLetterArray();
        for (int i = 0; i < _letterArray.Length; i++)
        {
            
        }
    }

    public override void _Draw()
    {
        for (int i = 0; i < _letterArray.Length; i++)
        {
            AnimatedLetter animatedLetter = _letterArray[i];
            if (animatedLetter.Texture != null && animatedLetter.Texture.Length > 0)
            {
                if (animatedLetter.Texture[animatedLetter.FrameIndex] is AtlasTexture letterAtlas)
                {
                    // AtlasTexture drawing
                    DrawTextureRectRegion(animatedLetter.Texture[animatedLetter.FrameIndex],
                        animatedLetter.Rect,
                        animatedLetter.SourceRect[animatedLetter.FrameIndex],
                        Modulate
                        );
                    
                    return;
                }
                // Drawing other Texture2D derivatives
                DrawTextureRect(animatedLetter.Texture[animatedLetter.FrameIndex],
                    animatedLetter.Rect,
                    false,
                    Modulate);
            }
        }
    }
}
