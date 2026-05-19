using UnityEngine;

public class KamikazeEnemy : EnemyBase
{
    [Header("Kamikaze Settings")]
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float explosionDamage = 20f;

    [Header("Navigation Settings")]
    [SerializeField] private float avoidanceForce = 10f;
    [SerializeField] private float detectionDistance = 1.5f;
    [SerializeField] private float sideRayAngle = 30f;


    [SerializeField] private LayerMask obstacleLayer;
    protected override void Awake()
    {
        base.Awake();

        rb.mass = 1f;
        rb.linearDamping = 1f;
    }

    private void FixedUpdate()
    {
        if (core == null) return;
        Patrol();

    }

    private void Patrol()
    {
        float distToCore = Vector2.Distance(rb.position, core.transform.position);

        if (distToCore <= alertRange)
        {
            Vector2 chaseForce = CalculateChaseForce();
            Vector2 avoidForce = CalculateAvoidanceForce();
            rb.AddForce(chaseForce + avoidForce);
        }
        else
        {
            float distToHome = Vector2.Distance(rb.position, initialPosition);

            if (distToHome > 0.2f)
            {
                Vector2 returnDir = (initialPosition - (Vector2)transform.position).normalized;
                rb.AddForce(returnDir * data.movementSpeed * 2f);
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    private Vector2 CalculateChaseForce()
    {
        Vector2 direction = ((Vector2)core.transform.position - rb.position).normalized;
        return direction * data.movementSpeed * acceleration;
    }

    private Vector2 CalculateAvoidanceForce()
    {
        Vector2 targetDir = ((Vector2)core.transform.position - rb.position).normalized;
        Vector2 totalAvoidForce = Vector2.zero;

        Vector2[] rayDirections = new Vector2[] // Obstacle detection rays
        {
            targetDir, // Center
            Quaternion.Euler(0, 0, sideRayAngle) * targetDir, // Left
            Quaternion.Euler(0, 0, -sideRayAngle) * targetDir // Right
        };
        foreach (Vector2 dir in rayDirections)
        {
            RaycastHit2D hit = Physics2D.Raycast(rb.position, dir, detectionDistance, obstacleLayer);

            Debug.DrawRay(rb.position, dir * detectionDistance, dir == targetDir ? Color.red : Color.blue);

            if (hit.collider != null)
            {
                float distanceMultiplier = 1f - (hit.distance / detectionDistance);

                Vector2 forceDir = hit.normal;

                totalAvoidForce += forceDir * avoidanceForce * distanceMultiplier;
            }
        }

        return totalAvoidForce;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Core"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        Debug.Log($"Kamikaze exploded! Dealt {explosionDamage} damage to core.");

        // TODO: implement the Core's HP system in the GameManager/CoreController .
        // For now, we trigger the logic here.

        Destroy(gameObject);
    }
}
