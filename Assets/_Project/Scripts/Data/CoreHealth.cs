using UnityEngine;
using UnityEngine.Events;

public class CoreHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private AudioClip bulletImpactClip;

    public UnityEvent<float> OnHealthChanged;
    public UnityEvent OnCoreDestroyed;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log("Core took " + amount + " damage. Current health: " + currentHealth);
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(bulletImpactClip);
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateHealthUI();
        if (currentHealth <= 0)
        {
            Die();
        }

    }
    public void UpdateHealthUI()
    {
        float healthPercentage = currentHealth / maxHealth;
        OnHealthChanged?.Invoke(healthPercentage);
    }

    public void Die()
    {
        Debug.Log("Core destroyed");
        OnCoreDestroyed?.Invoke();

        if(GameManager.Instance != null)
        {
            GameManager.Instance.TriggerGameOver();
        }
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

}
