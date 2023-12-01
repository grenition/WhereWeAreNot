using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AINavigation : MonoBehaviour
{

    [Header("Navigation targets")]

    [SerializeField] Transform target;

    [Header("Navigation walkPoints")]

    [SerializeField] bool walkPointSet;
    [SerializeField] private float walkPointRange;
    [SerializeField] private Vector3 walkPoint;

    [Header("Navigation triggers")]
    [SerializeField] float distanceToStartChase = 10f;
    [SerializeField] float distanceToStartAttack = 5f;


    NavMeshAgent navMeshAgent;


    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(target.position, transform.position) < distanceToStartChase) Chase();
        else Patrol();
    }

    void Patrol()
    {
        if (!walkPointSet)
        {
            float randomX = Random.Range(-walkPointRange, walkPointRange);
            float randomY = Random.Range(-walkPointRange, walkPointRange);
            float randomZ = Random.Range(-walkPointRange, walkPointRange);

            // surface ground(fixed y) walking 
            if (transform.position.y < 5f) {
                navMeshAgent.enabled = false;
                walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
                navMeshAgent.enabled = true;
            }

            // surface wall(fixed z) walking 
            else {
                navMeshAgent.enabled = false;
                walkPoint = new Vector3(transform.position.x + randomX, transform.position.y + randomY, transform.position.z);
                navMeshAgent.enabled = true;
            }
            

            walkPointSet = true;
        }

        if (walkPointSet)
        {
            navMeshAgent.SetDestination(walkPoint);
        }


        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f) walkPointSet = false;
    }


    void Chase()
    {
        print("check");
        navMeshAgent.SetDestination(target.position);
    }
}
