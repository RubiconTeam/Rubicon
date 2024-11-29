using System.Linq;
using Rubicon.Core;
using Rubicon.Core.Chart;
using Rubicon.Core.Data;
using Rubicon.Core.Meta;

namespace Rubicon.Rulesets;

/// <summary>
/// A control node with all general ruleset gameplay-related functions.
/// </summary>
[GlobalClass] public abstract partial class PlayField : Control
{
    /// <summary>
    /// The current health the player has.
    /// </summary>
    [Export] public uint Health = 0;

    /// <summary>
    /// The max health the player can have.
    /// </summary>
    [Export] public uint MaxHealth = 1000;

    /// <summary>
    /// Keeps track of the player's combos and score.
    /// </summary>
    [Export] public ScoreTracker ScoreTracker = new();

    /// <summary>
    /// The Chart for this PlayField.
    /// </summary>
    [Export] public RubiChart Chart;

    /// <summary>
    /// The Song meta for this PlayField
    /// </summary>
    [Export] public SongMeta Metadata;

    /// <summary>
    /// The UiStyle currently being used
    /// </summary>
    [Export] public UiStyle UiStyle;
    
    /// <summary>
    /// The bar lines associated with this play field.
    /// </summary>
    [Export] public BarLine[] BarLines;
    
    /// <summary>
    /// The Target Bar Line's name for the player to control
    /// </summary>
    [Export] public StringName TargetBarLine = "Player";
    
    /// <summary>
    /// The Target Bar Line's index for the player to control
    /// </summary>
    [Export] public int TargetIndex = 0;

    /// <summary>
    /// Creates notes for bar lines to use.
    /// </summary>
    [Export] public NoteFactory Factory;

    /// <summary>
    /// An event that is invoked when a note is hit.
    /// </summary>
    [Export] public RubiconEvent GetNoteResults = new();
    
    /// <summary>
    /// Triggers upon the statistics updating.
    /// </summary>
    [Signal] public delegate void StatisticsUpdatedEventHandler(long combo, HitType hit, double distance);
    
    /// <summary>
    /// A signal that is emitted upon failure.
    /// </summary>
    [Signal] public delegate void FailedEventHandler();

    /// <summary>
    /// Readies the PlayField for gameplay!
    /// </summary>
    /// <param name="meta">The song meta</param>
    /// <param name="chart">The chart loaded</param>
    /// <param name="targetIndex">The index to play in <see cref="SongMeta.PlayableCharts"/>.</param>
    public virtual void Setup(SongMeta meta, RubiChart chart, int targetIndex)
    {
        Name = "Base PlayField";
        Metadata = meta;
        Chart = chart;
        SetAnchorsPreset(LayoutPreset.FullRect);
        Input.UseAccumulatedInput = false;
        
        // Handle UI Style
        string uiStylePath = $"res://Resources/UI/{Metadata.UiStyle}/Style.tres";
        if (!ResourceLoader.Exists(uiStylePath))
        {
            string defaultUiPath = $"res://Resources/UI/{ProjectSettings.GetSetting("rubicon/general/default_ui_style")}/style.tres";
            GD.PrintErr($"UI Style Path: {uiStylePath} does not exist. Defaulting to {defaultUiPath}");
            uiStylePath = defaultUiPath;
        }
        UiStyle = GD.Load<UiStyle>(uiStylePath);
        if (UiStyle.HitDistance != null && UiStyle.HitDistance.CanInstantiate())
            AddChild(UiStyle.HitDistance.Instantiate());
        if (UiStyle.Judgment != null && UiStyle.Judgment.CanInstantiate())
            AddChild(UiStyle.Judgment.Instantiate());
        if (UiStyle.Combo != null && UiStyle.Combo.CanInstantiate())
            AddChild(UiStyle.Combo.Instantiate());
        
        BarLines = new BarLine[chart.Charts.Length];
        TargetBarLine = meta.PlayableCharts[targetIndex];
        for (int i = 0; i < chart.Charts.Length; i++)
        {
            IndividualChart indChart = chart.Charts[i];
            BarLine curBarLine = CreateBarLine(indChart, i);
            curBarLine.PlayField = this;
            if (indChart.Name == TargetBarLine)
            {
                TargetIndex = i;
                curBarLine.SetAutoPlay(false);   
            }
            
            AddChild(curBarLine);
            BarLines[i] = curBarLine;
            curBarLine.NoteHit += BarLineHit;
        }
        
        ScoreTracker.Initialize(chart, TargetBarLine);
        UpdateOptions();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        
        if (GetFailCondition())
            Fail();
    }

    /// <summary>
    /// Instantly kills the player and emits the signal.
    /// </summary>
    public void Fail()
    {
        EmitSignalFailed();
    }

    /// <summary>
    /// This function is triggered upon an update to the settings.
    /// </summary>
    public abstract void UpdateOptions();

    /// <summary>
    /// Triggers every time the player hits a note to update the in-game statistics
    /// </summary>
    public abstract void UpdateStatistics();
    
    /// <summary>
    /// The fail condition for this play field.
    /// </summary>
    /// <returns>Whether the player has failed</returns>
    public virtual bool GetFailCondition() => false;

    /// <summary>
    /// Creates a new bar line and sets it up along with it.
    /// </summary>
    /// <param name="chart">The chart to assign</param>
    /// <param name="index">The assigned index of the bar line</param>
    /// <returns>A new <see cref="BarLine"/></returns>
    public abstract BarLine CreateBarLine(IndividualChart chart, int index);

    /// <summary>
    /// The function that is connected to the bar lines when a note is hit. Can be overriden if needed for a specific ruleset.
    /// </summary>
    /// <param name="barLine">The bar line</param>
    /// <param name="lane">The lane</param>
    /// <param name="direction">The sing direction</param>
    /// <param name="inputElement">Info about the input recieved</param>
    protected virtual void BarLineHit(BarLine barLine, int lane, string direction, NoteInputElement inputElement)
    {
        NoteResult result = new NoteResult(NoteResultFlags.None, inputElement.Hit);
        Variant[] results = GetNoteResults.Invoke(barLine, lane, (int)inputElement.Hit, inputElement.Holding);
        for (int i = 0; i < results.Length; i++)
        {
            if (results[i].VariantType != Variant.Type.Object || results[i].AsGodotObject() is not NoteResult newResult)
                continue;

            if (result.Equals(newResult))
                continue;
            
            result = newResult;
            break;
        }
        
        if (BarLines[TargetIndex] == barLine && !result.HasFlag(NoteResultFlags.Score))
        {
            HitType hit = result.Hit;
            ScoreTracker.Combo = hit != HitType.Miss ? ScoreTracker.Combo + 1 : 0;
            if (ScoreTracker.Combo > ScoreTracker.HighestCombo)
                ScoreTracker.HighestCombo = ScoreTracker.Combo;

            switch (hit)
            {
                case HitType.Perfect:
                    ScoreTracker.PerfectHits++;
                    break;
                case HitType.Great:
                    ScoreTracker.GreatHits++;
                    break;
                case HitType.Good:
                    ScoreTracker.GoodHits++;
                    break;
                case HitType.Okay:
                    ScoreTracker.OkayHits++;
                    break;
                case HitType.Bad:
                    ScoreTracker.BadHits++;
                    break;
                case HitType.Miss:
                    ScoreTracker.Misses++;
                    break;
            }
            
            UpdateStatistics();
            EmitSignalStatisticsUpdated(ScoreTracker.Combo, result.Hit, inputElement.Distance);
        }
        
        result.Free();
        inputElement.Free();
    }
}