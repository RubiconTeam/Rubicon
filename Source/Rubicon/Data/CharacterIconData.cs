using TextureFilterEnum = Godot.CanvasItem.TextureFilterEnum;

namespace Rubicon.Data;

/// <summary>
/// An icon set for a character in-game.
/// </summary>
[GlobalClass] public partial class CharacterIconData : Resource
{
    /// <summary>
    /// The icon for this character.
    /// </summary>
    [Export] public SpriteFrames Icon;

    /// <summary>
    /// Moves this character's icon in pixels.
    /// </summary>
    [Export] public Vector2 Offset = new(0f, 10f);

    /// <summary>
    /// Used as the color representing this character on the health bar.
    /// </summary>
    [Export] public Color Color = new("#A1A1A1");
    
    /// <summary>
    /// How much to scale the icon.
    /// </summary>
    [Export] public Vector2 Scale = Vector2.One;

    /// <summary>
    /// The filter the icon will use.
    /// </summary>
    [Export] public TextureFilterEnum Filter = TextureFilterEnum.Linear;
}