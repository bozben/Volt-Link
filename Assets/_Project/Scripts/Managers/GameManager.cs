using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Recovery Settings")]
    [SerializeField] private float recoveryTime = 5f;
    [SerializeField] private TextMeshProUGUI timerText;

    private float currentTimer;
    private bool isRecovering = false;

    private void Awake()
    {
        //singleton
        Instance = this;
        if (timerText != null) timerText.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (!isRecovering) return;

        currentTimer -= Time.deltaTime;

        if (timerText != null)
            timerText.text = $"GO BACK TO THE CORE!\n{currentTimer:F1}s";

        if (currentTimer <= 0)
        {
            GameOver();
        }
    }


    public void StartRecovery()
    {
        isRecovering = true;
        currentTimer = recoveryTime;
        if (timerText != null) timerText.gameObject.SetActive(true);
    }

    public void EndRecovery()
    {
        isRecovering = false;
        if (timerText != null) timerText.gameObject.SetActive(false);
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER: Out of power!");
        //TODO: UI screen
        Time.timeScale = 0; // Freeze game
    }
}
