using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private int levelIndex;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Button buttonComponent;

    private void Start()
    {
        if (buttonText !=null) buttonText.text = $"Level {levelIndex}";
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        if (levelIndex > unlockedLevel)
        {
            buttonComponent.interactable = false;
            if (buttonText != null) buttonText.color = Color.gray;
        }
    }
    public void LoadLevel()
    {
        SceneManager.LoadScene(levelIndex);
    }
}
