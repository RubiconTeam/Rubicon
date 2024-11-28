using System.Linq;
using Rubicon.Core;
using Rubicon.Core.Chart;
using Rubicon.Core.Data;
using Rubicon.Core.Meta;
using Rubicon.Core.UI;

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

    // lego was probably right in having a high score class, ill save this for him
    [Export] public uint Score = 0;

    [Export] public float Accuracy = 100f;

    [Export] public uint PerfectHits = 0;
    
    [Export] public uint GreatHits = 0;
    
    [Export] public uint GoodHits = 0;

    [Export] public uint OkayHits = 0;
    
    [Export] public uint BadHits = 0;
    
    [Export] public uint Misses = 0;

    [Export] public uint Combo = 0;

    [Export] public uint HighestCombo = 0;

    [Export] public uint NoteCount = 0;

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

    [Export] public RubiconEvent NoteHitEvent = new();
    
    [Signal] public delegate void BeforeNoteCreateEventHandler(NoteData note);
    
    /// <summary>
    /// A signal that is emitted upon failure.
    /// </summary>
    [Signal] public delegate void OnFailEventHandler();

    /// <summary>
    /// The Judgment instance for this play field.
    /// </summary>
    public IJudgment Judgment;

    /// <summary>
    /// The ComboDisplay instance for this play field.
    /// </summary>
    public IComboDisplay ComboDisplay;

    /// <summary>
    /// The HitDistance instance for this play field.
    /// </summary>
    public IHitDistance HitDistance;
    
    /// <summary>
    /// Readies the PlayField for gameplay!
    /// </summary>
    /// <param name="meta">The song meta</param>
    /// <param name="chart">The chart loaded</param>
    public virtual void Setup(SongMeta meta, RubiChart chart)
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

        if (UiStyle.HitDistance != null)
        {
            Node hitDistNode = UiStyle.HitDistance.Instantiate();
            if (hitDistNode is not IHitDistance hitDist)
            {
                GD.PrintErr($"UI Style's HitDistance does not inherit from IHitDistance! ({uiStylePath})");
            }
            else
            {
                HitDistance = hitDist;
                AddChild(hitDistNode);
                
                if (HitDistance is IJudgmentMaterial toApply)
                    toApply.ApplyUiStyle(UiStyle);
            }
        }

        if (UiStyle.Judgment != null)
        {
            Node judgmentNode = UiStyle.Judgment.Instantiate();
            if (judgmentNode is not IJudgment judgment)
            {
                GD.PrintErr($"UI Style's Judgment does not inherit from IJudgment! ({uiStylePath})");
            }
            else
            {
                Judgment = judgment;
                AddChild(judgmentNode);   
                
                if (Judgment is IJudgmentMaterial toApply)
                    toApply.ApplyUiStyle(UiStyle);
            }
        }

        if (UiStyle.Combo != null)
        {
            Node comboNode = UiStyle.Combo.Instantiate();
            if (comboNode is not IComboDisplay comboDisplay)
            {
                GD.PrintErr($"UI Style's ComboDisplay does not inherit from IComboDisplay! ({uiStylePath})");
            }
            else
            {
                ComboDisplay = comboDisplay;
                AddChild(comboNode);   
                
                if (ComboDisplay is IJudgmentMaterial toApply)
                    toApply.ApplyUiStyle(UiStyle);
            }
        }
        
        BarLines = new BarLine[chart.Charts.Length];
        TargetBarLine = meta.PlayableCharts[0]; // For now, allow this to be modified later
        for (int i = 0; i < chart.Charts.Length; i++)
        {
            IndividualChart indChart = chart.Charts[i];
            for (int j = 0; j < indChart.Notes.Length; j++)
                EmitSignalBeforeNoteCreate(indChart.Notes[j]);
            
            BarLine curBarLine = CreateBarLine(indChart, i);
            curBarLine.PlayField = this;
            if (indChart.Name == TargetBarLine)
            {
                TargetIndex = i;
                curBarLine.SetAutoPlay(false);
                NoteCount = (uint)(indChart.Notes.Count(x => !x.ShouldMiss) + indChart.Notes.Count(x => !x.ShouldMiss && x.Length > 0));
            }
            
            AddChild(curBarLine);
            BarLines[i] = curBarLine;
            curBarLine.NoteHit += OnNoteHit;
        }
        
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
        EmitSignalOnFail();
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
    protected virtual void OnNoteHit(BarLine barLine, int lane, string direction, NoteInputElement inputElement)
    {
        NoteResult result = new NoteResult(NoteResultFlags.None, inputElement.Hit);
        Variant[] results = NoteHitEvent.Invoke(barLine, lane, (int)inputElement.Hit, inputElement.Holding);
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
            Combo = hit != HitType.Miss ? Combo + 1 : 0;
            if (Combo > HighestCombo)
                HighestCombo = Combo;

            switch (hit)
            {
                case HitType.Perfect:
                    PerfectHits++;
                    break;
                case HitType.Great:
                    GreatHits++;
                    break;
                case HitType.Good:
                    GoodHits++;
                    break;
                case HitType.Okay:
                    OkayHits++;
                    break;
                case HitType.Bad:
                    BadHits++;
                    break;
                case HitType.Miss:
                    Misses++;
                    break;
            }
            
            UpdateStatistics();
            Judgment?.Play(hit, UiStyle.JudgmentOffset);   
            ComboDisplay?.Show(Combo, hit, UiStyle.ComboOffset);
            HitDistance?.Show(inputElement.Distance, hit, UiStyle.HitDistanceOffset);
        }
        
        result.Free();
        inputElement.Free();
    }
}