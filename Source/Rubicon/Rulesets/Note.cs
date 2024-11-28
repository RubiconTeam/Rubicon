using Rubicon.Core.Chart;

namespace Rubicon.Rulesets;

/// <summary>
/// A control node used to visualize a note meant to be hit on-screen.
/// </summary>
[GlobalClass] public abstract partial class Note : Control
{
    /// <summary>
    /// Contains info about this note.
    /// </summary>
    [Export] public NoteData Info;
    
    /// <summary>
    /// The parent <see cref="NoteManager"/>.
    /// </summary>
    [Export] public NoteManager ParentManager;

    /// <summary>
    /// If false, this note is ready to be recycled.
    /// </summary>
    [Export] public bool Active = true;

    /// <summary>
    /// Whether this note has missed.
    /// </summary>
    [Export] public bool Missed = false;

    /// <summary>
    /// Run when a note is first created.
    /// </summary>
    public abstract void Initialize();
    
    public override void _Process(double delta)
    {
        if (!Active)
            return;
        
        UpdatePosition();
    }

    /// <summary>
    /// Triggers when the note needs to update its position.
    /// </summary>
    public abstract void UpdatePosition();

    /// <summary>
    /// Triggers upon this note being recycled.
    /// </summary>
    public virtual void Reset()
    {
        Info = null;
        Active = true;
        Visible = true;
        Missed = false;
    }

    /// <summary>
    /// Triggers when the note needs to be prepared for recycling. (Ex: hitting a note)
    /// </summary>
    public virtual void PrepareRecycle()
    {
        Active = Visible = false;
    }

    /// <summary>
    /// Gets the starting position of the note, with all the scroll velocities considered.
    /// </summary>
    /// <returns>The starting position of the note</returns>
    protected float GetStartingPoint()
    {
        SvChange[] svChangeList = ParentManager.ParentBarLine.Chart.SvChanges;
        return (float)(svChangeList[Info.StartingScrollVelocity].Position + ((Info.MsTime - svChangeList[Info.StartingScrollVelocity].MsTime) * svChangeList[Info.StartingScrollVelocity].Multiplier));
    }

    /// <summary>
    /// Gets the ending position of the note, with all the scroll velocities considered.
    /// </summary>
    /// <returns>The ending position of the note</returns>
    protected float GetEndingPoint()
    {
        SvChange[] svChangeList = ParentManager.ParentBarLine.Chart.SvChanges;
        return (float)(svChangeList[Info.EndingScrollVelocity].Position +
            ((Info.MsTime + Info.MsLength - svChangeList[Info.EndingScrollVelocity].MsTime) * svChangeList[Info.EndingScrollVelocity].Multiplier));
    }
}