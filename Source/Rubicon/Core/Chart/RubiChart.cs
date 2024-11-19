using System.Collections.Generic;
using System.Linq;

namespace Rubicon.Core.Chart;

/// <summary>
/// The general chart format for this engine.
/// </summary>
[GlobalClass]
public partial class RubiChart : Resource
{
    #region Public Variables
    /// <summary>
    /// The name of the people who helped with this chart.
    /// </summary>
    [Export] public string Charter = "";
    
    /// <summary>
    /// How hard the chart really is.
    /// </summary>
    [Export] public uint Difficulty = 0;

    /// <summary>
    /// The Rubicon Engine version this chart was created on.
    /// </summary>
    [Export] public uint Version = RubiconEngineInstance.Version.Raw;
    
    /// <summary>
    /// A list of BPM changes.
    /// </summary>
    [Export] public BpmInfo[] BpmInfo = [];

    /// <summary>
    /// The default scroll speed for this chart.
    /// </summary>
    [Export] public float ScrollSpeed = 1.6f;

    /// <summary>
    /// The chart offset.
    /// </summary>
    [Export] public double Offset;

    /// <summary>
    /// The individual charts (or "strum lines") that each contain its own notes.
    /// </summary>
    [Export] public IndividualChart[] Charts = [];
    #endregion

    #region Public Methods
    /// <summary>
    /// Converts everything in this chart to millisecond format.
    /// </summary>
    /// <returns>Itself</returns>
    public RubiChart ConvertData()
    {
        // Takes care of setting bpm and time signature's exact millisecond time.
        for (int i = 1; i < BpmInfo.Length; i++)
            BpmInfo[i].MsTime = BpmInfo[i - 1].MsTime + ConductorUtility.MeasureToMs(BpmInfo[i].Time - BpmInfo[i - 1].Time, BpmInfo[i - 1].Bpm, BpmInfo[i].TimeSignatureNumerator);

        foreach (IndividualChart curChart in Charts)
        {
            for (int n = 0; n < curChart.Notes.Length; n++)
                curChart.Notes[n].ConvertData(BpmInfo, curChart.SvChanges);

            if (curChart.SvChanges.Length <= 1)
                continue;

            for (int s = 1; s < curChart.SvChanges.Length; s++)
                curChart.SvChanges[s].ConvertData(BpmInfo, curChart.SvChanges[s - 1]);
        }
        
        return this;
    }

    /// <summary>
    /// Sorts the notes properly and attempts to get rid of any duplicate notes and notes inside holds.
    /// </summary>
    public void Format()
    {
        for (int c = 0; c < Charts.Length; c++)
        {
            List<NoteData> notes = new List<NoteData>();

            for (int l = 0; l < Charts[c].Lanes; l++)
            {
                List<NoteData> lane = Charts[c].Notes.Where(x => x.Lane == l).ToList();
                lane.Sort((x, y) =>
                {
                    if (x.Time < y.Time)
                        return -1;
                    if (x.Time > y.Time)
                        return 1;

                    return 0;
                });

                for (int i = 0; i < lane.Count - 1; i++)
                {
                    if (lane[i].Length > 0)
                    {
                        double start = lane[i].Time;
                        double end = lane[i].Time + lane[i].Length;
                        while (i < lane.Count - 1 && lane[i + 1].Time >= start && lane[i + 1].Time < end)
                        {
                            GD.Print($"Removed note inside hold note area at {lane[i + 1].Time} in lane {l} ({start}-{end})");
                            lane.RemoveAt(i + 1);
                        }
                    }

                    while (i < lane.Count - 1 && lane[i + 1].Time == lane[i].Time)
                    {
                        GD.Print($"Removed duplicate note at {lane[i + 1].Time} in lane {l}");
                        lane.RemoveAt(i + 1);
                    }
                }

                notes.AddRange(lane);
            }

            notes.Sort((x, y) =>
            {
                if (x.Time < y.Time)
                    return -1;
                if (x.Time > y.Time)
                    return 1;

                if (x.Lane < y.Lane)
                    return -1;
                if (x.Lane > y.Lane)
                    return 1;

                return 0;
            });

            Charts[c].Notes = notes.ToArray();
        }
    }
    #endregion
}