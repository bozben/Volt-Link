using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraEnemy : EnemyBase
{
    [Header("Aura Settings")]
    [SerializeField] private float slowAmount = 5f;
    [SerializeField] private float auraRadius = 4f;
    [SerializeField] private AudioClip energyHumClip;
    private AudioSource humSource;
    [SerializeField] private SpriteRenderer auraVisual;
    [SerializeField] private float auraVisualNativeRadius = 1f;
    [SerializeField] private float growDuration = 1f;
    private Coroutine auraCoroutine;
    private bool playerDetected = false;
    private bool isShrinking = false;

    private List<Rigidbody2D> affectedBodies = new List<Rigidbody2D>();

    protected override void Awake()
    {
        base.Awake();
        rb.mass = 1f;
        rb.linearDamping = 1f;

        humSource = gameObject.AddComponent<AudioSource>();
        humSource.outputAudioMixerGroup = AudioManager.Instance.GetAmbienceLoudGroup();
        humSource.clip = energyHumClip;
        humSource.loop = true;
        humSource.spatialBlend = 1.0f;
        humSource.Play();

        CircleCollider2D auraCollider = gameObject.AddComponent<CircleCollider2D>();
        auraCollider.radius = auraRadius;
        auraCollider.isTrigger = true;

        if (auraVisual != null) auraVisual.transform.localScale = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (core == null) return;
        Patrol();
    }
    protected override void PerformDeathVisuals()
    {
        base.PerformDeathVisuals();
        if (humSource != null) humSource.Stop();
    }

    private void Patrol()
    {
        float distToCore = Vector2.Distance(rb.position, core.transform.position);

        if (distToCore <= alertRange)
        {
            Vector2 direction = ((Vector2)core.transform.position - rb.position).normalized;
            rb.AddForce(direction * data.movementSpeed * 2f);
        }
        else
        {
            if (!playerDetected && auraVisual != null && auraVisual.transform.localScale.x > 0f && !isShrinking)
            {
                if (auraCoroutine != null) StopCoroutine(auraCoroutine);
                isShrinking = true;
                auraCoroutine = StartCoroutine(ShrinkAura());
            }
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
    private IEnumerator GrowAura()
    {
        isShrinking = false;
        float targetScale = auraRadius / auraVisualNativeRadius;
        float elapsed = 0f;
        float startScale = auraVisual.transform.localScale.x;

        while (elapsed < growDuration)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(startScale, targetScale, elapsed / growDuration);
            auraVisual.transform.localScale = Vector3.one * scale;
            yield return null;
        }
        auraVisual.transform.localScale = Vector3.one * targetScale;
    }

    private IEnumerator ShrinkAura()
    {
        float elapsed = 0f;
        float startScale = auraVisual.transform.localScale.x;

        while (elapsed < growDuration)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(startScale, 0f, elapsed / growDuration);
            auraVisual.transform.localScale = Vector3.one * scale;
            yield return null;
        }
        auraVisual.transform.localScale = Vector3.zero;
        isShrinking = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D targetRb = other.GetComponent<Rigidbody2D>();

        if (targetRb != null && (other.CompareTag("Player") || other.CompareTag("Core")))
        {
            targetRb.linearDamping += slowAmount;
            affectedBodies.Add(targetRb);

            if (other.CompareTag("Player") && auraVisual != null)
            {
                playerDetected = true;
                if (auraCoroutine != null) StopCoroutine(auraCoroutine);
                auraCoroutine = StartCoroutine(GrowAura());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Rigidbody2D targetRb = other.GetComponent<Rigidbody2D>();

        if (targetRb != null && affectedBodies.Contains(targetRb))
        {
            targetRb.linearDamping -= slowAmount;
            affectedBodies.Remove(targetRb);

            if (other.CompareTag("Player") && auraVisual != null)
            {
                playerDetected = false;
            }
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