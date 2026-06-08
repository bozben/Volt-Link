using UnityEngine;

public class KamikazeEnemy : EnemyBase
{
    [Header("Kamikaze Settings")]
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float explosionDamage = 20f;
    [SerializeField] private AudioClip sirenClip;
    [SerializeField] private AudioClip explosionClip;
    private AudioSource sirenSource;

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

        sirenSource = gameObject.AddComponent<AudioSource>();
        sirenSource.outputAudioMixerGroup = AudioManager.Instance.GetAmbienceGroup(); ;
        sirenSource.clip = sirenClip;
        sirenSource.loop = true;
        sirenSource.spatialBlend = 1.0f;
        sirenSource.Play();
    }

    private void FixedUpdate()
    {
        if (core == null) return;
        Patrol();

        
    }
    protected override void PerformDeathVisuals()
    {
        base.PerformDeathVisuals();
        StopSiren();
    }
    private void RotateTowardsCore()
    {
        Vector2 direction = ((Vector2)core.transform.position - rb.position).normalized;
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg - 90f;
        rb.MoveRotation(angle);
    }

    private void Patrol()
    {
        float distToCore = Vector2.Distance(rb.position, core.transform.position);

        if (distToCore <= alertRange)
        {
            Vector2 chaseForce = CalculateChaseForce();
            Vector2 avoidForce = CalculateAvoidanceForce();
            rb.AddForce(chaseForce + avoidForce);
            RotateTowardsCore();
            UpdateSirenVolume(distToCore);
        }
        else
        {
            StopSiren();
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
    private void UpdateSirenVolume(float distance)
    {
        float volume = 1f - (distance / alertRange);
        sirenSource.volume = Mathf.Clamp01(volume);

        if (!sirenSource.isPlaying) sirenSource.Play();
    }

    private void StopSiren()
    {
        if (sirenSource.isPlaying)
        {
            sirenSource.Stop();
        }
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
        CoreHealth coreHealth = core.GetComponent<CoreHealth>();
        if (coreHealth != null)
        {
            coreHealth.TakeDamage(explosionDamage);
        }
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(explosionClip);
        Die();
    }
}
