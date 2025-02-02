
namespace Rubicon.Extras.UI;
#if TOOLS
[Tool]
#endif
[GlobalClass] public partial class AnimatedFont : ReferenceRect
{
    [ExportToolButton("Update Text")] private Callable _update = Callable.From(UpdateText);

    [Export(PropertyHint.MultilineText)] public string Text = "Text Here";
    private string[] _letterArray;

    [Export(PropertyHint.File)] public SpriteFrames SpriteFrames;
    private int _frameIndex = 0;

    public override void _Ready()
    {
        _letterArray = GetLetterArray();
    }

    private string[] GetLetterArray()
    {
        return Text.Split("\n");
    }
    
    private static void UpdateText()
    {
        GD.Print("wow you updated the text and it did nothing");
    }

    public override void _Draw()
    {
        foreach (var letter in _letterArray)
        {
            if (SpriteFrames != null && SpriteFrames.HasAnimation(letter))
            {
                if (SpriteFrames.GetFrameTexture(letter, _frameIndex) is AtlasTexture spriteAtlas)
                {
                    //atlas texture handling
                    return;
                }
                //texture2d handling
            }
        }
    }
}
