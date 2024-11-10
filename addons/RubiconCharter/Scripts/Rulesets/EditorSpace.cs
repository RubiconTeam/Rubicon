#if TOOLS
using Rubicon.Core.Chart;
using Godot.Collections;

namespace Rubicon.Editor.Rulesets;

/// <summary>
/// A class representing an individual chart's space to edit in.
/// </summary>
[Tool] public abstract partial class EditorSpace : Control
{
    public IndividualChart Chart;
    public float Zoom = 1f;
    
    /// <summary>
    /// A cache of every hit object created. Does not represent the children 
    /// </summary>
    protected Array<AnimatedSprite2D> HitObjects = new();
    
    public void LoadChart(IndividualChart chart)
    {
        Chart = chart;
        UpdateHitObjects();
    }

    public virtual void UpdateHitObjects()
    {
        
    }
}
#endif