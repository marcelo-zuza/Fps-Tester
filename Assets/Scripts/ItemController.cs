using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1f;
    void Start()
    {

    }

    void Update()
    {
        ItemMovement();
    }

    void ItemMovement()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
