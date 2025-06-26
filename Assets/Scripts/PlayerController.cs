using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 6f;
    [SerializeField] float mouseSensitivity = 100f;
    [SerializeField] float gravitity = -9.8f;
    [SerializeField] public Transform playerCamera;
    [SerializeField] private float rotationSpeed = 5f;

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
}
