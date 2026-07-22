using UnityEngine;

public class HealthHUDController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private HealthUnitUI[] healthUnits;

    private void OnEnable()
    {
        if (playerHealth == null)
        {
            Debug.LogError(
                "HealthHUDController: PlayerHealth is not assigned.",
                this
            );

            return;
        }

        playerHealth.HealthChanged += UpdateHUD;
    }

    private void Start()
    {
        RefreshHUD();
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthChanged -= UpdateHUD;
        }
    }

    private void RefreshHUD()
    {
        if (playerHealth == null)
        {
            return;
        }

        UpdateHUD(
            playerHealth.CurrentHealth,
            playerHealth.MaxHealth
        );
    }

    private void UpdateHUD(
        int currentHealth,
        int maxHealth)
    {
        Debug.Log(
            $"HUD UPDATE | HP: {currentHealth}/{maxHealth}"
        );

        for (int i = 0; i < healthUnits.Length; i++)
        {
            if (healthUnits[i] == null)
            {
                continue;
            }

            bool isFull = i < currentHealth;

            healthUnits[i].SetFull(isFull);
        }
    }
}