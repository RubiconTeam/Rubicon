
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

    [ExportGroup("Animation"), Export(PropertyHint.File)]
    public SpriteFrames SpriteFrames
    {
        get => _spriteFrames;
        set
        {
            _spriteFrames = value;
            UpdateText();
        }
    }
    
    [Export(PropertyHint.Enum)] private AnimationStyles _animationStyles;
    
    
    [Export, ExportGroup("Style")] private float _separation = 24f;
    [Export] private float _fontSize = 24f;
    [Export] private HorizontalAlignment _horizontalAlignment = 0;
    [Export] private VerticalAlignment _verticalAlignment = 0;

    private double _syncFrameIndex = 0;

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
            AnimatedLetter animatedLetter = new AnimatedLetter(letter);
            letterArray[i] = animatedLetter;
        }
        return letterArray;
    }

    private void UpdateLetterPositions(bool redraw = true)
    {
        for (int i = 0; i < _letterArray.Length; i++)
        {
            AnimatedLetter animLetter = _letterArray[i];
            animLetter.Rect = new Rect2(new Vector2(_separation * i, 0), new Vector2(_fontSize, _fontSize));
        }
        CustomMinimumSize = new Vector2(_separation * _letterArray.Length + _fontSize, _fontSize);
        if (redraw)
            QueueRedraw();
    }
    
    private void UpdateText()
    {
        _letterArray = GetLetterArray();
        foreach (AnimatedLetter animLetter in _letterArray)
        {
            if (SpriteFrames == null)
            {
                GD.PrintErr("The provided SpriteFrames is null");
                return;
            }

            if (!SpriteFrames.HasAnimation(animLetter.Letter))
                continue;
            
            //GD.Print(animLetter.Letter);
            int frameCount = SpriteFrames.GetFrameCount(animLetter.Letter);
            //GD.Print($"Frame count {frameCount}");
            animLetter.Texture = new Texture2D[frameCount];
            UpdateLetterPositions(false);
                
            for (int frame = 0; frame < frameCount; frame++)
            {
                Texture2D frameTexture = SpriteFrames.GetFrameTexture(animLetter.Letter, frame);
                animLetter.Texture[frame] = frameTexture;
                
                if (_animationStyles == AnimationStyles.InstantLoop)
                    animLetter.FrameSpeed = SpriteFrames.GetAnimationSpeed(animLetter.Letter);

                //GD.Print($"Texture {frameTexture} for frame {frame} of letter {animLetter.Letter}");
                
                /*if (frameTexture is AtlasTexture atlasTexture)
                {
                    animatedLetter.SourceRect[frame] = atlasTexture.Region;
                }*/
            }
            //GD.Print($"Done setting {SpriteFrames.GetFrameCount(animLetter.Letter)} textures for letter: {animLetter.Letter}");
        }
        QueueRedraw();
    }

    public override void _Draw()
    {
        foreach (AnimatedLetter animLetter in _letterArray)
        {
            /*if (animLetter.Texture != null)
            {*/
                if (animLetter.Texture[animLetter.FrameIndex] is AtlasTexture letterAtlas)
                {
                    GD.Print($"drawing atlastexture letter {animLetter.Letter}");
                    // AtlasTexture drawing
                    Texture2D atlas = letterAtlas.Atlas;
                    Rect2 sourceRect = letterAtlas.Region;
                    DrawTextureRectRegion(atlas,
                        animLetter.Rect,
                        sourceRect,
                        Modulate
                        );
                    
                    continue;
                }
                //GD.Print($"drawing texture2d letter {animLetter.Letter}");
                // Drawing other Texture2D derivatives
                DrawTextureRect(animLetter.Texture[animLetter.FrameIndex],
                    animLetter.Rect,
                    false,
                    Modulate);
            //}
        }
    }
    
    public override void _PhysicsProcess(double delta)
    {
        
    }
}
