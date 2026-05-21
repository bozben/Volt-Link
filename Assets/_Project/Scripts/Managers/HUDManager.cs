using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("Health UI")]
    [SerializeField] private Image healthBarFill;

    [Header("Tower UI")]
    [SerializeField] private TextMeshProUGUI towerText;

    [Header("Timer UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    private CoreHealth coreHealth;
    private void Start()
    {
        coreHealth = FindFirstObjectByType<CoreHealth>();
        if(coreHealth != null)
        {
            coreHealth.OnHealthChanged.AddListener(UpdateHealthBar);
            UpdateHealthBar(coreHealth.GetHealthPercentage());
        }

        UpdateTowerCount();

        if (timerText != null) timerText.gameObject.SetActive(false);

    }

    public void UpdateHealthBar(float fillAmount)
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = fillAmount;
        }
    }

    public void UpdateTowerCount() 
    {
        if(TowerManager.Instance != null && towerText != null)
        {
            int current = TowerManager.Instance.GetProgress();
            int total = TowerManager.Instance.GetTotalTowers();
            towerText.text = $"Towers: {current} / {total}";
        }
    }

    public void UpdateTimer(string text)
    {
        if(timerText != null)
        {
            timerText.gameObject.SetActive(true);
            timerText.text = text;
        }
    }

    public void HideTimer()
    {
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }
    }

}
