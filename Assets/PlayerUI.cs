using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI ammoText;
    public Slider healthBar;
    public TextMeshProUGUI reloadText;
    public GameObject crosshair;

    [Header("Player Stats")]
    public int maxHealth = 100;
    private int currentHealth;

    private Gun gun;

    void Start()
    {
        gun = FindObjectOfType<Gun>();
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (reloadText != null)
            reloadText.gameObject.SetActive(false);

        if (crosshair != null)
            crosshair.SetActive(true);
    }

    void Update()
    {
        if (gun != null && ammoText != null)
        {
            ammoText.text = $"{gun.currentAmmo} / {gun.reserveAmmo}";
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
            healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Debug.Log("Player Died!");
            // TODO: death handling
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
            healthBar.value = currentHealth;
    }

    public void ShowReloading()
    {
        if (reloadText != null)
            reloadText.gameObject.SetActive(true);

        if (crosshair != null)
            crosshair.SetActive(false);
    }

    public void HideReloading()
    {
        if (reloadText != null)
            reloadText.gameObject.SetActive(false);

        if (crosshair != null)
            crosshair.SetActive(true);
    }
}
