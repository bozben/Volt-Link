using UnityEngine;
using System.Collections.Generic;

public class AuraEnemy : EnemyBase
{
    [Header("Aura Settings")]
    [SerializeField] private float slowAmount = 5f;
    [SerializeField] private float auraRadius = 4f;

    private List<Rigidbody2D> affectedBodies = new List<Rigidbody2D>();

    protected override void Awake()
    {
        base.Awake();
        rb.mass = 1f;
        rb.linearDamping = 1f;

        CircleCollider2D auraCollider = gameObject.AddComponent<CircleCollider2D>();
        auraCollider.radius = auraRadius;
        auraCollider.isTrigger = true;
    }

    private void FixedUpdate()
    {
        if (core == null) return;

        Vector2 direction = ((Vector2)core.transform.position - rb.position).normalized;
        rb.AddForce(direction * data.movementSpeed * 2f); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D targetRb = other.GetComponent<Rigidbody2D>();

        if (targetRb != null && (other.CompareTag("Player") || other.CompareTag("Core")))
        {
            targetRb.linearDamping += slowAmount;
            affectedBodies.Add(targetRb);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Rigidbody2D targetRb = other.GetComponent<Rigidbody2D>();

        if (targetRb != null && affectedBodies.Contains(targetRb))
        {
            targetRb.linearDamping -= slowAmount;
            affectedBodies.Remove(targetRb);
        }
    }

    private void OnDestroy()
    {
        foreach (var body in affectedBodies)
        {
            if (body != null)
            {
                body.linearDamping -= slowAmount;
            }
        }
    }
}