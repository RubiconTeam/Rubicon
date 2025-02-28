#if TOOLS
using Rubicon.Core.Rulesets.Mania;

namespace Rubicon.Editor.Rulesets.Mania;

public partial class ManiaEditorHitObject : EditorHitObject
{
    public ManiaNoteSkin NoteSkin;
    
    public override void UpdateGraphic()
    {
        if (Data == null)
        {
            GD.PrintErr($"[ManiaEditorHitObject] Note data for note {Name} is null");
            return;
        }
        
        if (NoteSkin == null)
        {
            GD.PrintErr($"[ManiaEditorHitObject] NoteSkin is null for note at time {Data.Time}");
            return;
        }

        SpriteFrames = NoteSkin.Notes;
        Play($"{NoteSkin.GetDirection(Data.Lane)}NoteNeutral");
    }
}
#endif