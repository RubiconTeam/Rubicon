using Rubicon.API;
using Rubicon.Core.Rulesets;
using Rubicon.Data;
using Rubicon.Game;
using Rubicon.View2D;

namespace Rubicon.Extras.Scripts;

public partial class TutorialScript : CsModChart
{
	public Character2D Boyfriend;
	public Character2D Girlfriend;
	
	public override void _Ready()
	{
		base._Ready();

		Boyfriend = RubiconGame.CanvasItemSpace.GetCharacter("Player");
		Girlfriend = RubiconGame.CanvasItemSpace.GetCharacter("Speaker");
	}

	// Triggers every beat.
	public override void BeatHit(int beat)
	{
		if (beat is < 16 or > 48 || beat % 16 != 15)
		{
			Boyfriend.CanDance = Girlfriend.CanDance = true;
			return;
		}

		Boyfriend.CanDance = Girlfriend.CanDance = false;
		Boyfriend.PlayAnim(new CharacterAnimation { Name = "hey", Force = true });
		Girlfriend.PlayAnim(new CharacterAnimation { Name = "cheer", Force = true });
	}
}

