using UnityEngine;
[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Volt-Link/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Visuals")]
    public string tierName;
    public Color tierColor = Color.white;

    [Header("Stats")]
    public float movementSpeed = 2f;
    public float damageToCore = 10f;

    [Header("Burner Settings")]
    [Tooltip("How many seconds the rope must be touching the enemy to kill it")]
    public float burnTimeRequired = 0.5f;

}
