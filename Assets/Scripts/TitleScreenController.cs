using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenController : MonoBehaviour
{
    [Header("Main Title Settings")]
    [SerializeField] private GameObject mainTitleText;
    [SerializeField] private GameObject instructionsText;
    [SerializeField] private Text startGameMessage;
    [SerializeField] private float timeToShowStartMessage = 2f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip instructionsAudio;
    private bool isInstructionsOn = false;
    private bool isStartGameMessageOn = false;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (mainTitleText != null && instructionsText != null && startGameMessage != null)
            {
                if (!isInstructionsOn)
                {
                    isInstructionsOn = true;
                    mainTitleText.gameObject.SetActive(false);
                    instructionsText.gameObject.SetActive(true);
                    if (audioSource != null && instructionsAudio != null) audioSource.PlayOneShot(instructionsAudio);
                    else Debug.Log("Audio elements not found");
                    StartCoroutine(ShowStartGameMessage());
                }
                else if (isInstructionsOn && isStartGameMessageOn)
                {
                    SceneManager.LoadScene(1);
                }
            }
        }

    }

    IEnumerator ShowStartGameMessage()
    {
        yield return new WaitForSeconds(timeToShowStartMessage);
        isStartGameMessageOn = true;
        startGameMessage.gameObject.SetActive(true);
    }
}
