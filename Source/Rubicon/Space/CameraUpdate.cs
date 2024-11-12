namespace Rubicon.Space;

/// <summary>
/// An enumerator specifying the type of update method on a certain part of the camera.
/// </summary>
public enum CameraUpdate : uint
{
    /// <summary>
    /// Doesn't update the camera at all.
    /// </summary>
    None,
    
    /// <summary>
    /// Immediate setting of the camera's value.
    /// </summary>
    Instant,
    
    /// <summary>
    /// Uses interpolation to update the camera over time.
    /// </summary>
    Interpolation,
    
    /// <summary>
    /// Tween the camera's value to the final value.
    /// </summary>
    Tween
}