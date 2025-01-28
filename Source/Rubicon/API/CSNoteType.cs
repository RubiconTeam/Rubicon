using Rubicon.Core.Chart;
using Rubicon.Core.Rulesets;
using Rubicon.Game;

namespace Rubicon.API;

/// <summary>
/// A template for a note type in C#. Must be inherited.
/// </summary>
[GlobalClass] public abstract partial class CSNoteType : Node
{
    private bool _initialized = false;
    
    /// <summary>
    /// If it hasn't been initialized already, link itself to the play field.
    /// </summary>
    public override void _Ready()
    {
        base._Ready();

        if (_initialized)
            return;

        PlayField playField = RubiconGame.Instance.PlayField;
        playField.Factory.SpawnNote += SpawnNote;
        playField.InitializeNote += InitializeNote;
        playField.NoteHit += NoteHit;

        _initialized = true;
    }

    /// <summary>
    /// Used to set up note data initially for every note type.
    /// </summary>
    /// <param name="notes">An array of notes</param>
    /// <param name="noteType">The note type</param>
    protected abstract void InitializeNote(NoteData[] notes, StringName noteType);

    /// <summary>
    /// Triggers when the factory spawns a note of this type. Use this to set up your note.
    /// </summary>
    /// <param name="note">The note node graphic</param>
    /// <param name="noteType">The note's result</param>
    protected abstract void SpawnNote(Note note, StringName noteType);
    
    /// <summary>
    /// Triggers every time a note of this type is hit. Use this to modify the result, if need be.
    /// </summary>
    /// <param name="barLineName">The bar line's name</param>
    /// <param name="result">The result passed through</param>
    protected abstract void NoteHit(StringName barLineName, NoteResult result);
}