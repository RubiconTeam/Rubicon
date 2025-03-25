using Rubicon.Data;

namespace Rubicon.Menus.FreePlay;

public partial class FreePlayMenu : SubMenu
{
    [Export] public WeekDatabase Database;

    [Export] public PackedScene SongTemplate;

    public override void UpdateSelection(Control focused)
    {
        
    }

    public override void _Ready()
    {
        base._Ready();
        
        
    }
}