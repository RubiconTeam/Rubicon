namespace Rubicon.Data;

[GlobalClass] public partial class WeekDatabase : Resource
{
    [Export] public WeekData[] Weeks = [];
}