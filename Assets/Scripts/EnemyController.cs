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
    [SerializeField] private GameObject bloodEffect;
    [SerializeField] private float bloodEffectTime = 0.2f;

    [Header("Enemy Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] public float enemyHealth;

    private NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = chaseSpeed;
        enemyHealth = maxHealth;
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
        StartCoroutine(BloodEffect());
        enemyHealth -= amount;
        Debug.Log("Inimigo levou dano de " + amount + " agora tem " + enemyHealth + " de vida");
        if (enemyHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Inimigo morreu");
        Destroy(gameObject);
    }

    IEnumerator BloodEffect()
    {
        if(bloodEffect != null)
        { 
            bloodEffect.gameObject.SetActive(true);
            yield return new WaitForSeconds(bloodEffectTime);
            bloodEffect.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Blood effect not assigned");
        }
    }


}
