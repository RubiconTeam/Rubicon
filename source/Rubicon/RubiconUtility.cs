using System.Collections.Generic;
using System.Linq;

namespace Rubicon;

/// <summary>
/// A general purpose utility class for Rubicon Engine
/// I'm not naming this CoolUtil bro - binpuki
/// :( - riven
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

	public static string ToMemoryFormat(this ulong mem)
	{
		return mem switch
		{
			>= 0x40000000 => (float)Math.Round(mem / 1024f / 1024f / 1024f, 2) + " GB",
			>= 0x100000 => (float)Math.Round(mem / 1024f / 1024f, 2) + " MB",
			>= 0x400 => (float)Math.Round(mem / 1024f, 2) + " KB",
			_ => mem + " B"
		};
	}
}
