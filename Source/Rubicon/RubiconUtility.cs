using System.Linq;
using Rubicon.Core;
using Rubicon.Core.Data;
using Rubicon.Core.UI;

namespace Rubicon;

/// <summary>
/// A general purpose utility class for Rubicon Engine
/// I'm not naming this CoolUtil bro - binpuki
/// :( - duo
/// </summary>
public static class RubiconUtility
{
	/// <summary>
	/// Creates a number based on the versions provided below
	/// </summary>
	/// <param name="major">The major build</param>
	/// <param name="minor">The minor build</param>
	/// <param name="patch"></param>
	/// <param name="build"></param>
	/// <returns>A number based on the four versions</returns>
	public static uint CreateVersion(byte major, byte minor, byte patch, byte build) => ((uint)major << 24) | ((uint)minor << 16) | ((uint)patch << 8) | build;

	/// <summary>
	/// Get a judgment material based on the rating.
	/// </summary>
	/// <param name="instance">The IJudgmentMaterial instance</param>
	/// <param name="hitType">The rating</param>
	/// <returns>The Material associated</returns>
	public static Material GetHitMaterial(this IJudgmentMaterial instance, HitType hitType)
	{
		switch (hitType)
		{
			default:
				return instance.PerfectMaterial;
			case HitType.Great:
				return instance.GreatMaterial;
			case HitType.Good:
				return instance.GoodMaterial;
			case HitType.Okay:
				return instance.OkayMaterial;
			case HitType.Bad:
				return instance.BadMaterial;
			case HitType.Miss:
				return instance.MissMaterial;
		}
	}

	/// <summary>
	/// Applies the <see cref="UiStyle"/>'s materials onto the <see cref="IJudgmentMaterial"/> instance.
	/// </summary>
	/// <param name="instance">The current <see cref="IJudgmentMaterial"/> instance</param>
	/// <param name="style">The UI style to apply</param>
	public static void ApplyUiStyle(this IJudgmentMaterial instance, UiStyle style)
	{
		instance.PerfectMaterial = style.PerfectMaterial;
		instance.GreatMaterial = style.GreatMaterial;
		instance.GoodMaterial = style.GoodMaterial;
		instance.OkayMaterial = style.OkayMaterial;
		instance.BadMaterial = style.BadMaterial;
		instance.MissMaterial = style.MissMaterial;
	}
	
	/// <summary>
	/// Gets the first child that is of type specified.
	/// </summary>
	/// <typeparam name="T">The type</typeparam>
	/// <param name="node">The root node</param>
	/// <param name="includeInternal">Whether to recursively check for children inside</param>
	/// <returns>A child of type <see cref="T"/> if found, else null.</returns>
	public static T GetChildOfType<T>(this Node node, bool includeInternal = false) where T : class
	{
		return node.GetChildren(includeInternal).FirstOrDefault(x => x is T) as T;
	}

	/// <summary>
	/// Gets all children that is of type specified.
	/// </summary>
	/// <typeparam name="T">The type</typeparam>
	/// <param name="node">The root node</param>
	/// <param name="includeInternal">Whether to recursively check for children inside</param>
	/// <returns>An array of children of type <see cref="T"/> if found, else null.</returns>
	public static T[] GetChildrenOfType<T>(this Node node, bool includeInternal = false) where T : class
	{
		return node.GetChildren(includeInternal).Where(x => x is T).Cast<T>().ToArray();
	}
}
