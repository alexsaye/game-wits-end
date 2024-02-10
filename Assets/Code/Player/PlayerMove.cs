using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    private Rigidbody rb;

    public bool IsDodging => dodgeTimer > 0f;

    private bool HasControl => true;

    [Header("Move")]

    private Vector2 moveInput;

    [SerializeField]
    private float moveSpeed = 1f;

    [SerializeField]
    private float moveAcceleration = 100f;

    [Header("Dodge")]

    private bool dodgeInput;

    [SerializeField]
    private float dodgeSpeed = 10f;

    [SerializeField]
    private float dodgeCooldown = 1f;

    private float dodgeTimer = 0f;

    protected void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected void FixedUpdate()
    {
        UpdateMove();
        UpdateDodge();
    }

    protected void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void UpdateMove()
    {
        if (moveInput != Vector2.zero && HasControl)
        {
            // Calculate the input velocity relative to the camera through the player's transverse (xz) plane.
            var forward = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z).normalized * moveInput.y;
            var right = new Vector3(Camera.main.transform.right.x, 0f, Camera.main.transform.right.z).normalized * moveInput.x;
            var inputVelocity = (forward + right) * moveAcceleration * Time.fixedDeltaTime;

            // Get the player's current transverse velocity.
            var transverseVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // Modulate the max speed by the input magnitude, matching the already modulated acceleration, so that the player can control pace.
            var maxSpeed = moveSpeed * moveInput.magnitude;

            // Determine how much additional speed would be required to get up to the max speed.
            var deficitSpeed = Mathf.Max(maxSpeed - transverseVelocity.magnitude, 0f);

            // Get the uncapped additional speed from the input velocity.
            var inputSpeed = inputVelocity.magnitude;

            // Calculate the permitted move velocity by adding the input velocity, limited to the deficit speed, to the current transverse velocity.
            var moveVelocity = transverseVelocity + inputVelocity * Mathf.Min(inputSpeed, deficitSpeed) / inputSpeed;

            // Set the velocity to the transferse velocity, plus the extra move velocity, with unchanged vertical velocity.
            rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);

            // Face the direction moved in.
            transform.forward = moveVelocity.normalized;
        }
    }

    protected void OnDodge(InputValue value)
    {
        dodgeInput = value.isPressed;
    }

    private void UpdateDodge()
    {
        if (IsDodging)
        {
            // Tick down the dodge timer.
            Debug.Log($"Dodging: {dodgeTimer} seconds left.");
            dodgeTimer -= Time.fixedDeltaTime;
        }
        else if (dodgeInput && HasControl)
        {
            // Perform a dodge by setting velocity instantly in the faced direction.
            rb.velocity = transform.forward * dodgeSpeed;

            // Start the dodge timer, disallowing movement until it has expired.
            dodgeTimer = dodgeCooldown;
        }
    }
}
