using System.Linq;
using Rubicon.Core.Chart;
using Rubicon.Core.Meta;
using Rubicon.Data.Settings;

namespace Rubicon.Rulesets.Mania;

/// <summary>
/// A <see cref="PlayField"/> class with Mania-related gameplay incorporated. Also the main mode for Rubicon Engine.
/// </summary>
[GlobalClass] public partial class ManiaPlayField : PlayField
{
    /// <summary>
    /// The max score this instance can get.
    /// </summary>
    [Export] public int MaxScore = 1000000;

    [Export] public ManiaNoteSkin NoteSkin;

    /// <summary>
    /// Readies this PlayField for Mania gameplay!
    /// </summary>
    /// <param name="meta">The song meta</param>
    /// <param name="chart">The chart loaded</param>
    /// <param name="targetIndex">The index to play in <see cref="SongMeta.PlayableCharts"/>.</param>
    public override void Setup(SongMeta meta, RubiChart chart, int targetIndex)
    {
        // REALLY SHITTY, REPLACE BELOW LATER !!!
        string noteSkinName = meta.NoteSkin;
        if (!ResourceLoader.Exists($"res://Resources/UI/{noteSkinName}/Mania.tres"))
        {
            string defaultPath = ProjectSettings.GetSetting("rubicon/rulesets/mania/default_note_skin").AsString();
            GD.PrintErr($"Mania Note Skin Path: {noteSkinName} does not exist. Defaulting to {defaultPath}");
            noteSkinName = defaultPath;
        }
        
        NoteSkin = GD.Load<ManiaNoteSkin>($"res://Resources/UI/{noteSkinName}/Mania.tres");
        ManiaNoteFactory maniaFactory = new ManiaNoteFactory();
        maniaFactory.NoteSkin = NoteSkin;
        Factory = maniaFactory;
        
        base.Setup(meta, chart, targetIndex);
        
        Name = "Mania PlayField";
        for (int i = 0; i < BarLines.Length; i++)
            BarLines[i].MoveToFront();
    }
    
    /// <inheritdoc/>
    public override void UpdateOptions()
    {
        //BarLineContainer.
        //BarLineContainer.Position = new Vector2(0f, UserSettings.DownScroll ? -120f : 120f);

        for (int i = 0; i < BarLines.Length; i++)
        {
            if (BarLines[i] is ManiaBarLine maniaBarLine)
                maniaBarLine.SetDirectionAngle(!UserSettings.Gameplay.DownScroll ? Mathf.Pi / 2f : -Mathf.Pi / 2f);

            BarLines[i].AnchorTop = BarLines[i].AnchorBottom = UserSettings.Gameplay.DownScroll ? 1f : 0f;
            BarLines[i].OffsetTop = BarLines[i].OffsetBottom = UserSettings.Gameplay.DownScroll ? -140f : 140f;
            //BarLines[i].SetAnchorsPreset(barLinePreset, true);
        }
    }

    /// <inheritdoc />
    public override void UpdateStatistics()
    {
        // Score
        if (ScoreTracker.PerfectHits == ScoreTracker.NoteCount) ScoreTracker.Score = MaxScore;
        else
        {
            float baseNoteValue = ((float)MaxScore / ScoreTracker.NoteCount) / 2f;
            float baseScore = (float)((baseNoteValue * ScoreTracker.PerfectHits) + (baseNoteValue * (ScoreTracker.GreatHits * 0.9375)) + (baseNoteValue * (ScoreTracker.GoodHits * 0.625)) + (baseNoteValue * (ScoreTracker.OkayHits * 0.3125)) + (baseNoteValue * (ScoreTracker.BadHits * 0.15625)));
            float bonusScore = Mathf.Sqrt(((float)ScoreTracker.HighestCombo / ScoreTracker.NoteCount) * 100f) * MaxScore * 0.05f; 
            ScoreTracker.Score = (int)Math.Floor(baseScore + bonusScore);
        }
        
        // Accuracy
        long hitNotes = ScoreTracker.PerfectHits + ScoreTracker.GreatHits + ScoreTracker.GoodHits + ScoreTracker.OkayHits + ScoreTracker.BadHits + ScoreTracker.Misses;
        ScoreTracker.Accuracy = ScoreTracker.PerfectHits == ScoreTracker.NoteCount
            ? 100f
            : ((ScoreTracker.PerfectHits + (ScoreTracker.GreatHits * 0.95f) + (ScoreTracker.GoodHits * 0.65f) + (ScoreTracker.OkayHits * 0.3f) + (ScoreTracker.BadHits + 0.15f)) /
               hitNotes) * 100f;
    }

    /// <inheritdoc />
    public override bool GetFailCondition() => Health <= 0;

    public override BarLine CreateBarLine(IndividualChart chart, int index)
    {
        ManiaBarLine barLine = new ManiaBarLine();
        barLine.Setup(chart, NoteSkin, Chart.ScrollSpeed);
        barLine.Name = "Mania Bar Line " + index;
            
        // Using Council positioning for now, sorry :/
        //curBarLine.Position = new Vector2(i * 720f - (chart.Charts.Length - 1) * 720f / 2f, 0f);
        barLine.AnchorLeft = barLine.AnchorRight = ((index * 0.5f) - (Chart.Charts.Length - 1) * 0.5f / 2f) + 0.5f;
        return barLine;
    }
}