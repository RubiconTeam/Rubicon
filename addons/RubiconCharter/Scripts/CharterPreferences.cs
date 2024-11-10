namespace Rubicon.Editor;

/// <summary>
/// This class holds chart editor preferences such as
/// chart editor settings, recent projects among others. (not planned yet)
/// The file will get saved in:
/// user://charter/
/// </summary>
[GlobalClass] public partial class CharterPreferences : Resource
{
    [Export] public bool ShowWelcomeWindow = true;
}

[Tool] public class CharterPreferenceManager
{
    public CharterPreferences Preferences = new();
}
