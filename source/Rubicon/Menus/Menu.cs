using PukiTools.GodotSharp.Screens;

namespace Rubicon.Menus;

[GlobalClass] public abstract partial class Menu : CsScreen
{
    [Export] public Control InitialFocus;

    [Export] public Control[] Focusable = [];

    public override void _Ready()
    {
        base._Ready();

        if (!IsLoaded())
            return;
        
        InitialFocus?.GrabFocus();
        for (int i = 0; i < Focusable.Length; i++)
        {
            Control cur = Focusable[i];
            cur.FocusEntered += () => UpdateSelection(cur);
        }
    }
    
    public abstract void UpdateSelection(Control focused);
}