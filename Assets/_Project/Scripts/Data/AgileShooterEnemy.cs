using System;
using System.Collections;
using UnityEngine;

public class AgileShooterEnemy : EnemyBase
{
    private enum State { IDLE, CHASE, SEARCHING, COMBAT, PANIC, TIRED, RESET }

    [Header("AI State")]
    [SerializeField] private State currentState = State.IDLE;

    [Header("Movement Settings")]
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float panicJumpForce = 15f;
    [SerializeField] private float orbitForce = 10f;
    [SerializeField] private float resetPushForce = 20f;

    [Header("Range Settings")]
    [SerializeField] private float combatRange = 6f;
    [SerializeField] private float panicThreshold = 4f;
    [SerializeField] private float calmThreshold = 5.5f;

    [Header("Tired Settings")]
    [SerializeField] private float tiredDuration = 10f;
    [SerializeField] private float flashMaxInterval = 0.8f;
    [SerializeField] private float flashMinInterval = 0.05f;
    private int panicCount = 0;
    [Header("Trail Settings")]
    [SerializeField] private GameObject trailPrefab;
    [Header("Blast Settings")]
    [SerializeField] private float blastRadius = 5f;
    [SerializeField] private float pulseDuration = 0.4f;
    [SerializeField] private GameObject ringPulsePrefab;

    [Header("Navigation Settings")]
    [SerializeField] private float avoidanceForce = 10f;
    [SerializeField] private float detectionDistance = 1.5f;
    [SerializeField] private float sideRayAngle = 30f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Sound Settings")]
    [SerializeField] private AudioClip panicDashClip;
    [SerializeField] private AudioClip tiredClip;
    [SerializeField] private AudioClip pushClip;

    [SerializeField] private Rigidbody2D playerRb;

    private EnemyShooter shooter;

    protected override void Awake()
    {
        base.Awake();
        shooter = GetComponent<EnemyShooter>();
        rb.mass = 1f;
        rb.linearDamping = 2f;
    }

    private void FixedUpdate()
    {
        if (core == null || playerRb == null) return;
        UpdateState();
        ExecuteStateBehavior();
    }

    private void UpdateState()
    {
        float distToPlayer = Vector2.Distance(rb.position, playerRb.position);
        float distToCore = Vector2.Distance(rb.position, core.transform.position);

        if (currentState != State.PANIC && currentState != State.TIRED && currentState != State.RESET)
        {
            if (distToPlayer < panicThreshold)
            {
                TransitionToState(State.PANIC);
                return;
            }
        }

        if (currentState == State.PANIC && distToPlayer > calmThreshold)
        {
            TransitionToState(State.CHASE);
            return;
        }

        if (currentState == State.CHASE || currentState == State.SEARCHING ||
            currentState == State.COMBAT || currentState == State.IDLE)
        {
            if (distToCore > alertRange)
                currentState = State.IDLE;
            else if (distToCore <= combatRange)
            {
                if (shooter != null && shooter.HasLineOfSight())
                    currentState = State.COMBAT;
                else
                    currentState = State.SEARCHING;
            }
            else
                currentState = State.CHASE;
        }
    }

    private void ExecuteStateBehavior()
    {
        Vector2 finalForce = Vector2.zero;

        switch (currentState)
        {
            case State.IDLE:
                float distToHome = Vector2.Distance(rb.position, initialPosition);
                if (distToHome > 0.2f)
                {
                    Vector2 returnDir = (initialPosition - (Vector2)transform.position).normalized;
                    finalForce = returnDir * data.movementSpeed * 2f;
                    RotateTowards(returnDir);
                }
                else rb.linearVelocity = Vector2.zero;
                break;

            case State.CHASE:
                Vector2 chaseDir = ((Vector2)core.transform.position - rb.position).normalized;
                finalForce = chaseDir * data.movementSpeed * acceleration;
                RotateTowards(chaseDir);
                break;

            case State.SEARCHING:
                Vector2 searchDir = ((Vector2)core.transform.position - rb.position).normalized;
                Vector2 orbitDir = new Vector2(-searchDir.y, searchDir.x);
                finalForce = orbitDir * orbitForce;
                RotateTowards(searchDir);
                break;

            case State.COMBAT:
                finalForce = Vector2.zero;
                Vector2 shootDir = ((Vector2)core.transform.position - rb.position).normalized;
                RotateTowards(shootDir);
                if (shooter != null) shooter.TryShoot();
                break;

            case State.PANIC:
                Vector2 awayFromPlayer = (rb.position - playerRb.position).normalized;
                RotateTowards(awayFromPlayer);
                break;

            case State.TIRED:
            case State.RESET:
                finalForce = Vector2.zero;
                break;
        }

        finalForce += CalculateAvoidanceForce();
        rb.AddForce(finalForce);
    }

    private void TransitionToState(State newState)
    {
        currentState = newState;

        if (newState == State.PANIC)
        {
            AudioManager.Instance.PlaySFX(panicDashClip);
            Vector2 jumpDir = (rb.position - playerRb.position).normalized;
            rb.AddForce(jumpDir * panicJumpForce, ForceMode2D.Impulse);
            if (trailPrefab != null)
                Instantiate(trailPrefab, transform.position, Quaternion.identity);

            panicCount++;
            if (panicCount >= 3)
                StartCoroutine(TiredCourutine());
        }
    }

    private IEnumerator TiredCourutine()
    {
        currentState = State.TIRED;
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(tiredClip);

        float halfTime = tiredDuration / 2f;
        Color originalColor = data.tierColor;
        Color grayColor = Color.gray;

        // First half: smoothly gray out
        float elapsed = 0f;
        while (elapsed < halfTime)
        {
            elapsed += Time.deltaTime;
            sr.color = Color.Lerp(originalColor, grayColor, elapsed / halfTime);
            yield return null;
        }

        // Second half: flash with increasing frequency
        elapsed = 0f;
        bool showOriginal = false;
        float nextFlashAt = 0f;

        while (elapsed < halfTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfTime;
            float interval = Mathf.Lerp(flashMaxInterval, flashMinInterval, t * t);

            if (elapsed >= nextFlashAt)
            {
                showOriginal = !showOriginal;
                sr.color = showOriginal ? originalColor : grayColor;
                nextFlashAt = elapsed + interval;
            }

            yield return null;
        }

        // Restore color before reset
        sr.color = originalColor;

        currentState = State.RESET;
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(pushClip);

        Vector2 pushDir = (playerRb.position - rb.position).normalized;
        if (ringPulsePrefab != null)
        {
            GameObject ring = Instantiate(ringPulsePrefab, transform.position, Quaternion.identity);
            ring.GetComponent<RingPulse>().Play(blastRadius, pulseDuration);
        }

        if (Vector2.Distance(rb.position, playerRb.position) <= blastRadius)
            playerRb.AddForce(pushDir * resetPushForce, ForceMode2D.Impulse);

        if (core != null)
        {
            Rigidbody2D coreRb = core.GetComponent<Rigidbody2D>();
            if (coreRb != null && Vector2.Distance(rb.position, core.transform.position) <= blastRadius)
                coreRb.AddForce(((Vector2)core.transform.position - rb.position).normalized * resetPushForce, ForceMode2D.Impulse);
        }

        rb.AddForce(-pushDir * resetPushForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);
        panicCount = 0;
        currentState = State.CHASE;
    }

    private void RotateTowards(Vector2 direction)
    {
        float angle = MathF.Atan2(direction.x, direction.y) * Mathf.Rad2Deg - 90f;
        rb.MoveRotation(angle);
    }

    private Vector2 CalculateAvoidanceForce()
    {
        Vector2 currentDir = rb.linearVelocity.normalized;
        if (currentDir.sqrMagnitude < 0.01f)
            currentDir = ((Vector2)core.transform.position - rb.position).normalized;

        Vector2 totalAvoidForce = Vector2.zero;
        Vector2[] rayDirections =
        {
            currentDir,
            Quaternion.Euler(0, 0, sideRayAngle) * currentDir,
            Quaternion.Euler(0, 0, -sideRayAngle) * currentDir
        };

        foreach (Vector2 dir in rayDirections)
        {
            RaycastHit2D hit = Physics2D.Raycast(rb.position, dir, detectionDistance, obstacleLayer);
            if (hit.collider != null)
            {
                float distanceMultiplier = 1f - (hit.distance / detectionDistance);
                totalAvoidForce += hit.normal * avoidanceForce * distanceMultiplier;
            }
        }

        return totalAvoidForce;
    }
}