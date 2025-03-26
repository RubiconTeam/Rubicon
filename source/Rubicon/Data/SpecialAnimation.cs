using Rubicon.View2D;
using Rubicon.View3D;

namespace Rubicon.Data;

/// <summary>
/// A class that is used by the <see cref="Character2D"/> or <see cref="Character3D"/> class to determine, which animation should be played and if it should be played or not
/// among other stuff.
/// </summary>
[GlobalClass]
public partial class SpecialAnimation : RefCounted
{
	/// <summary>
	/// The name of the animation played by the <see cref="AnimationPlayer"/>. It has to match exactly.
	/// </summary>
	public string Name = "idle";

	/// <summary>
	/// Prevents character from dancing while this special animation is playing.
	/// </summary>
	public bool OverrideDance = false;

	/// <summary>
	/// Prevents character from singing while this special animation is playing.
	/// </summary>
	public bool OverrideSing = false;

	/// <summary>
	/// The time that the animation will start at. 0 by default.
	/// </summary>
	public float StartTime = 0f;
}