using System;
using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class EnemyBase : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] protected EnemyData data;
    [Header("Alert Settings")]
    [SerializeField] protected float alertRange = 10f;    
    [Header("Audio Settings")]
    [SerializeField] private AudioClip searingLoopClip;
    [SerializeField] private AudioClip deathClip;

    protected Vector2 initialPosition;
    protected Animator anim;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected CoreController core;

    private AudioSource searingSource;
    private float currentBurnTime = 0f;
    private int burningSegmentsCount = 0;
    private Collider2D col;
    private bool isDying = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        core = FindFirstObjectByType<CoreController>();
        anim = GetComponent<Animator>();

        searingSource = gameObject.AddComponent<AudioSource>();
        searingSource.outputAudioMixerGroup = AudioManager.Instance.GetAmbienceGroup();
        searingSource.clip = searingLoopClip;
        searingSource.loop = true;
        searingSource.playOnAwake = false;
        searingSource.spatialBlend = 1.0f;

        initialPosition = transform.position;

        if (data != null)
        {
            sr.color = data.tierColor;
        }

        SetupPhysics();
    }

    private void Update()
    {
        HandleBurning();
    }

    private void HandleBurning()
    {
        if (burningSegmentsCount > 0)
        {
            if (!searingSource.isPlaying) searingSource.Play();
            currentBurnTime += Time.deltaTime;

            float burnPercentage = currentBurnTime / data.burnTimeRequired;
            sr.color = Color.Lerp(data.tierColor, Color.grey, burnPercentage);
            if (currentBurnTime >= data.burnTimeRequired)
            {
                Die();
            }
        }
        else
        {
            if (searingSource.isPlaying) searingSource.Stop();
            currentBurnTime -= Time.deltaTime * 0.2f;
		    if(currentBurnTime <= 0){ currentBurnTime = 0; }
        }
    }
    

    private void SetupPhysics()
    {
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

    }
    protected virtual void Die()
    {
        if (isDying) return;
        isDying = true;
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        if (col != null) col.enabled = false;
        if (rb != null){
            rb.linearVelocity = Vector2.zero;   
            rb.angularVelocity = 0f;  
            rb.simulated = false;
        }

        if (AudioManager.Instance != null && deathClip != null)
        {
            AudioManager.Instance.PlaySFX(deathClip);
        }

        PerformDeathVisuals();

        

        if (anim != null)
        {
            yield return null;

            yield return new WaitUntil(() =>
                anim.GetCurrentAnimatorStateInfo(0).IsTag("Death"));
            yield return new WaitUntil(() =>
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f &&
                !anim.IsInTransition(0));
        }
        else
        {   
            yield return new WaitForSeconds(0.5f);
        }

        Destroy(gameObject);
    }
    protected virtual void PerformDeathVisuals()
    {
        if (anim != null) anim.SetTrigger("Die");
    }
    public void AddBurningContact()
    {
        burningSegmentsCount++;
    }

    public void RemoveBurningContact()
    {
        burningSegmentsCount--;
        if (burningSegmentsCount < 0) burningSegmentsCount = 0;
    }
}
