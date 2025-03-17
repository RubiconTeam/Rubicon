namespace Rubicon.Editor.Rulesets.Mania;

#if TOOLS
public partial class ManiaEditorSpace : EditorSpace
{
    protected override EditorHitObject CreateHitObject() => new ManiaEditorHitObject();
}
#endif