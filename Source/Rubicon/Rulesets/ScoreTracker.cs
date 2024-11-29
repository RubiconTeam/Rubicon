using System.Collections.Generic;
using System.Linq;
using Rubicon.Core.Chart;
using Range = System.Range;

namespace Rubicon.Rulesets;

[GlobalClass] public partial class ScoreTracker : RefCounted
{
    [Export] public int Score = 0;

    [Export] public float Accuracy = 100f;

    [Export] public long PerfectHits = 0;
    
    [Export] public long GreatHits = 0;
    
    [Export] public long GoodHits = 0;

    [Export] public long OkayHits = 0;
    
    [Export] public long BadHits = 0;
    
    [Export] public long Misses = 0;

    [Export] public long Combo = 0;

    [Export] public long HighestCombo = 0;

    [Export] public long NoteCount = 0;

    [ExportGroup("References"), Export] public RubiChart Chart;

    private NoteData[] _playableNotes;
    
    public void Initialize(RubiChart chart, StringName target)
    {
        Chart = chart;
        
        IndividualChart firstChart = chart.Charts.FirstOrDefault(x => x.Name == target);
        if (firstChart == null)
            return;
        
        double startTime = 0;
        List<NoteData> notes = new List<NoteData>();
        List<TargetSwitch> switches = new List<TargetSwitch>(firstChart.Switches);
        switches.Insert(0, new TargetSwitch{ Time = 0.0, MsTime = 0.0, Name = target });
        for (int i = 0; i < switches.Count; i++)
        {
            IndividualChart curChart = chart.Charts.FirstOrDefault(x => x.Name == switches[i].Name);
            if (curChart == null)
                continue;
            
            if (i < switches.Count - 1)
            {
                notes.AddRange(GetNotesInRange(curChart.Notes, startTime, switches[i + 1].Time));
                startTime = switches[i + 1].Time;
                continue;
            }
            
            notes.AddRange(GetNotesInRange(curChart.Notes, startTime));
        }
        
        _playableNotes = notes.ToArray();
        NoteCount = _playableNotes.LongLength;
    }

    public NoteData[] GetPlayableNotes() => _playableNotes;

    public void MarkNoteAsMine(NoteData note)
    {
        if (!_playableNotes.Contains(note))
            return;

        note.ShouldMiss = true;
        NoteCount--;

        if (note.Length > 0)
            NoteCount--;
    }

    private NoteData[] GetNotesInRange(NoteData[] notes, double start)
    {
        return notes.Where(x => x.Time >= start).ToArray();
    }
    
    private NoteData[] GetNotesInRange(NoteData[] notes, double start, double end)
    {
        return notes.Where(x => x.Time >= start && x.Time <= end).ToArray();
    }
}