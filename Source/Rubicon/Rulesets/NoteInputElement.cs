using Rubicon.Core.Chart;
using Rubicon.Core.Data;

namespace Rubicon.Rulesets;

/// <summary>
/// An element for notes to be placed in a queue to be processed next frame. 
/// </summary>
[GlobalClass]
public partial class NoteInputElement : RefCounted
{
    /// <summary>
    /// The note data linked with this object.
    /// </summary>
    [Export] public NoteData Note;
    
    /// <summary>
    /// The index of the note hit.
    /// </summary>
    [Export] public int Index = 0;

    /// <summary>
    /// The distance from the note's actual hit time.
    /// </summary>
    [Export] public double Distance;

    /// <summary>
    /// Indicates whether the NoteManager was holding this note or not, if this note is a hold note.
    /// </summary>
    [Export] public bool Holding = false;

    /// <summary>
    /// The type of hit that was retrieved. Automatically set when setting Distance.
    /// </summary>
    [Export] public HitType Hit;

    /// <summary>
    /// Sets up <see cref="Hit"/> based on <see cref="Distance"/>.
    /// </summary>
    /// <returns>Itself, for chaining purposes</returns>
    public NoteInputElement AutoDetectHit()
    {
        double[] hitWindows = [ 
            ProjectSettings.GetSetting("rubicon/judgments/perfect_hit_window").AsDouble(),
            ProjectSettings.GetSetting("rubicon/judgments/great_hit_window").AsDouble(),
            ProjectSettings.GetSetting("rubicon/judgments/good_hit_window").AsDouble(),
            ProjectSettings.GetSetting("rubicon/judgments/okay_hit_window").AsDouble(),
            ProjectSettings.GetSetting("rubicon/judgments/bad_hit_window").AsDouble()
        ]; 
        int hit = hitWindows.Length;
        for (int i = 0; i < hitWindows.Length; i++)
        {
            if (Mathf.Abs(Distance) <= hitWindows[i])
            {
                hit = i;
                break;
            }
        }

        Hit = (HitType)hit;
        return this;
    }
}