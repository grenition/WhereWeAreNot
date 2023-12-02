using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour
{



    private enum StateTypes
    {
        Seek,
        Chase,
        Attack,
        Escape
    }

    [Header("Текущее состояние")]
    [SerializeField] private StateTypes currentState = StateTypes.Seek;

    [Header("Навигационные точки")]
    [SerializeField] bool targetSet;
    [SerializeField] Vector3 target;
    // [SerializeField] private Vector3 walkPoint;

    [Header("Дистанция срабатывания триггеров")]
    [SerializeField] float seekRadius = 15f;
    [SerializeField] float distanceToStartChase = 10f;
    [SerializeField] float distanceToStartAttack = 5f;

    [Header("Параметры ИИ")]
    [Tooltip("Вероятность испугаться игрока")]
    [SerializeField][Range(0f, 100f)] float fright = 30f;
    [Tooltip("Задержка перед совершением действия (сек)")]
    [SerializeField] float reflex = 2f;

    [Header("Параметры Атаки")]
    [Tooltip("Объект для выстрела")]
    [SerializeField] GameObject projectile;
    [Tooltip("Позиция выстрела на объекте врага")]
    [SerializeField] Transform shootingPoint;
    [Tooltip("Скорость полёта снаряда")]
    [SerializeField] float projectileVelocity = 15f;




    private NavMeshAgent navMeshAgent;
    private Vector3 initialPozition;
    private bool isAttacking;

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

        if (currentState == StateTypes.Seek && !isAttacking) Seek();
        if (currentState == StateTypes.Chase && !isAttacking) Chase();
        if (currentState == StateTypes.Attack && !isAttacking) StartCoroutine(Attack());
        // if (currentState == StateTypes.Escape) Escape();

        navMeshAgent.SetDestination(target);
    }

    void ManageStates()
    {
        float distanceToPlayer = Vector3.Distance(Player.Instance.transform.position, transform.position);

        if (Physics.Raycast(new Ray(transform.position, Player.Instance.transform.position - transform.position), out RaycastHit _hit, distanceToStartChase))
        {
            if (_hit.collider.gameObject == Player.Instance.gameObject)
            {
                if (distanceToPlayer < distanceToStartAttack)
                    UpdateState(StateTypes.Attack);
                else
                    UpdateState(StateTypes.Chase);
            }
            else
                UpdateState(StateTypes.Seek);
        }
        else UpdateState(StateTypes.Seek);

    }

    void UpdateState(StateTypes newState)
    {
        currentState = newState;
    }

    void Seek()
    {
        if (!targetSet)
        {
            float randomX = Random.Range(initialPozition.x - seekRadius, initialPozition.x + seekRadius);
            // float randomY = Random.Range(-walkPointRange, walkPointRange);
            float randomZ = Random.Range(initialPozition.z - seekRadius, initialPozition.z + seekRadius);

            target = new Vector3(randomX, transform.position.y, randomZ);

            targetSet = true;
        }

        // Vector3 distanceToWalkPoint = transform.position - target;
        if (Vector3.Distance(transform.position, target) < 1f) targetSet = false;
    }

    void Chase()
    {
        target = Player.Instance.transform.position;
    }

    IEnumerator Attack()
    {
        // Остановиться перед игроком
        isAttacking = true;
        target = transform.position;

        // Выстрелить в игрока с задержкой Reflex

        yield return new WaitForSeconds(reflex);
        GameObject prj = Instantiate(projectile, shootingPoint.position, shootingPoint.rotation);
        prj.GetComponent<Rigidbody>().velocity = (Player.Instance.transform.position - transform.position).normalized * projectileVelocity;
        isAttacking = false;
    }

    void Escape()
    {
        target = initialPozition;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 255, 0);
        Gizmos.DrawWireCube(transform.position, new Vector3(seekRadius * 2f, 2, seekRadius * 2));


        Gizmos.color = new Color(0, 0, 255);
        Gizmos.DrawWireSphere(transform.position, distanceToStartChase);


        Gizmos.color = new Color(255, 0, 0);
        Gizmos.DrawWireSphere(transform.position, distanceToStartAttack);
    }
}
