using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseController : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private Color closedColor = Color.red;
    [SerializeField] private Color openColor = Color.green;

    private bool isOpen = false;
    private SpriteRenderer sr;
    private Collider2D baseCollider;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseCollider = GetComponent<Collider2D>();

        if (sr != null) sr.color = closedColor;

        if (baseCollider != null)
        {
            baseCollider.isTrigger = false;
        }
    }

    public void OpenBase()
    {
        isOpen = true;

        if (sr != null) sr.color = openColor;

        if (baseCollider != null)
        {
            baseCollider.isTrigger = true;
        }

        Debug.Log("The Base is now OPEN!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpen && other.CompareTag("Core"))
        {
            Victory();
        }
    }

    private void Victory()
    {
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (currentLevelIndex == unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", unlockedLevel + 1);
            PlayerPrefs.Save();
            Debug.Log($"Level {unlockedLevel + 1} Unlocked!");
        }


        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerVictory();
        }
    }
}
