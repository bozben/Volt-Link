using System;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class EnemyBase : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] protected EnemyData data;

    [Header("Alert Settings")]
    [SerializeField] protected float alertRange = 10f;
    protected Vector2 initialPosition;

    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected CoreController core;

    private float currentBurnTime = 0f;
    private int burningSegmentsCount = 0;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        core = FindFirstObjectByType<CoreController>();

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
            currentBurnTime -= Time.deltaTime * 0.2f;
        }
    }

    private void SetupPhysics()
    {
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

    }
    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} burned to a crisp!");
        Destroy(gameObject);
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
