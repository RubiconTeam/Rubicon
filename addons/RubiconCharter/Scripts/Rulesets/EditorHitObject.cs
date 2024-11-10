#if TOOLS
using Rubicon.Core.Chart;

namespace Rubicon.Editor.Rulesets;

[Tool]
public abstract partial class EditorHitObject : AnimatedSprite2D
{
    public NoteData Data;

    public abstract void UpdateGraphic();
}
#endif