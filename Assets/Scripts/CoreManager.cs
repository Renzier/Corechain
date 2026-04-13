using UnityEngine;
using UnityEngine.Events;

public class CoreManager : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Visuals")]
    public Material coreMaterial; // Assign a Tron blue material in inspector
    public Light coreLight;

    public UnityEvent<float> OnHealthChanged;
    public UnityEvent OnGameOver;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateVisuals();
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        UpdateVisuals();
        OnHealthChanged?.Invoke(currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    private void UpdateVisuals()
    {
        float healthPct = currentHealth / maxHealth;

        if (coreMaterial != null)
        {
            // Update emission intensity based on health
            Color baseColor = Color.cyan;
            coreMaterial.SetColor("_EmissionColor", baseColor * healthPct * 2f);
        }

        if (coreLight != null)
        {
            coreLight.intensity = healthPct * 2f;
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over! Core Destroyed.");
        OnGameOver?.Invoke();
    }
}
