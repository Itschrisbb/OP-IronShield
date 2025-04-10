using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    private PlayerUI playerUI;

    void Start()
    {
        currentHealth = maxHealth;

        //Get PlayerUI on the same GameObject
        playerUI = GetComponent<PlayerUI>();

        if (playerUI != null)
        {
            playerUI.SetMaxHealth(maxHealth);
        }
        else
        {
            Debug.LogWarning("PlayerUI not found on the Player object.");
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (playerUI != null)
        {
            playerUI.UpdateHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
             Debug.Log("Player died!");
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
            gm.PlayerDied();
            }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (playerUI != null)
        {
            playerUI.UpdateHealth(currentHealth);
        }
    }
}
