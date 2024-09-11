using System.Linq;
using Godot.Collections;
using Rubicon.Core;
using Rubicon.Core.Chart;
using Rubicon.Core.Data;

namespace Rubicon.Rulesets;

public partial class NoteManager : Control
{
    /// <summary>
    /// Contains the individual notes for this manager.
    /// </summary>
    [Export] public NoteData[] Notes;

    /// <summary>
    /// Contains the visual hit objects for this manager. Notes are recycled.
    /// </summary>
    [Export] public Array<Note> HitObjects;

    /// <summary>
    /// If true, the computer will hit the notes that come by.
    /// </summary>
    [Export] public bool Autoplay = true;

    /// <summary>
    /// If false, nothing can be input through this note manager. Not even the computer.
    /// </summary>
    [Export] public bool InputsEnabled = true;

    /// <summary>
    /// The scroll speed for this note manager.
    /// </summary>
    [Export] public float ScrollSpeed = 1f;

    /// <summary>
    /// This note manager's parent bar line.
    /// </summary>
    [Export] public BarLine ParentBarLine;
    
    /// <summary>
    /// Is true when the manager has gone through all notes present in <see cref="Chart">Chart</see>.
    /// </summary>
    public bool IsComplete => NoteHitIndex >= Notes.Length;

    /// <summary>
    /// Is true when the manager has no notes to hit for at least a measure.
    /// </summary>
    public bool OnBreak => !IsComplete && Notes[NoteHitIndex].MsTime - Conductor.Time * 1000d >
        ConductorUtility.MeasureToMs(Conductor.CurrentMeasure, Conductor.Bpm, Conductor.TimeSigNumerator);
    
    /// <summary>
    /// The next note's index to be hit.
    /// </summary>
    [ExportGroup("Advanced"), Export] public int NoteHitIndex = 0; 
    
    /// <summary>
    /// The next hit object's index to be spawned in.
    /// </summary>
    [Export] public int NoteSpawnIndex = 0;

    public override void _Process(double delta)
    {
        base._Process(delta);
        
        // Handle note spawning
        double time = Conductor.Time * 1000d;
        SvChange currentScrollVel = ParentBarLine.Chart.SvChanges[ParentBarLine.ScrollVelocityIndex];
        if (NoteSpawnIndex < Notes.Length && Visible)
        {
            while (NoteSpawnIndex < Notes.Length && Notes[NoteSpawnIndex].MsTime - time <= 2000)
            {
                if (Notes[NoteSpawnIndex].MsTime - time < 0 || Notes[NoteSpawnIndex].WasSpawned)
                {
                    NoteSpawnIndex++;
                    continue;
                }

                Note note = HitObjects.FirstOrDefault(x => x.Active);
                if (note == null)
                {
                    note = CreateNote();
                    AddChild(note);
                }
                else
                {
                    note.MoveToFront();
                }
                
                SetupNote(note, Notes[NoteSpawnIndex], currentScrollVel);
                Notes[NoteSpawnIndex].WasSpawned = true;
                NoteSpawnIndex++;
            }
        }
        
        // If note hitting is done, stop here
        if (IsComplete)
            return;
        
        NoteData curNoteData = Notes[NoteHitIndex];
        if (Autoplay && InputsEnabled)
        {
            while (curNoteData.MsTime - time <= 0)
            {
                if (!Notes[NoteHitIndex].ShouldMiss)
                    OnNoteHit(curNoteData, 0, curNoteData.MsLength > 0);
                
                NoteHitIndex++;
                curNoteData = Notes[NoteHitIndex];
            }
        }

        if (curNoteData.MsTime - time <= -(float)ProjectSettings.GetSetting("rubicon/judgments/bad_hit_window"))
        {
            OnNoteMiss(curNoteData, -(float)ProjectSettings.GetSetting("rubicon/judgments/bad_hit_window") - 1, false);
            NoteHitIndex++;
        }
    }

    #region Virtual (Overridable) Methods

    /// <summary>
    /// Is called when creating a new note. Override to replace with a type that inherits from <see cref="Note"/>.
    /// </summary>
    /// <returns>A new note.</returns>
    protected virtual Note CreateNote() => new Note();

    /// <summary>
    /// Called when setting up a note. Notes will be recycled.
    /// </summary>
    /// <param name="note">The note passed in</param>
    /// <param name="data">The note data</param>
    /// <param name="svChange">The SV change associated</param>
    protected virtual void SetupNote(Note note, NoteData data, SvChange svChange)
    {
        
    }

    protected virtual void OnNoteHit(NoteData note, double distance, bool holding)
    {
        note.WasHit = true;
    }
    
    protected virtual void OnNoteMiss(NoteData note, double distance, bool holding)
    {
        note.WasHit = true;
    }
    #endregion
}