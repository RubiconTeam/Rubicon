using Rubicon.API;
using Rubicon.Core.Data;

// This is a template for certain UI elements that hook into the game (i.e. combos, judgment).
// This can also act as a Node! So yes, you will have access to such things like _Ready() and _Process(delta).
public partial class NewStatDisplay : CsStatDisplay
{
    // Called whenever the player hits or misses a note.
    public override void UpdateStats(long combo, HitType hit, float distance)
    {
        
    }
}
