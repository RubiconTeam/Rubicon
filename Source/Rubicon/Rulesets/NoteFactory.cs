using System.Linq;
using Godot.Collections;
using Rubicon.Core.Chart;

namespace Rubicon.Rulesets;

[GlobalClass] public abstract partial class NoteFactory : Node
{
    /// <summary>
    /// All the visual hit objects that were spawned.
    /// </summary>
    [Export] public Dictionary<StringName, Array<Note>> HitObjects = new();
    
    /// <summary>
    /// Triggers when a note is spawned.
    /// </summary>
    [Signal] public delegate void SpawnNoteEventHandler(Note note, StringName noteType);

    /// <summary>
    /// Obtains a note for the type provided. If none are present, a new one is created.
    /// </summary>
    /// <param name="type">A note type</param>
    /// <returns><see cref="Note"/></returns>
    public Note GetNote(StringName type)
    {
        if (!HitObjects.ContainsKey(type))
            HitObjects.Add(type, new Array<Note>());

        Note result = HitObjects[type].FirstOrDefault(x => !x.Active);
        if (result != null)
        {
            result.Reset();
            return result;   
        }

        result = CreateNote();
        result.Initialize();
        SetupNote(result, type);
        HitObjects[type].Add(result);
        
        return result;
    }
    
    /// <summary>
    /// Is called when creating a new note. Override to replace with a type that inherits from <see cref="Note"/>.
    /// </summary>
    /// <returns>A new note.</returns>
    protected abstract Note CreateNote();

    /// <summary>
    /// Called when setting up a note for the first time.
    /// </summary>
    /// <param name="note">The note passed in</param>
    /// <param name="type">The note type</param>
    protected abstract void SetupNote(Note note, StringName type);
}