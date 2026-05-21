using UnityEngine;
using System.Collections.Generic;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance;

    [Header("Tower Tracking")]
    [SerializeField] private List<EnergyTower> allTowers = new List<EnergyTower>();

    private int activatedTowersCount = 0;
    private bool allTowersPowered = false;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        allTowers.Clear();
        EnergyTower[] foundTowers = FindObjectsByType<EnergyTower>(FindObjectsSortMode.None);
        allTowers.AddRange(foundTowers);
    }

    private void Start()
    {
        

        Debug.Log($"TowerManager: Found {allTowers.Count} towers in the level.");
    }
    public void TowerActivated(EnergyTower tower)
    {
        activatedTowersCount++;
        Debug.Log($"Tower Activated! Progress: {activatedTowersCount}/{allTowers.Count}");

        if (activatedTowersCount >= allTowers.Count)
        {
            allTowersPowered = true;
            OnAllTowersPowered();
        }
        FindFirstObjectByType<HUDManager>().UpdateTowerCount();
    }

    private void OnAllTowersPowered()
    {
        Debug.Log("ALL TOWERS POWERED! The Base is now open.");

        BaseController baseController = FindFirstObjectByType<BaseController>();
        if (baseController != null)
        {
            baseController.OpenBase();
        }
    }

    public bool AreAllTowersPowered()
    {
        return allTowersPowered;
    }

    public int GetProgress()
    {
        return activatedTowersCount;
    }

    public int GetTotalTowers()
    {
        return allTowers.Count;
    }
}