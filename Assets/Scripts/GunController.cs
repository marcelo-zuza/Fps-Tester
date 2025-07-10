using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GunController : MonoBehaviour
{
    [Header("Arsenal")]
    private bool gotKnife = true;
    private bool gotPistol = true;
    [SerializeField] private bool gotRifle = false;

    [Header("Is the Gun Automatic?")]
    [SerializeField] private bool isAutomatic = false;

    [Header("Ammunition")]
    [SerializeField] public float currentAmmo = 12;
    [SerializeField] public float reserveAmmo = 50;
    [SerializeField] private Text textCurrentAmmo;
    [SerializeField] private Text textReserveAmmo;

    [Header("Reload Settings")]
    [SerializeField] float reloadDuration = 1.5f;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private int maxAmmo = 30;
    private bool isReloading = false;


    [Header("Gun Shake")]
    [SerializeField] private float swayAmountX = 0.05f;
    [SerializeField] private float swayAmounty = 0.05f;
    [SerializeField] private float swaySpeed = 5f;
    [SerializeField] private float returnSmoothness = 10f;
    [SerializeField] private Transform playerTransform;

    [Header("Gun Damage")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 15f;
    [SerializeField] private float recoilForce = 0.1f;
    [SerializeField] private float recoilDuration = 0.1f;

    [Header("Visual Effects")]
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private float muzzleFlashDuration = 0.1f;
    [SerializeField] private Camera playerCamera;

    [Header("Delay Time")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float nextFireTime = 0f;
    private Vector3 initialWeaponPosition;


    private Vector3 initialPosition;
    private float timerX;
    private float timerY;

    [Header("Sound FX")]
    [SerializeField] private AudioClip gunshotSound;
    [SerializeField] private AudioSource audioSource;


    void Start()
    {
        initialPosition = transform.localPosition;
        initialWeaponPosition = transform.localPosition;
    }

    void Update()
    {
        ShakeGun();
        textCurrentAmmo.text = currentAmmo.ToString() + " / ";
        textReserveAmmo.text = reserveAmmo.ToString();
        if (currentAmmo > 0)
        {
            if (isAutomatic == false)
            {
                if (Input.GetButtonDown("Fire1") && Time.time > nextFireTime)
                {
                    nextFireTime = Time.time + 1f / fireRate;
                    Shoot();
                }
            }
            else if (isAutomatic == true)
            {
                if (Input.GetButton("Fire1") && Time.time > nextFireTime)
                {
                    nextFireTime = Time.time + 1f / fireRate;
                    Shoot();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && reserveAmmo > 0)
        {
            StartCoroutine(Reload());
        }
    }

    void ShakeGun()
    {
        bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;

        if (isMoving)
        {
            timerX += Time.deltaTime * swaySpeed;
            timerY += Time.deltaTime * swaySpeed;

            float swayOffsetX = Mathf.Sin(timerX) * swayAmountX;
            float swayOffsetY = Mathf.Cos(timerY) * swayAmounty;

            Vector3 targetPosition = initialPosition + new Vector3(swayOffsetX, swayOffsetY, 0);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * returnSmoothness);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * returnSmoothness);
            timerX = 0;
            timerY = 0;
        }
    }

    void Shoot()
    {
        if(isReloading || currentAmmo <= 0)
        {
            return;
        }

        StartCoroutine(MuzzleFlashEffect());
        StartCoroutine(RecoilEffect());
        if (gunshotSound != null) audioSource.PlayOneShot(gunshotSound);
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, range))
        {
            Debug.Log("Acertou");
            EnemyController enemy = hit.transform.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        currentAmmo--;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Vector3 startPosition = transform.localPosition;
        Vector3 loweredPosition = startPosition + new Vector3(0, -0.2f, 0);
        float timer = 0f;

        if (reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        while (timer < reloadDuration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, loweredPosition, timer / reloadDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        reserveAmmo -= (maxAmmo - currentAmmo);
        if (reserveAmmo < 0)
        {
            reserveAmmo = 0;
        }
        currentAmmo += (maxAmmo - currentAmmo);

        isReloading = false;    
    }

    IEnumerator MuzzleFlashEffect()
    {
        if (muzzleFlash == null)
        {
            Debug.LogWarning("Muzzle Flash Prefab não atribuído! (WeaponController)");
            yield break;
        }
        if (firePoint == null)
        {
            Debug.LogWarning("Fire Point não atribuído! (WeaponController)");
            yield break; // Interrompe a coroutine se o firePoint não estiver setado.
        }

        muzzleFlash.gameObject.SetActive(true);
        yield return new WaitForSeconds(muzzleFlashDuration);
        muzzleFlash.gameObject.SetActive(false);
    }

    IEnumerator RecoilEffect()
    {
        float timer = 0f;
        Vector3 recoilPosition = initialWeaponPosition - transform.forward * recoilForce;

        while (timer < recoilDuration)
        {
            transform.localPosition = Vector3.Lerp(initialWeaponPosition, recoilPosition, timer / recoilDuration);
            timer += Time.deltaTime;
            yield return null; // Espera o próximo frame
        }

        timer = 0f;
        Vector3 currentPosition = transform.localPosition;
        while (timer < recoilDuration) // Pode usar um tempo diferente para o retorno
        {
            transform.localPosition = Vector3.Lerp(currentPosition, initialWeaponPosition, timer / recoilDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = initialWeaponPosition; // Garante que volte para a posição exata
    }
}
