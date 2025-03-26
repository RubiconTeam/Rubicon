using Rubicon.Core;
using Rubicon.Core.Rulesets;
using Rubicon.Game;

namespace Rubicon.API;

[GlobalClass] public partial class CsSongScript : Node
{
    public override void _Ready()
    {
        base._Ready();

        Conductor.MeasureHit += MeasureHit;
        Conductor.BeatHit += BeatHit;
        Conductor.StepHit += StepHit;
        RubiconGame.PlayField.NoteHit += NoteHit;
    }
    
    public virtual void MeasureHit(int measure) { }

    public virtual void BeatHit(int beat) { }
    
    public virtual void StepHit(int step) { }

    public virtual void NoteHit(StringName barLineName, NoteResult result) { }
}