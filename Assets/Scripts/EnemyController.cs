using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Configuration")]
    [SerializeField] private float chaseSpeed = 1f;
    [SerializeField] public Transform player;
    [SerializeField] public bool isChasing = false;

    private NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = chaseSpeed;
    }

    void Update()
    {
        if (isChasing && player != null && agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerIsClose"))
        {
            if (player != null)
            {
                isChasing = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerIsClose"))
        {
            isChasing = false;
            agent.ResetPath();
        }
    }
}
