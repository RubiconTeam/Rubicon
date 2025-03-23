using PukiTools.GodotSharp.Screens;

namespace Rubicon.Menus;

[GlobalClass] public abstract partial class Menu : CsScreen
{
    [Export] public Control InitialFocus;

    [Export] public Control[] Focusable = [];
    
    public abstract void UpdateSelection();

    public override void _Ready()
    {
        base._Ready();

        if (!IsPreloaded())
            return;
        
        InitialFocus?.GrabFocus();
    }
}