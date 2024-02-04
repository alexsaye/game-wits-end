/// <summary>
/// Provides error correction.
/// </summary>
public interface ICorrector
{
  /// <summary>
  /// Calculate the error correction.
  /// </summary>
  public float Correct(float error);
}
