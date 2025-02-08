
using System.Linq;
using Godot.Collections;

namespace Rubicon.Extras.UI;
#if TOOLS
[Tool, Icon("res://Assets/UI/Misc/AnimatedLabel.svg")]
#endif
[GlobalClass] public partial class AnimatedLabel : ReferenceRect
{
    [Export(PropertyHint.MultilineText)] public string Text
    {
        get => _text;
        set
        {
            _text = value;
            _letterArray = GetLetterArray();
            UpdateText();
        }
    }
    
    [Export(PropertyHint.File)] public AnimatedFont AnimatedFont
    {
        get => _animatedFont;
        set
        {
            _animatedFont = value;
            UpdateText();
        }
    }
    
    [ExportGroup("Animation"), Export(PropertyHint.Enum)] 
    private AnimationStyles AnimationStyles
    {
        get => _animationStyles;
        set
        {
            _animationStyles = value;
            UpdateText();
        }
    }
    
    [Export] public float SyncFrameSpeed
    {
        get => _syncFrameSpeed;
        set
        {
            _syncFrameSpeed = value;
            _syncFrameLength = 1 / _syncFrameSpeed;
        }
    }

    [ExportGroup("Style"), Export] public float Separation
    {
        get => _separation;
        set
        {
            _separation = value;
            UpdateText();
        }
    }
    
    [Export] public float FontSize
    {
        get => _fontSize;
        set
        {
            _fontSize = value;
            UpdateText();
        }
    }
    
    [Export] public HorizontalAlignment HorizontalAlignment
    {
        get => _horizontalAlignment;
        set
        {
            _horizontalAlignment = value;
            UpdateText();
        }
    }
    
    [Export] public VerticalAlignment VerticalAlignment
    {
        get => _verticalAlignment;
        set
        {
            _verticalAlignment = value;
            UpdateText();
        }
    }

    private string _text;
    private AnimatedFont _animatedFont;
    private AnimationStyles _animationStyles = AnimationStyles.Synchronized;
    private float _separation = 24f;
    private float _fontSize = 24f;
    private HorizontalAlignment _horizontalAlignment = HorizontalAlignment.Left;
    private VerticalAlignment _verticalAlignment = VerticalAlignment.Top;
    
    private AnimatedLetter[] _letterArray;
    private float _syncFrameSpeed = 24f;
    private float _syncFrameLength;
    private int _syncFrameIndex = 0;

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
            
            // basic position for spaces or texture-less letters
            Vector2 position = new Vector2(Separation * i, 0);
            Vector2 size = new Vector2(FontSize, FontSize);
            
            if (animLetter.Texture != null && animLetter.Texture[0] != null)
            {
                Vector2 texSize = animLetter.Texture[0].GetSize();
                
                size = CalculateLetterSize(texSize);
                position = new Vector2(Separation * i, FontSize - size.Y);
            }
            
            animLetter.Rect = new Rect2(position, size);
        }
        CustomMinimumSize = new Vector2(Separation * _letterArray.Length + FontSize, FontSize);
        if (redraw)
            QueueRedraw();
    }

    private Vector2 CalculateLetterSize(Vector2 texSize)
    {
        float scale = FontSize / texSize.X;
        
        Vector2 newSize = new Vector2(texSize.X * scale, FontSize);
        return newSize;
    }
    
    private void UpdateText()
    {
        foreach (AnimatedLetter animLetter in _letterArray)
        {
            if (AnimatedFont == null)
            {
                GD.PrintErr("The provided AnimatedFont is null");
                return;
            }

            if (!AnimatedFont.SpriteFrames.HasAnimation(animLetter.Letter))
                continue;
            
            //GD.Print(animLetter.Letter);
            int frameCount = AnimatedFont.SpriteFrames.GetFrameCount(animLetter.Letter);
            //GD.Print($"Frame count {frameCount}");
            animLetter.Texture = new Texture2D[frameCount];
            UpdateLetterPositions(false);
                
            for (int frame = 0; frame < frameCount; frame++)
            {
                Texture2D frameTexture = AnimatedFont.SpriteFrames.GetFrameTexture(animLetter.Letter, frame);
                animLetter.Texture[frame] = frameTexture;
                
                if (AnimationStyles == AnimationStyles.InstantLoop)
                    animLetter.FrameSpeed = (float)AnimatedFont.SpriteFrames.GetAnimationSpeed(animLetter.Letter);

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
            if (animLetter.Texture != null)
            {
                if (animLetter.Texture[animLetter.FrameIndex] is AtlasTexture letterAtlas)
                {
                    //GD.Print($"drawing atlastexture letter {animLetter.Letter}");
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
            }
        }
    }
    
    public override void _PhysicsProcess(double delta)
    {
        
    }
}
