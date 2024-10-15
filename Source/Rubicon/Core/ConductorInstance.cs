using Rubicon.Core.Chart;
using Rubicon.Data.Generation;

namespace Rubicon.Core.Autoload;

/// <summary>
/// A Node that keeps track of musical timing and what not.
/// </summary>
[GlobalClass, StaticAutoloadSingleton("Rubicon.Core", "Conductor")]
public partial class ConductorInstance : Node
{
	#region Status Variables
	/// <summary>
	/// The current BPM at the moment.
	/// </summary>
	[Export] public double Bpm = 100;

	/// <summary>
	/// The index pointing to which BPM is currently set based on the bpms array.
	/// </summary>
	[Export] public int BpmIndex = 0;

	/// <summary>
	/// Corrects the time so the chart can be accurate.
	/// </summary>
	[Export] public double ChartOffset = 0f;

	/// <summary>
	/// How fast the Conductor goes.
	/// </summary>
	[Export] public double Speed = 1f;

	/// <summary>
	/// Is true when the Conductor has been started with Start() or Play(), false when either Pause() or Stop() is called.
	/// </summary>
	[Export] public bool Playing = false;
	#endregion

	#region Time Variables
	/// <summary>
	/// The raw timestamp of this Conductor, without any corrections made to it.
	/// </summary>
	public double RawTime => GetRawTime();
		
	/// <summary>
	/// The raw timestamp of this Conductor + the chart offset.
	/// </summary>
	public double UncorrectedTime => GetUncorrectedTime();

	/// <summary>
	/// The current timestamp from when the time was last set.
	/// Equivalent to Conductor.songPosition in most other FNF projects.
	/// </summary>
	public double Time { get => GetTime(); set => SetTime(value); }

	/// <summary>
	/// The current step according to the time, which also keeps BPM changes in mind.
	/// </summary>
	public double CurrentStep => GetCurrentStep();

	/// <summary>
	/// The current beat according to the time, which also keeps BPM changes in mind.
	/// </summary>
	public double CurrentBeat => GetCurrentBeat();

	/// <summary>
	/// The current measure according to the time, which also keeps BPM changes in mind.
	/// </summary>
	public double CurrentMeasure => GetCurrentMeasure();
	#endregion

	#region Other Variables
	/// <summary>
	/// Get all BPMs listed in the Conductor currently.
	/// </summary>
	[ExportGroup("Info")] 
	[Export] public BpmInfo[] BpmList { get => GetBpmList(); set => SetBpmList(value); }
		
	/// <summary>
	/// A whole number that indicates the number of beats in each measure.
	/// </summary>
	[Export] public int TimeSigNumerator = 4;
		
	/// <summary>
	/// A whole number representing how many steps are in a beat.
	/// </summary>
	[Export] public int TimeSigDenominator = 4;
	#endregion

	#region Signals
	/// <summary>
	/// Event triggered when the next beat is hit.
	/// </summary>
	[Signal] public delegate void BeatHitEventHandler(int beat);

	/// <summary>
	/// Event triggered when the next step is hit.
	/// </summary>
	[Signal] public delegate void StepHitEventHandler(int step);
	
	/// <summary>
	/// Event triggered when the next measure is hit.
	/// </summary>
	[Signal] public delegate void MeasureHitEventHandler(int measure);
	#endregion

	#region Private Fields
	private double _relativeStartTime = 0;
	private double _relativeTimeOffset = 0;
	private double _lastTime = double.NegativeInfinity;
	private double _delta = 0d;
	private double _time = 0d;
		
	private double _cachedStep;
	private double _cachedStepTime;

	private double _cachedBeat;
	private double _cachedBeatTime;

	private double _cachedMeasure;
	private double _cachedMeasureTime;
		
	private BpmInfo[] _bpmList = { new() { Bpm = 100 } };

	private int _lastBeat = -int.MaxValue;
	private int _lastStep = -int.MaxValue;
	private int _lastMeasure = -int.MaxValue;
	#endregion
		
	#region Public Methods
	public override void _Process(double delta)
	{
		if (!Playing)
		{
			_relativeStartTime = Godot.Time.GetUnixTimeFromSystem();
			_relativeTimeOffset = _time;
		}

		base._Process(delta);
			
		// Handles bpm changing
		if (BpmIndex < BpmList.Length - 1 && BpmList[BpmIndex + 1].MsTime / 1000f <= Time)
		{
			BpmIndex++;
			Bpm = BpmList[BpmIndex].Bpm;
		}
		
		int curMeasure = Mathf.FloorToInt(CurrentMeasure);
		int curBeat = Mathf.FloorToInt(CurrentBeat);
		int curStep = Mathf.FloorToInt(CurrentStep);

		if (curMeasure != _lastMeasure)
			EmitSignal(SignalName.MeasureHit, curMeasure);

		if (curBeat != _lastBeat)
			EmitSignal(SignalName.BeatHit, curBeat);

		if (curStep != _lastStep)
			EmitSignal(SignalName.StepHit, curStep);

		_lastMeasure = curMeasure;
		_lastBeat = curBeat;
		_lastStep = curStep;
	}
		
	/// <summary>
	/// Starts the Conductor at the time provided.
	/// </summary>
	/// <param name="time">The time the Conductor starts at. Default is 0</param>
	public void Play(double time = 0d)
	{
		Resume();
		Time = time;
	}

	/// <summary>
	/// Resumes the Conductor at the last time it was paused at.
	/// </summary>
	public void Resume()
	{
		Playing = true;
	}

	/// <summary>
	/// Stops the Conductor from ticking, but keeps the current time.
	/// </summary>
	public void Pause()
	{
		_time = Time;
		Playing = false;
	}

	/// <summary>
	/// Stops the Conductor entirely, resetting its time to 0.
	/// </summary>
	public void Stop()
	{
		Time = 0;
		Pause();
	}
		
	/// <summary>
	/// Resets the Conductor, wiping all its fields with its default values.
	/// </summary>
	public void Reset()
	{
		TimeSigNumerator = TimeSigDenominator = 4;
		BpmList = [new BpmInfo { Bpm = 100 }];
		Bpm = BpmList[0].Bpm;
		BpmIndex = 0;
		ChartOffset = 0;
		Speed = 1f;
		Stop();
	}
	#endregion
		
	#region Getters and Setters
	/// <summary>
	/// Gets the raw time of this Conductor, without any corrections made to it.
	/// </summary>
	/// <returns>The raw time, in seconds</returns>
	public double GetRawTime()
	{
		return Playing ? Godot.Time.GetUnixTimeFromSystem() - _relativeStartTime + _relativeTimeOffset :
			_time != 0d ? _time : 0d;
	}

	/// <summary>
	/// Gets the raw time of this Conductor + the chart offset.
	/// </summary>
	/// <returns>The raw time + the chart offset, in seconds</returns>
	public double GetUncorrectedTime()
	{
		return RawTime + ChartOffset / 1000d;
	}

	/// <summary>
	/// Gets the calculated time of this Conductor.
	/// </summary>
	/// <returns>The raw time + the chart offset multiplied by the speed, in seconds</returns>
	public double GetTime()
	{
		return UncorrectedTime * Speed;
	}

	/// <summary>
	/// Sets the time of this Conductor.
	/// </summary>
	/// <param name="time">The time to set it to, in seconds.</param>
	public void SetTime(double time)
	{
		_time = time;
		_relativeStartTime = Godot.Time.GetUnixTimeFromSystem();
		_relativeTimeOffset = time;
	}

	/// <summary>
	/// Gets the current step of this Conductor, with decimals.
	/// </summary>
	/// <returns>The current step</returns>
	public double GetCurrentStep()
	{
		if (_cachedStepTime == Time)
			return _cachedStep;

		if (BpmList.Length <= 1)
			return Time / (60d / (Bpm * TimeSigDenominator));

		_cachedStepTime = Time;
		_cachedStep = (Time - BpmList[BpmIndex].MsTime / 1000d) / (60 / (Bpm * TimeSigDenominator)) + (BpmList[BpmIndex].Time * TimeSigNumerator * TimeSigDenominator);
		return _cachedStep;
	}

	/// <summary>
	/// Gets the current beat of this Conductor, with decimals.
	/// </summary>
	/// <returns>The current beat</returns>
	public double GetCurrentBeat()
	{
		if (_cachedBeatTime == Time)
			return _cachedBeat;

		if (BpmList.Length <= 1)
			return Time / (60d / Bpm);

		_cachedBeatTime = Time;
		_cachedBeat = (Time - BpmList[BpmIndex].MsTime / 1000d) / (60d / Bpm) + BpmList[BpmIndex].Time * TimeSigNumerator;
		return _cachedBeat;
	}

	/// <summary>
	/// Gets the current measure of this Conductor, with decimals.
	/// </summary>
	/// <returns>The current measure</returns>
	public double GetCurrentMeasure()
	{
		if (_cachedMeasureTime == Time)
			return _cachedMeasure;

		if (BpmList.Length <= 1)
			return Time / (60d / (Bpm / TimeSigNumerator));

		_cachedMeasureTime = Time;
		_cachedMeasure = (Time - BpmList[BpmIndex].MsTime / 1000d) / (60d / (Bpm / TimeSigNumerator)) + BpmList[BpmIndex].Time;
		return _cachedMeasure;
	}

	/// <summary>
	/// Get all BPMs listed in the Conductor currently.
	/// </summary>
	/// <returns>All BPMs listed in the Conductor</returns>
	public BpmInfo[] GetBpmList() => _bpmList;
		
	/// <summary>
	/// Set the BPM list in the Conductor.
	/// </summary>
	/// <param name="data">An array of BpmInfos</param>
	public void SetBpmList(BpmInfo[] data)
	{
		_bpmList = data;
		Bpm = BpmList[0].Bpm;
	}
	#endregion
}
