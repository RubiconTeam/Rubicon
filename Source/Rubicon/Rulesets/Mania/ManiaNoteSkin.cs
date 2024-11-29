using Godot.Collections;
using CollectionExtensions = System.Collections.Generic.CollectionExtensions;

namespace Rubicon.Rulesets.Mania;

/// <summary>
/// A resource that holds important information for Mania-related graphics.
/// </summary>
[GlobalClass] public partial class ManiaNoteSkin : Resource
{
	/// <summary>
	/// The SpriteFrames resource to grab note textures from.
	/// </summary>
	[Export] public SpriteFrames NoteAtlas;

	/// <summary>
	/// The SpriteFrames resource to grab lane textures from.
	/// </summary>
	[Export] public SpriteFrames LaneAtlas;

	/// <summary>
	/// The SpriteFrames resource to grab hold and tail textures from.
	/// </summary>
	[Export] public SpriteFrames HoldAtlas;
	
	/// <summary>
	/// The scale used when generating notes and lanes.
	/// </summary>
	[Export] public Vector2 Scale = Vector2.One;

	/// <summary>
	/// The width of each lane.
	/// </summary>
	[Export] public float LaneSize = 160f;

	/// <summary>
	/// The filtering used when generating notes and lanes.
	/// </summary>
	[Export] public CanvasItem.TextureFilterEnum Filter = CanvasItem.TextureFilterEnum.Linear;

	/// <summary>
	/// Specifies direction names for each lane count.
	/// </summary>
	[Export] public Dictionary<int, string[]> Directions = new() { { 4, [ "left", "down", "up", "right" ] } };

	/// <summary>
	/// Whether to enable tiling on hold graphics. Hold textures in <see cref="HoldAtlas"/> should NOT be an <see cref="AtlasTexture"/>/part of a sprite sheet!
	/// </summary>
	[Export] public bool UseTiledHold = false;

	/// <summary>
	/// Gets a direction name based on lane count and lane number.
	/// </summary>
	/// <param name="lane">The lane index</param>
	/// <param name="laneCount">The amount of lanes.</param>
	/// <returns>A direction name if found (Ex: "left"), otherwise an empty name.</returns>
	public string GetDirection(int lane, int laneCount = 4)
	{
		string[] directions = GetDirections(laneCount);
		if (lane < directions.Length && lane >= 0)
			return directions[lane];

		return "";
	}
	
	/// <summary>
	/// Gets an array of directions based on the lane count provided.
	/// </summary>
	/// <param name="laneCount">The amount of lanes.</param>
	/// <returns>An array of direction names. (Ex: ["left", "down", "up", "right"])</returns>
	public string[] GetDirections(int laneCount = 4)
	{
		return CollectionExtensions.GetValueOrDefault(Directions, laneCount);
	}
}
