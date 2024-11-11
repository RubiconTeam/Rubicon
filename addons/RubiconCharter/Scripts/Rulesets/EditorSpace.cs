#if TOOLS
using System.Collections.Generic;
using System.Linq;
using Rubicon.Core.Chart;
using Godot.Collections;

namespace Rubicon.Editor.Rulesets;

/// <summary>
/// A class representing an individual chart's space to edit in.
/// </summary>
[Tool] public abstract partial class EditorSpace : Control
{
    public IndividualChart Chart;
    
    /// <summary>
    /// The spacing for every note, in pixels. Typically the note's height.
    /// </summary>
    public float Spacing = 160f;
    
    /// <summary>
    /// Stretches or squishes the notes together depending on how high or low the value is.
    /// </summary>
    public float Zoom = 1f;
    
    /// <summary>
    /// The active hit objects on screen.
    /// </summary>
    protected Array<EditorHitObject> ActiveHitObjects = new();
    
    /// <summary>
    /// A bin for every hit object that isn't in use, for recycling purposes.
    /// </summary>
    protected Array<EditorHitObject> HitObjectBin = new();
    
    /// <summary>
    /// Loads an <see cref="IndividualChart"/> instance into this space, usually found in <see cref="RubiChart"/>.<see cref="RubiChart.Charts"/>.
    /// </summary>
    /// <param name="chart">The individual chart</param>
    public void LoadChart(IndividualChart chart)
    {
        Chart = chart;
        UpdateHitObjects();
    }

    /// <summary>
    /// Updates all the hit objects on screen to match the chart.
    /// </summary>
    public virtual void UpdateHitObjects()
    {
        EditorHitObject[] deletedObjects = ActiveHitObjects.Where(x => !Chart.Notes.Contains(x.Data)).ToArray();
        for (int i = 0; i < deletedObjects.Length; i++)
        {
            EditorHitObject current = deletedObjects[i];
            
            RemoveChild(current);
            ActiveHitObjects.Remove(current);
            HitObjectBin.Add(current);
        }

        for (int i = 0; i < Chart.Notes.Length; i++)
        {
            bool hasObjects = ActiveHitObjects.Count > 0;
            if (hasObjects && ActiveHitObjects[i].Data == Chart.Notes[i])
            {
                Vector2 pos = ActiveHitObjects[i].Position;
                pos.Y = Spacing * Zoom * (float)Chart.Notes[i].Time;
                ActiveHitObjects[i].Position = pos;

                continue;
            }

            EditorHitObject current = null;
            if (ActiveHitObjects.Count > i + 1)
            {
                current = HitObjectBin.Count != 0 ? HitObjectBin[0] : CreateHitObject();
                AddChild(current);
            }
            else
            {
                current = ActiveHitObjects[i];
            }

            current.Data = Chart.Notes[i];
            current.Position = new Vector2(0f, Spacing * Zoom * (float)Chart.Notes[i].Time);
            current.UpdateGraphic();
        }
    }

    /// <summary>
    /// Creates a new hit object. Should be overridden to match the ruleset's version.
    /// </summary>
    /// <returns>A new hit object</returns>
    protected virtual EditorHitObject CreateHitObject() => null;
}
#endif