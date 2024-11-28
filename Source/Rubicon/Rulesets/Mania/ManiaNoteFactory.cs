using Rubicon.Core.Chart;

namespace Rubicon.Rulesets.Mania;

[GlobalClass] public partial class ManiaNoteFactory : NoteFactory
{
    [Export] public ManiaNoteSkin NoteSkin;

    protected override Note CreateNote() => new ManiaNote();

    protected override void SetupNote(Note note, StringName type)
    {
        if (note is not ManiaNote maniaNote)
            return;
        
        EmitSignal(NoteFactory.SignalName.SpawnNote, maniaNote, type);
        if (maniaNote.NoteSkin != null)
            return;
        
        maniaNote.ChangeNoteSkin(NoteSkin);
    }
}