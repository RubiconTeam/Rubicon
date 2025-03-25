namespace Rubicon.Menus;

[GlobalClass] public abstract partial class SubMenu : Control
{
    [Export] public Control InitialFocus;

    [Export] public Control[] Focusable = [];

    public override void _Ready()
    {
        base._Ready();
        
        InitialFocus?.GrabFocus();
        for (int i = 0; i < Focusable.Length; i++)
        {
            Control cur = Focusable[i];
            cur.FocusEntered += () => UpdateSelection(cur);
        }
    }
    
    public abstract void UpdateSelection(Control focused);
}