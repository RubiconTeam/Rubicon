using Rubicon.Core.API;
using Rubicon.Core.Data;
using Rubicon.Core.Rulesets;

namespace Rubicon.Extras.UI;

/// <summary>
/// Score panel that imitates something you would find in a typical FNF mod.
/// </summary>
[GlobalClass] public partial class CsFunkinScorePanel : CsHudElement
{
    /// <summary>
    /// The label to set the score text on.
    /// </summary>
    [Export] public Label TextLabel;

    /// <summary>
    /// The seperator for the text.
    /// </summary>
    [Export] public string Divider = " / ";

    public override void Initialize() { }

    public override void OptionsUpdated() { }
    
    public override void _Process(double delta)
    {
        base._Process(delta);

        ScoreTracker scoreTracker = PlayField.ScoreTracker;
        TextLabel.Text = $"Score: {scoreTracker.Score}{Divider}Accuracy: {scoreTracker.Accuracy:n2}%{Divider}Misses: {scoreTracker.Misses}{Divider}Rank: {scoreTracker.Rank.ToString().ToUpper()} ({GetClearText(scoreTracker.Clear)})";
    }

    private string GetClearText(ClearRank clear)
    {
        switch (clear)
        {
            case ClearRank.Clear:
                return "Clear";
            case ClearRank.FullCombo:
                return "FC";
            case ClearRank.GreatFullCombo:
                return "Great FC";
            case ClearRank.Perfect:
                return "Perfect";
            default:
                return "Failure";
        }
    }
}