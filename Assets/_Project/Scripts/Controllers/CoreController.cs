using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class CoreController : MonoBehaviour
{
    [Header("Core Settings")]
    [SerializeField] private float coreMass = 10f;
    [SerializeField] private float linearDamping = 1f;
    [SerializeField] private AudioClip wallImpactClip;


    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        SetupPhysics();
    }
    private void SetupPhysics()
    {
        rb.mass = coreMass;
        rb.linearDamping = linearDamping;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.gravityScale = 0f;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(wallImpactClip);
        }
    }
}
