using UnityEngine;

/// <summary>
/// A behaviour which applies correction movement towards a target position.
/// </summary>
public class MoveTowardsTarget : MonoBehaviour
{
  private Rigidbody rb;

  [Header("Target")]

  [Tooltip("The target transform to move towards. If not specified, the target position is in world space.")]
  public Transform TargetTransform;

  [Tooltip("The target position in the target transform's local space (or world space if no target transform is specified).")]
  public Vector3 TargetPosition;

  [Header("Correction")]

  public PIDCorrector X;
  public PIDCorrector Y;
  public PIDCorrector Z;

  public Space Space = Space.Self;

  protected void Start()
  {
    rb = GetComponent<Rigidbody>();
  }

  private void FixedUpdate()
  {
    var target = TargetPosition;
    if (TargetTransform != null)
    {
      target = TargetTransform.TransformPoint(target);
    }

    var deltaTarget = target - transform.position;
    if (Space == Space.Self)
    {
      deltaTarget = transform.InverseTransformDirection(deltaTarget);
    }

    var dx = X.Correct(deltaTarget.x);
    var dy = Y.Correct(deltaTarget.y);
    var dz = Z.Correct(deltaTarget.z);

    if (rb == null)
    {
      transform.Translate(dx * Time.fixedDeltaTime, dy * Time.fixedDeltaTime, dz * Time.fixedDeltaTime, Space);
    }
    else if (Space == Space.Self)
    {
      rb.AddRelativeForce(dx, dy, dz, ForceMode.Force);
    }
    else
    {
      rb.AddForce(dx, dy, dz, ForceMode.Force);
    }
  }
}
