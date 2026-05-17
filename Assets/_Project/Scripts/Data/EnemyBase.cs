using System;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class EnemyBase : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] protected EnemyData data;

    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected CoreController core;

    private float currentBurnTime = 0f;
    private bool isCurrentlyBurning = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        core = FindFirstObjectByType<CoreController>();

        // Apply the color from the ScriptableObject
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
        if (isCurrentlyBurning)
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
        // We will set mass and damping in the specific enemy types
    }
    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} burned to a crisp!");
        Destroy(gameObject);
    }
    public void SetBurning(bool burning)
    {
        isCurrentlyBurning = burning;
    }
}
