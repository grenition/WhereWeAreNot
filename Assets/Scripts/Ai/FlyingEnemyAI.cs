using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] Vector3 target;
    [SerializeField] bool seekPointSet;

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
    private bool isEscaping;

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

        switch (currentState)
        {
            case StateTypes.Seek: SeekTarget(); break;
            case StateTypes.Chase: ChaseTarget(); break;
            case StateTypes.Attack: StartCoroutine(AttackTarget()); break;
            case StateTypes.Escape: EscapeToInitialPoint(); break;
        }

        WalkToTarget();
    }

    void WalkToTarget()
    {

        Vector3 direction = (target - transform.position).normalized;


        if (!isAttacking)
        {
            rotationBody.rotation = Quaternion.LookRotation(Vector3.RotateTowards(rotationBody.forward, direction, 720 * Time.deltaTime, 0f));
            walker.MoveInExtraMode(direction * movementSpeed);
        }
        else
        {
            walker.MoveInExtraMode(Vector3.zero);
        }
    }

    void ManageStates()
    {
        if (currentState != StateTypes.Seek) seekPointSet = false;

        if (isEscaping && Vector3.Distance(transform.position, initialPozition) > 3f) return;
        isEscaping = false;

        if (isAttacking) return;

        float distanceToPlayer = Vector3.Distance(Player.Instance.transform.position, transform.position);

        bool ifHit = Physics.Raycast(new Ray(transform.position, Player.Instance.transform.position - transform.position), out RaycastHit _hit, distanceToStartChase);

        if (ifHit && _hit.collider.gameObject == Player.Instance.gameObject)
        {
            if (Random.Range(0f, 100f) < fright)
                UpdateState(StateTypes.Escape);
            else if (distanceToPlayer < distanceToStartAttack)
                UpdateState(StateTypes.Attack);
            else
                UpdateState(StateTypes.Chase);
        }
        else UpdateState(StateTypes.Seek);
    }

    void UpdateState(StateTypes newState)
    {
        currentState = newState;
    }

    void SeekTarget()
    {
        if (!seekPointSet || Vector3.Distance(transform.position, target) < 1.5f)
        {
            float randomX = Random.Range(initialPozition.x - seekRadius, initialPozition.x + seekRadius);
            float randomY = Random.Range(initialPozition.y - seekRadius, initialPozition.y + seekRadius);
            float randomZ = Random.Range(initialPozition.z - seekRadius, initialPozition.z + seekRadius);

            Vector3 randomPoint = new Vector3(randomX, randomY, randomZ);

            target = randomPoint;
            seekPointSet = true;
        }

        if (Vector3.Distance(transform.position, target) < 1.5f) seekPointSet = false;
    }

    void ChaseTarget()
    {
        target = Player.Instance.transform.position;
    }

    IEnumerator AttackTarget()
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

    void EscapeToInitialPoint()
    {
        isEscaping = true;
        target = initialPozition;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 255, 0);
        Gizmos.DrawWireCube(initialPozition, new Vector3(seekRadius * 2f, 2, seekRadius * 2));


        Gizmos.color = new Color(0, 0, 255);
        Gizmos.DrawWireSphere(transform.position, distanceToStartChase);


        Gizmos.color = new Color(255, 0, 0);
        Gizmos.DrawWireSphere(transform.position, distanceToStartAttack);
    }
}




