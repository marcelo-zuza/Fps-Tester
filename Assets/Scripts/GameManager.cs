using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI")]
    [SerializeField] private Text ghostCounterText;
    [SerializeField] private GameObject victoryPanel;

    [SerializeField] private int totalGhosts;
    [SerializeField] private GameObject aimImage;
    [SerializeField] private Text pressEnterMessage;
 
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        totalGhosts = GameObject.FindGameObjectsWithTag("Enemy").Length;
        UpdateUI();
        if (victoryPanel != null) victoryPanel.SetActive(false);
        else Debug.Log("Victory Panel Missing");
        
    }

    public void GhostKilled()
    {
        totalGhosts--;
        UpdateUI();

        if (totalGhosts == 0)
        {
            StartCoroutine(WinGame());
        }
    }

    void UpdateUI()
    {
        if (ghostCounterText != null)
        {
            ghostCounterText.text = "Ghosts: " + totalGhosts;
        }
    }

IEnumerator WinGame()
{
    Debug.Log("You win");

    if (aimImage != null) aimImage.gameObject.SetActive(false);
    else Debug.Log("aimImage not found");

    if (victoryPanel != null) victoryPanel.gameObject.SetActive(true);
    else Debug.Log("Victory Panel missing");

    yield return new WaitForSeconds(2f);

    Time.timeScale = 0f;
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;

    yield return new WaitForSecondsRealtime(5f); // 

    if (pressEnterMessage != null) pressEnterMessage.gameObject.SetActive(true);
    else Debug.Log("pressEnterMessage not found");

    while (!Input.GetKeyDown(KeyCode.Return))
    {
        yield return null; 
    }

    Time.timeScale = 1f; 
    SceneManager.LoadScene(1);
}
}
