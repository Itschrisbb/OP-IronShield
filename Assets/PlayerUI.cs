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

    [Header("Hitmarker Settings")]
    public float hitmarkerDuration = 0.1f;
    private Coroutine hitmarkerRoutine;

    private Gun gun;

    void Start()
    {
        gun = FindObjectOfType<Gun>();

        if (healthBar != null)
        {
            healthBar.value = healthBar.maxValue;
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

    // ✅ Called from PlayerHealth.cs
    public void SetMaxHealth(int max)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = max;
            healthBar.value = max;
        }
    }

    public void UpdateHealth(int current)
    {
        if (healthBar != null)
        {
            healthBar.value = current;
        }
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

    private System.Collections.IEnumerator HitmarkerFlash()
    {
        if (hitmarker != null)
        {
            hitmarker.SetActive(true);
            yield return new WaitForSeconds(hitmarkerDuration);
            hitmarker.SetActive(false);
        }
    }
}
