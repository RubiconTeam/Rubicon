
using Charter.Scripts;

namespace Charter;

[Tool]
public partial class ChartEditor : Control
{
    public CharterPreferenceManager preferenceManager = new();
    /*public enum WindowCloseType
    {
        DESTROY,
        HIDE,
        NONE
    }

    public void DuplicateWindow(string WindowPath)//, WindowCloseType CloseRequestType = WindowCloseType.DESTROY)
    {
        Window newWindow = GetNode<Window>(WindowPath).Duplicate() as Window;
        GetNode("Windows").AddChild(newWindow);
        if (CloseRequestType != WindowCloseType.NONE)
            newWindow.CloseRequested += () =>
            {
                if (CloseRequestType == WindowCloseType.DESTROY)
                    DestroyWindow(GetPathTo(newWindow).ToString().Replace("Windows/",""));
                else
                    CloseWindow(GetPathTo(newWindow).ToString().Replace("Windows/", ""));
            };
    }*/

    public void ShowWindow(string WindowPath)
    {
        GetNode<Window>($"Windows/{WindowPath}").PopupCentered();
    }

    public void CloseWindow(string WindowPath)
    {
        GetNode<Window>($"Windows/{WindowPath}").Visible = false;
    }

    public void DestroyWindow(string WindowPath)
    {
        GetNode<Window>($"Windows/{WindowPath}").QueueFree();
    }

    public void MakeFileDialog(string LineEditPath)
    {
        if (Engine.IsEditorHint())
        {
            EditorFileDialog fileDialog = new()
            {
                Title = "Select a chart",
                FileMode = EditorFileDialog.FileModeEnum.OpenFile,
                //CurrentDir = "res://songs/",
                Size = new Vector2I(512, 512),
                InitialPosition = Window.WindowInitialPosition.CenterMainWindowScreen,
                Filters = ["*.tres"]
            };
            fileDialog.FileSelected += (string path) => GetNode<LineEdit>(LineEditPath).Text = path;
            AddChild(fileDialog);
        }
        else
        {
            FileDialog fileDialog = new()
            {
                Title = "Select a chart",
                FileMode = FileDialog.FileModeEnum.OpenFile,
                Size = new Vector2I(512,512),
                InitialPosition = Window.WindowInitialPosition.CenterMainWindowScreen,
                Filters = ["*.tres"],
                RootSubfolder = "res://Songs"
            };
            fileDialog.FileSelected += (string path) => GetNode<LineEdit>(LineEditPath).Text = path;
            AddChild(fileDialog);
        }
    }

    public void ShowAgainToggle(bool toggle)
    {
        preferenceManager.Preferences.ShowWelcomeWindow = toggle;
        //preferenceManager.Save();
    }
}
