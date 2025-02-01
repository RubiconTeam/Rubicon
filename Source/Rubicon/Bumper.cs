using Rubicon.Core;
using Rubicon.Core.Chart;

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
    /// How many times to bump each measure.
    /// </summary>
    [Export] public float BumpMeasure { get => _bumpMeasure; set => SetBumpMeasure(value); }
    
    /// <summary>
    /// Emits every bump.
    /// </summary>
    [Signal] public delegate void BumpedEventHandler();

    private BpmInfo _currentBpm;
    
    private int _bumpStep = 4;
    private int _stepOffset = 0;
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
        _bumpStep = (int)Math.Floor(_currentBpm.TimeSignatureNumerator * _currentBpm.TimeSignatureDenominator * _bumpMeasure);
    }
}