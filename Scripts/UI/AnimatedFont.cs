
using System.Linq;
using Godot.Collections;

namespace Rubicon.Extras.UI;
#if TOOLS
[Tool]
#endif
[GlobalClass] public partial class AnimatedFont : ReferenceRect
{
    [Export(PropertyHint.MultilineText)]
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            UpdateText();
        }
    }
    
    
    [Export] private Dictionary<string,string> _characterAliases = new();
    private AnimatedLetter[] _letterArray;

    [ExportGroup("Style"), Export(PropertyHint.File)]
    public SpriteFrames SpriteFrames
    {
        get => _spriteFrames;
        set
        {
            _spriteFrames = value;
            UpdateSpriteFrames();
        }
    }
    
    
    [Export] private float _separation = 3f;
    [Export] private float _fontSize = 24f;

    private string _text;
    private SpriteFrames _spriteFrames;

    public override void _Ready()
    {
        UpdateText();
    }

    private AnimatedLetter[] GetLetterArray()
    {
        string[] splitText = Text.Select(x => x.ToString()).ToArray();
        AnimatedLetter[] letterArray = new AnimatedLetter[splitText.Length];
        for (int i = 0; i < splitText.Length; i++)
        {
            string letter = splitText[i];
            AnimatedLetter animatedLetter = new AnimatedLetter()
            {
                Letter = letter
            };
            letterArray[i] = animatedLetter;
            GD.Print(animatedLetter.Letter);
        }
        return letterArray;
    }

    public void UpdateSpriteFrames()
    {
        UpdateText();
    }
    
    private void UpdateText()
    {
        _letterArray = GetLetterArray();
        for (int i = 0; i < _letterArray.Length; i++)
        {
            AnimatedLetter animatedLetter = _letterArray[i];
            GD.Print("SPRITE FRAMES IIISS NULL !? ", SpriteFrames is null);
            animatedLetter.Texture = new Texture2D[SpriteFrames.GetFrameCount(animatedLetter.Letter)];
            animatedLetter.Rect = new Rect2(new Vector2(_separation * i, 0), new Vector2(_fontSize, _fontSize));
                
            for (int frame = 0; frame < SpriteFrames.GetFrameCount(animatedLetter.Letter); frame++)
            {
                Texture2D frameTexture = SpriteFrames.GetFrameTexture(animatedLetter.Letter, frame);
                animatedLetter.Texture[frame] = frameTexture;
                animatedLetter.FrameSpeed = SpriteFrames.GetAnimationSpeed(animatedLetter.Letter);


                if (frameTexture is AtlasTexture atlasTexture)
                {
                    animatedLetter.SourceRect[frame] = atlasTexture.GetRegion();
                }
            }
        }
        QueueRedraw();
    }

    public override void _Draw()
    {
        GD.Print("DAH");
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
