namespace Rubicon.Editor.Rulesets.Mania;

public partial class ManiaEditorSpace : EditorSpace
{
    protected override EditorHitObject CreateHitObject() => new ManiaEditorHitObject();
}