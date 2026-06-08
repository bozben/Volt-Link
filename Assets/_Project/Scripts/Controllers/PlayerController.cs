using Unity.VisualScripting;
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
    [SerializeField] private float maxDetachedSpeed = 15f;

    [Header("Physics Tuning")]
    [SerializeField] private float linearDamping = 2f;
    [SerializeField] private float angularDamping = 2f; 
    [SerializeField] private float playerMass = 1f;

    [SerializeField] private AudioClip thrusterClip;

    private AudioSource thrusterSource;
    private Animator anim; 
    private TetherManager tetherManager;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        tetherManager = FindFirstObjectByType<TetherManager>();
        SetupPhysics();

        thrusterSource = gameObject.AddComponent<AudioSource>();
        thrusterSource.outputAudioMixerGroup = AudioManager.Instance.GetAmbienceGroup();
        thrusterSource.clip = thrusterClip;
        thrusterSource.loop = true;
        thrusterSource.playOnAwake = false;
        thrusterSource.spatialBlend = 0;
    }
    private void Update()
    {
        if (Time.timeScale == 0 && thrusterSource != null && thrusterSource.isPlaying)
        {
            thrusterSource.Stop();
        }
    }
    private void FixedUpdate()
    {
        HandleMovement();
        HandleSteering();
        ClampDetachedSpeed();

    }
    private void ClampDetachedSpeed()
    {
        if (tetherManager != null && !tetherManager.IsConnected())
        {
            if (rb.linearVelocity.magnitude > maxDetachedSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * maxDetachedSpeed;
        }
    }
    private void SetupPhysics()
    {
        rb.mass = playerMass;
        rb.linearDamping = linearDamping;
        rb.angularDamping = angularDamping; 
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
        if (moveInput.sqrMagnitude > 0.01f)
        {
            if (!thrusterSource.isPlaying) thrusterSource.Play();
            if (anim != null) anim.SetBool("isMoving", true);
        }
        else
        {
            if (thrusterSource.isPlaying) thrusterSource.Stop();
            if (anim != null) anim.SetBool("isMoving", false);
        }
    }
    private void HandleSteering()
    {
        if (Mathf.Abs(moveInput.x) > 0.1f)
        {
            rb.AddTorque(-moveInput.x * steerTorque);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Core"))
        {
            if(tetherManager!=null && !tetherManager.IsConnected())
            {
                tetherManager.Reconnect();
                GameManager.Instance.EndRecovery();
            }

        }
    }

}
