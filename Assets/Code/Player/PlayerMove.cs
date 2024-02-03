using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    private Rigidbody rb;

    private Vector2 moveInput;

    [SerializeField]
    private float moveSpeed = 1f;

    [SerializeField]
    private float moveAcceleration = 100f;

    protected void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected void FixedUpdate()
    {
        if (moveInput != Vector2.zero)
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
        }
    }

    protected void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
}
