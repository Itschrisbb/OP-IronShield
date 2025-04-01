using System.Collections;
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
    public GameObject hitmarker;

    [Header("Player Stats")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Hitmarker Settings")]
    public float hitmarkerDuration = 0.1f;
    private Coroutine hitmarkerRoutine;

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

        if (hitmarker != null)
            hitmarker.SetActive(false);
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

    public void ShowHitmarker()
    {
        if (hitmarkerRoutine != null)
            StopCoroutine(hitmarkerRoutine);

        hitmarkerRoutine = StartCoroutine(HitmarkerFlash());
    }

    private IEnumerator HitmarkerFlash()
    {
        hitmarker.SetActive(true);
        yield return new WaitForSeconds(hitmarkerDuration);
        hitmarker.SetActive(false);
    }
}
