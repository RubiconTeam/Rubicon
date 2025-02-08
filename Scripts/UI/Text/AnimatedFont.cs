using Godot.Collections;

namespace Rubicon.Extras.UI;

#if TOOLS
[Tool]
#endif
[GlobalClass] public partial class AnimatedFont : Resource
{
    [Export] public SpriteFrames SpriteFrames;
    [ExportGroup("Letter Options"), Export] private Dictionary<string,string> _characterAliases = new();
    [Export] public Dictionary<string,Vector2> Offset = new();
    [Export] public Dictionary<string,Vector2> Advance = new();
}