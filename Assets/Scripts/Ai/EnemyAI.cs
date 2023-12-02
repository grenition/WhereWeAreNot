using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour
{

    [Header("Цель преследования")]
    [SerializeField] Vector3 target;

    private enum StateTypes
    {
        Seek,
        Chase,
        Attack,
        Escape
    }

    [SerializeField] private StateTypes currentState = StateTypes.Seek;

    [Header("Навигационные точки")]
    [SerializeField] bool walkPointSet;
    // [SerializeField] private Vector3 walkPoint;

    [Header("Дистанция срабатывания триггеров")]
    [SerializeField] float seekRadius = 15f;
    [SerializeField] float distanceToStartChase = 10f;
    [SerializeField] float distanceToStartAttack = 5f;

    [Header("Действия ИИ")]
    [Tooltip("Вероятность испугаться игрока")]
    [SerializeField][Range(0f, 100f)] float playerFright = 30f;
    [Tooltip("Задержка перед совершением действия (сек)")]
    [SerializeField] float reflex = 2f;


    private NavMeshAgent navMeshAgent;
    private Vector3 initialPozition;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        initialPozition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        ManageStates();

        if (currentState == StateTypes.Seek) Seek();
        if (currentState == StateTypes.Chase) Chase();
        if (currentState == StateTypes.Attack) Attack();
        if (currentState == StateTypes.Escape) Escape();
    }

    void ManageStates()
    {
        if(Vector3.Distance(Player.Instance.transform.position, transform.position) < distanceToStartChase) {
            UpdateState(StateTypes.Chase);
        } else {
            UpdateState(StateTypes.Seek);
        }

    }

    void UpdateState(StateTypes newState)
    {
        currentState = newState;
    }

    void Seek()
    {
        if (!walkPointSet)
        {
            float randomX = Random.Range(initialPozition.x - seekRadius, initialPozition.x + seekRadius);
            // float randomY = Random.Range(-walkPointRange, walkPointRange);
            float randomZ = Random.Range(initialPozition.z - seekRadius, initialPozition.z + seekRadius);

            target = new Vector3(randomX, transform.position.y, randomZ);

            walkPointSet = true;
        }

        if (walkPointSet)
        {
            navMeshAgent.SetDestination(target);
        }

        // Vector3 distanceToWalkPoint = transform.position - target;
        if (Vector3.Distance(transform.position, target) < 1f) walkPointSet = false;
    }

    void Chase()
    {
        target = Player.Instance.transform.position;
        navMeshAgent.SetDestination(target);

    }

    void Attack()
    {

    }

    void Escape()
    {

    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 255, 0);
        Gizmos.DrawWireCube(initialPozition, new Vector3(seekRadius * 2f, 2,seekRadius * 2));


        Gizmos.color = new Color(0, 0, 255);
        Gizmos.DrawWireSphere(transform.position, distanceToStartChase);


        Gizmos.color = new Color(255, 0, 0);
        Gizmos.DrawWireSphere(transform.position, distanceToStartAttack);
    }
}
