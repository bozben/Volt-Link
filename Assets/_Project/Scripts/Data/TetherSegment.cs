using UnityEngine;

public class TetherSegment : MonoBehaviour
{
    private TetherManager manager;

    public void Initialize(TetherManager mgr)
    {
        manager = mgr;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            manager.SetWallContact(true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            manager.SetWallContact(false);
        }
    }
}
