using System.Linq;
using Rubicon.Core.Chart;
using Rubicon.Core.Meta;
using Rubicon.Data.Settings;

namespace Rubicon.Rulesets.Mania;

/// <summary>
/// A <see cref="PlayField"/> class with Mania-related gameplay incorporated. Also the main mode for Rubicon Engine.
/// </summary>
public partial class ManiaPlayField : PlayField
{
    /// <summary>
    /// The max score this instance can get.
    /// </summary>
    [Export] public uint MaxScore = 1000000;

    private uint _noteCount;
    
    /// <summary>
    /// Readies this PlayField for Mania gameplay!
    /// </summary>
    /// <param name="meta">The song meta</param>
    /// <param name="chart">The chart loaded</param>
    public override void Setup(SongMeta meta, RubiChart chart)
    {
        BarLines = new BarLine[chart.Charts.Length];
        TargetBarLine = meta.PlayableCharts[0]; // For now, allow this to be modified later
        
        // REALLY SHITTY, REPLACE BELOW LATER !!!
        string noteSkinName = meta.NoteSkin;
        if (!ResourceLoader.Exists($"res://Resources/UI/{noteSkinName}/Mania.tres"))
        {
            string defaultPath = ProjectSettings.GetSetting("rubicon/rulesets/mania/default_note_skin").AsString();
            GD.PrintErr($"Mania Note Skin Path: {noteSkinName} does not exist. Defaulting to {defaultPath}");
            noteSkinName = defaultPath;
        }
        
        ManiaNoteSkin noteSkin = GD.Load<ManiaNoteSkin>($"res://Resources/UI/{noteSkinName}/Mania.tres");
        for (int i = 0; i < chart.Charts.Length; i++)
        {
            IndividualChart indChart = chart.Charts[i];
            ManiaBarLine curBarLine = new ManiaBarLine();
            curBarLine.Setup(indChart, noteSkin, chart.ScrollSpeed);
            curBarLine.Name = "Mania Bar Line " + i;
            
            // Using Council positioning for now, sorry :/
            //curBarLine.Position = new Vector2(i * 720f - (chart.Charts.Length - 1) * 720f / 2f, 0f);
            curBarLine.AnchorLeft = curBarLine.AnchorRight = ((i * 0.5f) - (chart.Charts.Length - 1) * 0.5f / 2f) + 0.5f;

            if (indChart.Name == TargetBarLine)
            {
                TargetBarLineIndex = i;
                curBarLine.SetAutoPlay(false);
                _noteCount = (uint)(indChart.Notes.Count(x => !x.ShouldMiss) + indChart.Notes.Count(x => !x.ShouldMiss && x.Length > 0));
            }
            
            AddChild(curBarLine);
            BarLines[i] = curBarLine;
        }
        
        base.Setup(meta, chart);
        Name = "Mania PlayField";
        for (int i = 0; i < BarLines.Length; i++)
            BarLines[i].MoveToFront();;
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
        if (PerfectHits == _noteCount) Score = MaxScore;
        else
        {
            float baseNoteValue = ((float)MaxScore / _noteCount) / 2f;
            float baseScore = (float)((baseNoteValue * PerfectHits) + (baseNoteValue * (GreatHits * 0.9375)) + (baseNoteValue * (GoodHits * 0.625)) + (baseNoteValue * (OkayHits * 0.3125)) + (baseNoteValue * (BadHits * 0.15625)));
            float bonusScore = Mathf.Sqrt(((float)HighestCombo / _noteCount) * 100f) * MaxScore * 0.05f; 
            Score = (uint)(baseScore + bonusScore);
        }
        
        // Accuracy
        uint hitNotes = PerfectHits + GreatHits + GoodHits + OkayHits + BadHits + Misses;
        Accuracy = PerfectHits == _noteCount
            ? 100f
            : ((PerfectHits + (GreatHits * 0.95f) + (GoodHits * 0.65f) + (OkayHits * 0.3f) + (BadHits + 0.15f)) /
               hitNotes) * 100f;
    }

    /// <inheritdoc />
    public override bool GetFailCondition() => Health <= 0;
}