using Rubicon.Core.Meta;

namespace Rubicon.Data;

[GlobalClass] public partial class WeekData : Resource
{
    [Export] public string Name;
    
    [Export] public SongMeta[] Songs;
}