using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Recovery Settings")]
    [SerializeField] private float recoveryTime = 5f;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("UI Panels")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject gameOverPanel;

    private float currentTimer;
    private bool isRecovering = false;

    private HUDManager hud;

    private void Awake()
    {
        //singleton
	if(Instance == null)
	{
		Instance = this;
	}else
	{
		Destroy(gameObject);
    }

        hud = FindFirstObjectByType<HUDManager>();

        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        
    }
    private void Update()
    {
        if (!isRecovering) return;

        currentTimer -= Time.deltaTime;

        if (hud != null)
        {
            string timerStr = $"GO BACK TO THE CORE!\n{currentTimer:F1}s";
            hud.UpdateTimer(timerStr);
        }
        if (currentTimer <= 0) TriggerGameOver();
    }


    public void StartRecovery()
    {
        isRecovering = true;
        currentTimer = recoveryTime;
    }

    public void EndRecovery()
    {
        isRecovering = false;
        if (hud != null)
        {
            hud.HideTimer();
        }
    }

    public void TriggerVictory()
    {
        Debug.Log("Game State: Victory");
        Time.timeScale = 0;
        if (victoryPanel != null) victoryPanel.SetActive(true);
    }

    public void TriggerGameOver()
    {
        Debug.Log("Game State: Game Over");
        Time.timeScale = 0;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("All levels done");
        }
    }
}
