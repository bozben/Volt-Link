using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveForce = 20f;
    [SerializeField] private float linearDamping = 2f;
    [SerializeField] private float playerMass = 1f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        SetupPhysics();
    }
    private void SetupPhysics()
    {
        rb.mass = playerMass;
        rb.linearDamping = linearDamping;
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    private void FixedUpdate()
    {
        if (moveInput.sqrMagnitude > 0.01f)
        {
            rb.AddForce(moveInput.normalized * moveForce);
        }
    }
}
