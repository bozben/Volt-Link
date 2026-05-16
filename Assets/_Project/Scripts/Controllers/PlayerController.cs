using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float forwardForce = 20f;
    [SerializeField] private float brakeForce = 10f;
    [SerializeField] private float steerTorque = 5f;

    [Header("Physics Tuning")]
    [SerializeField] private float linearDamping = 2f;
    [SerializeField] private float angularDamping = 2f; 
    [SerializeField] private float playerMass = 1f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        SetupPhysics();
    }
    private void FixedUpdate()
    {
        HandleMovement();
        HandleSteering();
    }
    private void SetupPhysics()
    {
        rb.mass = playerMass;
        rb.linearDamping = linearDamping;
        rb.angularDamping = angularDamping; // Crucial to prevent infinite spinning
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        rb.constraints = RigidbodyConstraints2D.None;
    }
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    
    private void HandleMovement()
    {
        if (moveInput.y > 0.1f)
        {
            rb.AddForce(transform.up * moveInput.y * forwardForce);
        }
        else if (moveInput.y < -0.1f)
        {
            rb.AddForce(-transform.up * Mathf.Abs(moveInput.y) * brakeForce);
        }
    }
    private void HandleSteering()
    {
        if (Mathf.Abs(moveInput.x) > 0.1f)
        {
            rb.AddTorque(-moveInput.x * steerTorque);
        }
    }

}
