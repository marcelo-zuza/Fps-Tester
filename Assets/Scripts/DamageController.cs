using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DamageController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [Header("Damage Management")]
    [SerializeField] private bool isTakingDamage = false;
    [SerializeField] private float damageAmount = 1f;
    [SerializeField] private float damageInterval = 2f;
    [SerializeField] private Image damageOverlay;
    [SerializeField] private float flashSpeed = 5f;
    [SerializeField] private float overlayMaxAlpha = 0.4f;
    [SerializeField] private Text healthText;
    private Coroutine damageCoroutine;

    private bool flashing = false;

    void Start()
    {
        healthText.text = playerController.health.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(ApplyDamageOverTime());
            }
            // StartCoroutine(ApplyDamageOverTime());
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
            //StopCoroutine(ApplyDamageOverTime());
            
            
        }    
    }

    IEnumerator ApplyDamageOverTime()
    {
        isTakingDamage = true;

        while (isTakingDamage)
        {
            playerController.health -= damageAmount;
            playerController.health = Mathf.Clamp(playerController.health, 0, playerController.maxHealth);

            healthText.text = playerController.health.ToString();
            StartCoroutine(FlashDamageOverlay());

            if (playerController.health <= 0)
            {
                playerController.Die();
                yield break;
            }
            yield return new WaitForSeconds(damageInterval);
        }
    }

    IEnumerator FlashDamageOverlay()
    {
        flashing = true;

        damageOverlay.color = new Color(1, 0, 0, overlayMaxAlpha);

        yield return new WaitForSeconds(0.1f);

        float elapsed = 0f;
        Color startColor = damageOverlay.color;
        Color endColor = new Color(1, 0, 0, 0);

        while (elapsed < 0.5f)
        {
            damageOverlay.color = Color.Lerp(startColor, endColor, elapsed / 0.5f);
            elapsed += Time.deltaTime * flashSpeed;
            yield return null;
        }

        damageOverlay.color = endColor;
        flashing = false;
    }
}
