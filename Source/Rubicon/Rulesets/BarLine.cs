using Rubicon.Core;
using Rubicon.Core.Chart;

namespace Rubicon.Rulesets;

/// <summary>
/// A base bar line for Rubicon rulesets
/// </summary>
[GlobalClass] public abstract partial class BarLine : Control
{
	/// <summary>
	/// The individual chart for this bar line. Contains notes and scroll velocity changes.
	/// </summary>
	[Export] public IndividualChart Chart;
	
	/// <summary>
	/// Contains all the nodes used to manage notes.
	/// </summary>
	[Export] public NoteManager[] Managers;
	
	/// <summary>
	/// The PlayField this instance is associated with.
	/// </summary>
	[Export] public PlayField PlayField;

	/// <summary>
	/// The distance to offset notes by position-wise.
	/// </summary>
	[Export] public float DistanceOffset = 0;
	
	/// <summary>
	/// The index of the current scroll velocity.
	/// </summary>
	[Export] public int ScrollVelocityIndex = 0;
	
	/// <summary>
	/// A signal that is emitted every time a manager in this bar line hits a note. Can be a miss.
	/// </summary>
	[Signal] public delegate void NoteHitEventHandler(BarLine barLine, int lane, string direction, NoteInputElement inputElement);

	/// <summary>
	/// A signal that is emitted every time a manager in this bar line either presses or lets go of a bind.
	/// </summary>
	[Signal] public delegate void BindPressedEventHandler(BarLine barLine);
	
	public override void _Process(double delta)
	{
		base._Process(delta);
		
		// Handle SV changes
		if (Chart?.SvChanges == null)
			return;
		
		double time = Conductor.Time * 1000d;
		SvChange[] svChangeList = Chart.SvChanges;
		while (ScrollVelocityIndex + 1 < svChangeList.Length && svChangeList[ScrollVelocityIndex + 1].MsTime - time <= 0)
			ScrollVelocityIndex++;
		
		SvChange currentScrollVel = Chart.SvChanges[ScrollVelocityIndex];
		DistanceOffset = -(float)(currentScrollVel.Position + (time - currentScrollVel.MsTime) * currentScrollVel.Multiplier);
	}

	/// <summary>
	/// Triggers upon one of its note managers hitting a note in any way, even if it's a miss.
	/// </summary>
	/// <param name="lane">The lane index of the note manager</param>
	/// <param name="inputElement">The input element received</param>
	public abstract void OnNoteHit(int lane, NoteInputElement inputElement);

	public void SetAutoPlay(bool autoplay)
	{
		foreach (NoteManager noteManager in Managers)
			noteManager.Autoplay = autoplay;
	}

	public void SetScrollSpeed(float scrollSpeed)
	{
		foreach (NoteManager noteManager in Managers)
			noteManager.ScrollSpeed = scrollSpeed;
	}
}
