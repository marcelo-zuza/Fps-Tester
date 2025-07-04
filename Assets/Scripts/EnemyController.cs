using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private WeaponSwitcher weaponSwitcher;
    
    [Header("Enemy Configuration")]
    [SerializeField] private float chaseSpeed = 1f;
    [SerializeField] public Transform player;
    [SerializeField] public bool isChasing = false;

    [Header("Enemy Stats")]
    [SerializeField] public float enemyHealth = 100f;

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

    public void TakeDamage(float amount)
    {
        enemyHealth -= amount;
        if (enemyHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {

    }
}
