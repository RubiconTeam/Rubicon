
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
    
    public override void _Ready()
    {
        _letterArray = GetLetterArray();
    }

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

    private Rect2[] GetLetterRects()
    {
        Rect2[] letterRects = new Rect2[_letterArray.Length];
        /*foreach (string letter in _letterArray)
        {
            letter 
        }*/
        return letterRects;
    }

    public void UpdateSpriteFrames(SpriteFrames newSpriteFrames)
    {
        SpriteFrames = newSpriteFrames;
        UpdateLetters();
    }
    
    private void UpdateLetters()
    {
        
    }

    public override void _Draw()
    {
        for (int i = 0; i < _letterArray.Length; i++)
        {
            AnimatedLetter animatedLetter = _letterArray[i];
            if (SpriteFrames != null && SpriteFrames.HasAnimation(animatedLetter.Letter))
            {
                if (SpriteFrames.GetFrameTexture(animatedLetter.Letter, animatedLetter.FrameIndex) is AtlasTexture spriteAtlas)
                {
                    //atlas texture handling
                    return;
                }
                //texture2d handling
                
            }
        }
    }
}
