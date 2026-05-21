using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float damage = 10f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        rb.linearVelocity = transform.up * speed;

        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Core"))
        {
            CoreHealth coreHealth = collision.gameObject.GetComponent<CoreHealth>();
            if (coreHealth != null)
            {
                coreHealth.TakeDamage(damage);
            }

            Rigidbody2D coreRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (coreRb != null)
            {
                Vector2 pushDir = rb.linearVelocity.normalized;
                coreRb.AddForce(pushDir * knockbackForce, ForceMode2D.Impulse);

                Debug.Log("KNOCKBACK");
            }
        }

        Destroy(gameObject);
    }
}
