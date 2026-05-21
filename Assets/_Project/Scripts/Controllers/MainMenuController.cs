using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject levelSelectorPanel;
    [SerializeField] private GameObject optionsPanel;
    private void Start()
    {
        CloseOption();
        CloseLevelSelector();
    }
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.deleteKey.wasPressedThisFrame)
        {
            ResetProgress();
        }
    }

    private void ResetProgress()
    {
        Debug.Log("Deleted");
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Level_01");
    }

    public void OpenOptions()
    {
        //mainPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }
    public void CloseOption()
    {
        //mainPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public void OpenLevelSelector()
    {
        //mainPanel.SetActive(false);
        levelSelectorPanel.SetActive(true);
    }
    public void CloseLevelSelector()
    {
        //mainPanel.SetActive(true);
        levelSelectorPanel.SetActive(false);
    }
}
