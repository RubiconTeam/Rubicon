using Rubicon.Core.Chart;
using Rubicon.Core.Rulesets;

namespace Rubicon.API;

// This is a template for a custom note type in C#.
// Sorry if the amount of lines seem a little scary at first, there's a lot here I know!
// This can also act as a Node! So yes, you will have access to such things like _Ready() and _Process(delta)
public partial class NewNoteType : CsNoteType
{
    // This is what your note type will be named.
    [Export] public StringName TypeName = "";
    
    // This is for modifying the initial note data before the notes spawn.
    protected override void InitializeNote(NoteData[] notes, StringName noteType)
    {
        if (TypeName != noteType)
            return;
        
        // Add your code here!
    }

    // This is for customizing the note's graphic when a note spawns! (This note is a node.)
    protected override void SpawnNote(Note note, StringName noteType)
    {
        if (TypeName != noteType)
            return;
        
        // Add your code here!
    }

    // This is triggerred every time a note is hit/missed! Use this to modify "result".
    protected override void NoteHit(StringName barLineName, NoteResult result)
    {
        if (TypeName == result.Note.Type)
            return;
        
        // Add your code here!
    }
}