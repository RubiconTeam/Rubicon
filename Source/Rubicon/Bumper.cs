using Rubicon.Core;
using Rubicon.Core.Chart;
using Rubicon.Data;

namespace Rubicon;

/// <summary>
/// A special <see cref="Node"/> that bumps to the beat.
/// </summary>
[GlobalClass] public partial class Bumper : Node
{
    /// <summary>
    /// Whether to allow bumps or not.
    /// </summary>
    [Export] public bool Enabled = true;

    /// <summary>
    /// The type of time value to have. Options <see cref="TimeValue.Beat"/> and <see cref="TimeValue.Step"/> are referenced in 4/4 time.
    /// </summary>
    [Export] public TimeValue Type = TimeValue.Measure;

    [Export]
    public float Value
    {
        get
        {
            switch (Type)
            {
                default:
                    return _bumpMeasure;
                case TimeValue.Beat:
                    return ConductorUtility.MeasureToBeats(_bumpMeasure);
                case TimeValue.Step:
                    return ConductorUtility.MeasureToSteps(_bumpMeasure);
            }
        }
        set
        {
            switch (Type)
            {
                default:
                    SetBumpMeasure(value);
                    break;
                case TimeValue.Beat:
                    SetBumpMeasure(ConductorUtility.BeatsToMeasures(value));
                    break;
                case TimeValue.Step:
                    SetBumpMeasure(ConductorUtility.StepsToMeasures(value));
                    break;
            }
        }
    }
    
    /// <summary>
    /// Emits every bump.
    /// </summary>
    [Signal] public delegate void BumpedEventHandler();

    private BpmInfo _currentBpm;
    
    private int _bumpStep = 4; // This is DIFFERENT from TimeValue.Step!!!
    private int _stepOffset = 0;

    private float _cachedStep = 0;
    private float _cachedBeat = 0;
    private float _bumpMeasure = 1f / 2f;

    private bool _initialized = false;

    public override void _Ready()
    {
        _initialized = true;
        
        base._Ready();

        Conductor.StepHit += StepHit;
        Conductor.BpmChanged += BpmChanged;
        
        BpmChanged(Conductor.BpmList[Conductor.BpmIndex]);
    }

    private void StepHit(int step)
    {
        if (!Enabled)
            return;
        
        if ((step - _stepOffset) % _bumpStep == 0)
            EmitSignalBumped();
    }
    
    private void BpmChanged(BpmInfo currentBpm)
    {
        _stepOffset += _currentBpm == null ? 0 : (int)Math.Floor((currentBpm.Time - _currentBpm.Time) * _currentBpm.TimeSignatureNumerator * _currentBpm.TimeSignatureDenominator);
        
        _currentBpm = currentBpm;
        SetBumpMeasure(_bumpMeasure);
    }

    private void SetBumpMeasure(float value)
    {
        if (!_initialized)
            _Ready();
        
        _bumpMeasure = value;
        _cachedBeat = ConductorUtility.MeasureToBeats(_bumpMeasure);
        _cachedStep = ConductorUtility.MeasureToSteps(_bumpMeasure);
        
        _bumpStep = (int)Math.Floor(_currentBpm.TimeSignatureNumerator * _currentBpm.TimeSignatureDenominator * _bumpMeasure);
    }
}