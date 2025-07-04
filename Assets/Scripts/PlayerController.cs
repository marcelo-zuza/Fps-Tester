using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] public float health = 100f;
    [SerializeField] public float maxHealth = 100f;
    //[SerializeField] private Text healthText;
    [SerializeField] float speed = 6f;
    [SerializeField] float mouseSensitivity = 500;
    [SerializeField] float gravitity = -9.8f;
    [SerializeField] public Transform playerCamera;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Jump Configuration")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private float jumpForce = 3f;

    [Header("Death Settings")]
    [SerializeField] private Image deathOverlay;
    [SerializeField] private Text deathMessage;
    [SerializeField] private Text pressEnterToRestart;
    [SerializeField] private RawImage cross;
    [SerializeField] private float cameraTiltAngle = 30f;
    [SerializeField] private float cameraDropDistance = 0.5f;
    [SerializeField] private float effectDuration = 2f;
    [SerializeField] private bool isDead = false;


    private CharacterController playerController;
    private Vector3 velocity;
    private float xRotation = 0f;

    void Start()
    {
        playerController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        MovePlayer();
        //healthText.text = health.ToString();

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

    }

    void MovePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.Rotate(Vector3.up * mouseX);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        float xAxis = Input.GetAxis("Horizontal");
        float zAxis = Input.GetAxis("Vertical");
        Vector3 move = transform.right * xAxis + transform.forward * zAxis;
        playerController.Move(move * speed * Time.deltaTime);

        // gravity
        if (playerController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravitity * Time.deltaTime;
        playerController.Move(velocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(RotatePlayer(rotationSpeed));
        }
    }

    IEnumerator RotatePlayer(float rorationSpeed)
    {
        float startAngle = transform.eulerAngles.y;
        float targetAngle = startAngle = 180f;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            transform.rotation = Quaternion.Euler(0f, Mathf.Lerp(startAngle, targetAngle, elapsedTime), 0f);
            elapsedTime += Time.deltaTime * rotationSpeed;
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
    }

    void Jump()
    {
        if (playerController.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravitity);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }


    public void Die()
    {
        Debug.Log("Player morreu");
        TriggerDeath();
        
    }

    public void TriggerDeath()
    {
        if (!isDead)
        {
            isDead = true;
            if (cross != null)
            {
                cross.gameObject.SetActive(false);
            }
            GetComponent<PlayerController>().enabled = false;
            GetComponentInChildren<GunController>().enabled = false;
            GetComponent<CharacterController>().enabled = false;
            GetComponentInChildren<DamageController>().enabled = false;
            StartCoroutine(DeathEffect());

        }
    }

    IEnumerator DeathEffect()
    {
        float elapsed = 0f;
        Vector3 startPos = playerCamera.localPosition;
        Quaternion startRot = playerCamera.localRotation;
        Vector3 endPos = startPos + Vector3.down * cameraDropDistance;
        Quaternion endRot = Quaternion.Euler(cameraTiltAngle, 0, cameraTiltAngle);

        deathMessage.gameObject.SetActive(true);

        while (elapsed < effectDuration)
        {
            float alpha = Mathf.Lerp(0, 0.5f, elapsed / effectDuration);
            deathOverlay.color = new Color(1, 0, 0, alpha);

            playerCamera.localPosition = Vector3.Lerp(startPos, endPos, elapsed / effectDuration);
            playerCamera.localRotation = Quaternion.Slerp(startRot, endRot, elapsed / effectDuration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        deathOverlay.color = new Color(1, 0, 0, 0.5f);
        playerCamera.localPosition = endPos;
        playerCamera.localRotation = endRot;

        StartCoroutine(WaitForRestart());
    }

    IEnumerator WaitForRestart()
    {
        yield return new WaitForSeconds(3f);
        if (pressEnterToRestart != null)
        {
            pressEnterToRestart.gameObject.SetActive(true);
        }

        while (true)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                yield return null;
            }
    }

}
