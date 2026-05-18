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
    }
}
