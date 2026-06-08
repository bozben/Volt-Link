using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private CoreController targetCore;

    [Header("Shooting Settings")]
    [SerializeField] private float fireRate = 1.5f;
    [SerializeField] private float attackRange = 8f;
    [SerializeField] private LayerMask blockageLayer;
    [SerializeField] private AudioClip shotClip;

    private float nextFireTime;

    private void Awake()
    {
        if (targetCore == null)
        {
            targetCore = FindFirstObjectByType<CoreController>();
        }
    }

    public bool TryShoot()
    {
        if (targetCore == null) return false;

        float distance = Vector2.Distance(transform.position, targetCore.transform.position);
        if (distance > attackRange) return false;

        if (Time.time < nextFireTime) return false;

        if (!HasLineOfSight()) return false;

        Fire();
        return true;
    }

    public bool HasLineOfSight()
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, targetCore.transform.position, blockageLayer);

        return hit.collider == null;
    }

    private void Fire()
    {
        nextFireTime = Time.time + fireRate;

        Vector2 fireDirection = (targetCore.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg - 90f;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        Instantiate(projectilePrefab, transform.position, rotation);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(shotClip);
        }

        Debug.Log($"{gameObject.name} fired a projectile!");
    }

}
