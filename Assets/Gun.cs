using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Gun Settings")]
    public float damage = 25f;
    public float range = 100f;
    public float fireRate = 10f;
    public float impactForce = 30f;

    [Header("Ammo")]
    public int maxAmmo = 30;
    public int currentAmmo;
    public int reserveAmmo = 90;
    public float reloadTime = 2f;
    private bool isReloading = false;

    [Header("References")]
    public Camera fpsCam;
    public Transform muzzlePoint;
    public GameObject muzzleFlash;
    public GameObject impactEffect;

    private float nextTimeToFire = 0f;
    private PlayerUI ui;

    void Start()
    {
        currentAmmo = maxAmmo;
        ui = FindObjectOfType<PlayerUI>();
    }

    void Update()
    {
        if (isReloading) return;

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && reserveAmmo > 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            if (currentAmmo > 0)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
            else
            {
                Debug.Log("Click! No ammo.");
            }
        }
    }

    void Shoot()
{
    currentAmmo--;

    if (muzzleFlash != null)
    {
        muzzleFlash.SetActive(true);
        Invoke(nameof(DisableMuzzleFlash), 0.05f);
    }

    RaycastHit hit;
    if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
    {
        Debug.Log("Hit: " + hit.transform.name);

        EnemyAI enemy = hit.transform.GetComponentInParent<EnemyAI>();
        if (enemy != null)
        {
            enemy.TakeDamage((int)damage);
            if (ui != null) ui.ShowHitmarker();
        }

        Target target = hit.transform.GetComponent<Target>();
        if (target != null)
        {
            target.TakeDamage(damage);
            if (ui != null) ui.ShowHitmarker();
        }

        if (hit.rigidbody != null)
        {
            hit.rigidbody.AddForce(-hit.normal * impactForce);
        }

        if (impactEffect != null)
        {
            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 1f);
        }
    }

    Debug.Log("Ammo: " + currentAmmo + "/" + reserveAmmo);
}


    void DisableMuzzleFlash()
    {
        if (muzzleFlash != null)
            muzzleFlash.SetActive(false);
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        if (ui != null) ui.ShowReloading();

        yield return new WaitForSeconds(reloadTime);

        int bulletsNeeded = maxAmmo - currentAmmo;
        int bulletsToReload = Mathf.Min(bulletsNeeded, reserveAmmo);

        currentAmmo += bulletsToReload;
        reserveAmmo -= bulletsToReload;

        isReloading = false;

        if (ui != null) ui.HideReloading();

        Debug.Log("Reloaded: " + currentAmmo + "/" + reserveAmmo);
    }
}
