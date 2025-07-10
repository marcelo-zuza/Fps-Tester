using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PistolBulletsController : MonoBehaviour
{
    [SerializeField] private GunController gunController;
    [SerializeField] private Text textGotAmmunation;
    [SerializeField] private float textTime = 0.5f;
    [SerializeField] private AudioClip itemFx;
    [SerializeField] private AudioSource audioSource;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gunController.reserveAmmo += 30;
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
