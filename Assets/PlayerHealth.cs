using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // Maximum amount of health the player can have
    public int maxHealth = 100;

    // Current health value of the player (changes over time)
    public int currentHealth;

    // Reference to the PlayerUI script (updates health bar visuals)
    private PlayerUI playerUI;

    void Start()
    {
        // Set player starting health to max at game start
        currentHealth = maxHealth;

        // Attempt to grab PlayerUI component attached to the same object(So they can talk to eachother)
        playerUI = GetComponent<PlayerUI>();

        // If player is found, initialize UI health bar
        if (playerUI != null)
        {
            playerUI.SetMaxHealth(maxHealth);
        }
        else
        {
            // Debug to check for missing references
            Debug.LogWarning("PlayerUI not found on the Player object.");
        }
    }

    // Called when the player takes damage from enemies
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        // making sure health stays between 0 and maxHealth
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update the health bar UI to reflect current health
        if (playerUI != null)
        {
            playerUI.UpdateHealth(currentHealth);
        }

        // If health reaches 0, player dies
        if (currentHealth <= 0)
        {
            Debug.Log("Player died!");

            // Notify the GameManager to handle player dying
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
                gm.PlayerDied();
        }
    }

    // Called to heal the player (from planned pickups)
    public void Heal(int amount)
    {
        currentHealth += amount;

        // Ensure health stays within limits
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update health bar UI
        if (playerUI != null)
        {
            playerUI.UpdateHealth(currentHealth);
        }
    }
}
