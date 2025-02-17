using Rubicon.Data;

namespace Rubicon;

/// <summary>
/// A class that holds useful data for camera motion, such as Lerping, Tweening and Smoothing.
/// </summary>
public partial class CameraMotionData : Resource
{
    /// <summary>
    /// Motion update type.
    /// See <see cref="CameraUpdate"/> for all the update types.
    /// </summary>
    [ExportGroup("Update Settings"), Export] public CameraUpdate UpdateType = CameraUpdate.Interpolation;
    
    /// <summary>
    /// Weight value when calculating motion lerp.
    /// Only affects <see cref="CameraUpdate.Interpolation"/> update type.
    /// </summary>
    [ExportSubgroup("Interpolation"), Export] public float LerpWeight = 2.4f;

    /// <summary>
    /// Duration of the motion tweens.
    /// Only affects <see cref="CameraUpdate.Tween"/> update type.
    /// </summary>
    [ExportSubgroup("Tweening"), Export] public float TweenDuration = 0.5f;
    [Export(PropertyHint.Enum)] public Tween.TransitionType TweenTrans = Tween.TransitionType.Cubic;
    [Export(PropertyHint.Enum)] public Tween.EaseType TweenEase = Tween.EaseType.Out;
    
    public Tween MotionTween;
    public bool RunningTween;
}