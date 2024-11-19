namespace Rubicon.Game;

[GlobalClass]
public partial class SongStorySequence : StorySequence
{
    [Export] public string Name = ProjectSettings.GetSetting("rubicon/general/fallback/song").AsString();
    
    [Export] public string RuleSet = ProjectSettings.GetSetting("rubicon/rulesets/default_ruleset").AsString();

    [Export] public bool FreePlay = false;
    
    public override void Execute()
    {
        RubiconGame.Context = new LoadContext { Name = Name, Difficulty = StoryMode.Difficulty, RuleSet = RuleSet };
        // TODO: Actually load into RubiconGame LOL
    }
}