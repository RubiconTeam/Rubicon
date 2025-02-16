using Rubicon.Core.API;
using Rubicon.Core.Data;
using Rubicon.Core.Rulesets;

namespace Rubicon.Extras.UI;

[GlobalClass] public partial class FunkinScorePanel : Label, IPlayElement
{
    [Export] public Label TextLabel;

    public PlayField PlayField { get; set; }
    
    public void Initialize() { }

    public override void _Process(double delta)
    {
        base._Process(delta);

        ScoreTracker scoreTracker = PlayField.ScoreTracker;
        TextLabel.Text = $"Score: {scoreTracker.Score} / Accuracy: {scoreTracker.Accuracy:n2}% / Misses: {scoreTracker.Misses} / Rank: {scoreTracker.Rank.ToString().ToUpper()} ({GetClearText(scoreTracker.Clear)})";
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