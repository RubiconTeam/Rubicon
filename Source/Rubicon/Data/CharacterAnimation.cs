namespace Rubicon.Data;

/// <summary>
/// A class that is used by the <see cref="Character2D"/> or <see cref="Character3D"/> class to determine, which animation should be played and if it should be played or not
/// among other stuff.
/// </summary>
[GlobalClass]
public partial class CharacterAnimation : RefCounted
{
	/// <summary>
	/// The name of the animation played by the <see cref="AnimationPlayer"/>. It has to match exactly.
	/// </summary>
	public string Name = "idle";

	/// <summary>
	/// Overrides any animation currently playing on the character, no matter what.
	/// </summary>
	public bool Force = false;

	/// <summary>
	/// The time that the animation will start at. 0 by default.
	/// </summary>
	public float StartTime = 0f;

    /// <summary>
    /// A prefix for the animation played by the <see cref="AnimationPlayer"/>. Overrides the <paramref name="StaticPrefix"/> found in the <see cref="Character2D"/> or <see cref="Character3D"/> class.
    /// </summary>
    public string CustomPrefix;

    /// <summary>
    /// A suffix for the animation played by the <see cref="AnimationPlayer"/>. Overrides the <paramref name="StaticSuffix"/> found in the Character class.
    /// </summary>
    public string CustomSuffix;

	/// <summary>
	/// This animation will play at the end of the current one.
	/// </summary>
	public CharacterAnimation PostAnimation;
}