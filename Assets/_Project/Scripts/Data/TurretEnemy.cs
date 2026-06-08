using UnityEngine;

public class TurretEnemy : EnemyBase
{
    private EnemyShooter shooter;

    protected override void Awake()
    {
        base.Awake();
        shooter = GetComponent<EnemyShooter>();

        rb.bodyType = RigidbodyType2D.Static;
    }
    private void FixedUpdate()
    {
        if (shooter != null)
        {
            shooter.TryShoot();
        }
        if (core != null) RotateTowardsCore();

    }
    private void RotateTowardsCore()
    {
        Vector2 direction = ((Vector2)core.transform.position - rb.position).normalized;
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg +90;
        rb.MoveRotation(angle);
    }
}
