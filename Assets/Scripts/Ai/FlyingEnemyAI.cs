using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Walker))]
public class FlyingEnemyAI : MonoBehaviour
{
    private enum StateTypes
    {
        Seek,
        Chase,
        Attack,
        Escape
    }

    [Header("Параметры ИИ")]
    [Tooltip("Cкорость передвижения")]
    [SerializeField] float movementSpeed = 5f;
    [Tooltip("Вероятность испугаться игрока")]
    [SerializeField][Range(0f, 100f)] float fright = 30f;
    [Tooltip("Задержка перед совершением действия (сек)")]
    [SerializeField] float reflex = 2f;

    [Header("Навигационное состояние")]
    [SerializeField] private StateTypes currentState = StateTypes.Seek;
    [SerializeField] bool targetSet;
    [SerializeField] Vector3 target;
    // [SerializeField] private Vector3 walkPoint;

    [Header("Дистанция срабатывания триггеров")]
    [SerializeField] float seekRadius = 15f;
    [SerializeField] float distanceToStartChase = 10f;
    [SerializeField] float distanceToStartAttack = 5f;


    [Header("Параметры Атаки")]
    [Tooltip("Центр вращения объекта")]
    [SerializeField] Transform rotationBody;
    [Tooltip("Позиция выстрела на объекте")]
    [SerializeField] Transform shootingPoint;
    [Tooltip("Объект для выстрела")]
    [SerializeField] GameObject projectile;
    [Tooltip("Скорость полёта снаряда")]
    [SerializeField] float projectileVelocity = 15f;


    private Vector3 initialPozition;
    private bool isAttacking;

    private Walker walker;
    private NavMeshPath path;
    private float pathFindingTargetTime = 0f;
    private float pathFindingInterval = 0.1f;
    private bool pathCorrect = false;

    // Start is called before the first frame update
    void Start()
    {
        walker = GetComponent<Walker>();
        path = new NavMeshPath();
        initialPozition = transform.position;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        ManageStates();

        if (currentState == StateTypes.Seek && !isAttacking) Seek();
        if (currentState == StateTypes.Chase && !isAttacking) Chase();
        if (currentState == StateTypes.Attack && !isAttacking) StartCoroutine(Attack());


        Vector3 direction = (target - transform.position).normalized;


        if (!isAttacking)
        {
            rotationBody.rotation = Quaternion.LookRotation(Vector3.RotateTowards(rotationBody.forward, direction, 720 * Time.deltaTime, 0f));
            walker.MoveInExtraMode(direction * movementSpeed);
        } else {
            walker.MoveInExtraMode(Vector3.zero);
        }


    }

    void ManageStates()
    {
        if (isAttacking) return;


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
            float randomY = Random.Range(initialPozition.y - seekRadius, initialPozition.y + seekRadius);
            float randomZ = Random.Range(initialPozition.z - seekRadius, initialPozition.z + seekRadius);

            Vector3 randomPoint = new Vector3(randomX, randomY, randomZ);

            if (Vector3.Distance(randomPoint, transform.position) < 3f)
            {
                Seek();
                return;
            }

            target = randomPoint;
            targetSet = true;
        }

        // Vector3 distanceToWalkPoint = transform.position - target;
        if (Vector3.Distance(transform.position, target) < 1.5f) targetSet = false;
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
        Gizmos.DrawWireSphere(initialPozition, seekRadius);


        Gizmos.color = new Color(0, 0, 255);
        Gizmos.DrawWireSphere(transform.position, distanceToStartChase);


        Gizmos.color = new Color(255, 0, 0);
        Gizmos.DrawWireSphere(transform.position, distanceToStartAttack);
    }
}
