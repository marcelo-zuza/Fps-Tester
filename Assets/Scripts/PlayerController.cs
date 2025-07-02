using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Text healthText;
    [SerializeField] float speed = 6f;
    [SerializeField] float mouseSensitivity = 500;
    [SerializeField] float gravitity = -9.8f;
    [SerializeField] public Transform playerCamera;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Jump Configuration")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private float jumpForce = 3f;

    [Header("Damage Management")]
    [SerializeField] private bool isTakingDamage = false;
    [SerializeField] private float damageAmount = 1;
    [SerializeField] private float damageInterval = 2;
    [SerializeField] private Image damageOverlay;
    [SerializeField] private float flashSpeed = 5f;
    [SerializeField] private float overlayMaxAlpha = 0.4f;
    private bool flashing = false;


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
        healthText.text = health.ToString();

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

        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(ApplyDamageOverTime());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            isGrounded = false;
        }

        if (other.CompareTag("Enemy"))
        {

        }
    }

    IEnumerator ApplyDamageOverTime()
    {
        isTakingDamage = true;

        while (isTakingDamage)
        {
            health -= damageAmount;
            health = Mathf.Clamp(health, 0, maxHealth);

            healthText.text = health.ToString();
            StartCoroutine(FlashDamageOverlay());

            if (health <= 0)
            {
                Die();
                yield break;
            }
            yield return new WaitForSeconds(damageInterval);
        }
    }

    void Die()
    {
        Debug.Log("Player morreu");
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
