using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthItemController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private DamageController damageController;
    [SerializeField] private Text textGotAmmunation;
    [SerializeField] private float textTime = 0.5f;
    [SerializeField] private AudioClip itemFx;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController.health += 30;
            damageController.healthText.text = playerController.health.ToString();
            if (itemFx != null && audioSource != null)
            {
                audioSource.PlayOneShot(itemFx);
            }
            StartCoroutine(ShowText());
        }
    }
    
    IEnumerator ShowText()
    {
        if (textGotAmmunation != null)
        {
            textGotAmmunation.gameObject.SetActive(true);
            yield return new WaitForSeconds(textTime);
            textGotAmmunation.gameObject.SetActive(false);
            yield return null;
        }
        Destroy(gameObject);
    }
}
