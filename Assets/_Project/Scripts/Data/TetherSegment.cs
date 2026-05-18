using UnityEngine;

public class TetherSegment : MonoBehaviour
{
    private TetherManager manager;

    public void Initialize(TetherManager mgr)
    {
        manager = mgr;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            manager.SetWallContact(true);
        }

        EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.AddBurningContact();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            manager.SetWallContact(false);
        }

        EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.RemoveBurningContact();
        }
    }
}
