using Rubicon.API;
using Rubicon.Core.Rulesets;

// This is a template for a modchart in C#.
// This can also act as a Node! So yes, you will have access to such things like _Process(delta).
public partial class NewModChart : CsModChart
{
	// Triggers every measure.
	public override void MeasureHit(int measure)
	{
		
	}

	// Triggers every beat.
	public override void BeatHit(int beat)
	{
		
	}

	// Triggers every step.
	public override void StepHit(int step)
	{
		
	}
	
	// Triggers every time a note is hit.
	// public override void NoteHit(StringName barLineName, NoteResult result) { }
}
