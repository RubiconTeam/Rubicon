global using Godot;
global using Godot.Sharp.Extras;
global using System;

using Rubicon.Data.Generation;

namespace Rubicon;

/// <summary>
/// A Node that contains basic engine info. Essentially the Main class.
/// More useful in GDScript than it is in C#.
/// </summary>
[GlobalClass, StaticAutoloadSingleton("Rubicon", "RubiconEngine")]
public partial class RubiconEngineInstance : Node
{
	/// <summary>
	/// The current version of Rubicon being used.
	/// </summary>
	public uint Version => GetVersion();

	/// <summary>
	/// A tag for the current version.
	/// </summary>
	public string VersionTag => GetVersionTag();

	/// <summary>
	/// The current Rubicon version, in string format.
	/// </summary>
	public string VersionToString => GetVersionToString();
	
	/// <summary>
	/// The scene that the game first starts with. Automatically set by <see cref="_Ready"/>.
	/// Will always be the main scene when exported, but can vary in editor.
	/// </summary>
	public string StartingScene;
	
	/// <summary>
	/// The type of node the starting scene is. Automatically set by <see cref="_Ready"/>.
	/// Will always be the main scene's type when exported, but can vary in editor.
	/// </summary>
	public Type StartingSceneType;
	
	public override void _Ready()
	{
		// Override the current scale size with the one set in the Rubicon project settings
		// This is done so that the editor can stay in a 16:9 aspect ratio while keeping
		// the 4:3 support in-game typically.
		GetWindow().ContentScaleSize = ProjectSettings.GetSetting("rubicon/general/content_minimum_size").AsVector2I();
		
		StartingScene = GetTree().CurrentScene.Name;
		StartingSceneType = GetTree().CurrentScene.GetType();
	}

	/// <inheritdoc cref="Version"/>
	public uint GetVersion() => RubiconUtility.CreateVersion(0, 1, 0, 0);

	/// <inheritdoc cref="VersionTag"/>
	public string GetVersionTag() => "-alpha";

	/// <inheritdoc cref="VersionToString"/>
	public string GetVersionToString() => RubiconUtility.VersionToString(GetVersion()) + GetVersionTag();
}