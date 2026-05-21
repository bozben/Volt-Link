using UnityEngine;

public class EnergySensor : MonoBehaviour
{
    public EnergyTower tower;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<TetherSegment>() != null) tower.AddContact();
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<TetherSegment>() != null) tower.RemoveContact();
    }
}
