using Rubicon.Core;
using Rubicon.Core.Chart;

namespace Rubicon.Shared;

/// <summary>
/// A special <see cref="Node2D"/> that bumps to the beat.
/// </summary>
[GlobalClass] public abstract partial class Bumper2D : Node2D
{
    /// <summary>
    /// How many times to bump each measure.
    /// </summary>
    [Export] public double BumpMeasure { get => _bumpMeasure; set => SetBumpMeasure(value); }

    private BpmInfo _currentBpm;
    
    private int _bumpStep = 4;
    private int _stepOffset = 0;
    private double _bumpMeasure = 1d / 2d;

    public override void _Ready()
    {
        base._Ready();

        Conductor.StepHit += StepHit;
        Conductor.BpmChanged += BpmChanged;
        
        BpmChanged(Conductor.BpmList[Conductor.BpmIndex]);
    }
    
    /// <summary>
    /// Triggers every bump.
    /// </summary>
    public abstract void Bump();

    private void StepHit(int step)
    {
        if ((step - _stepOffset) % _bumpStep == 0)
            Bump();
    }
    
    private void BpmChanged(BpmInfo currentBpm)
    {
        _stepOffset += _currentBpm == null ? 0 : (int)Math.Floor((currentBpm.Time - _currentBpm.Time) * _currentBpm.TimeSignatureNumerator * _currentBpm.TimeSignatureDenominator);
        
        _currentBpm = currentBpm;
        SetBumpMeasure(_bumpMeasure);
    }

    private void SetBumpMeasure(double value)
    {
        _bumpMeasure = value;
        _bumpStep = (int)Math.Floor(_currentBpm.TimeSignatureNumerator * _currentBpm.TimeSignatureDenominator * _bumpMeasure);
    }
}