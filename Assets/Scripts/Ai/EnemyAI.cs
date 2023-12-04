using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Walker))]
public class EnemyAI : MonoBehaviour
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
    [Tooltip("Задержка перед совершением действия (сек)")]
    [SerializeField] float reflex = 2f;
    [Tooltip("Вероятность испугаться игрока")]
    [SerializeField][Range(0f, 100f)] float fright = 30f;

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
    [SerializeField] AudioClip shootSound;

    private Vector3 initialPozition;
    private bool isAttacking;
    private bool isEscaping;

    private Walker walker;
    private NavMeshPath path;
    private AudioSource audioSource;
    private float pathFindingTargetTime = 0f;
    private float pathFindingInterval = 0.1f;
    private bool pathCorrect = false;
    private bool dontEscape = false;

    // Start is called before the first frame update
    void Start()
    {
        walker = GetComponent<Walker>();
        path = new NavMeshPath();
        audioSource = GetComponent<AudioSource>();
        initialPozition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ManageStates();

        if (!isAttacking)
        {
            switch (currentState)
            {
                case StateTypes.Seek: SeekTarget(); break;
                case StateTypes.Chase: ChaseTarget(); break;
                case StateTypes.Attack: StartCoroutine(AttackTarget()); break;
                case StateTypes.Escape: EscapeToInitialPoint(); break;
            }
        }

        WalkToTarget();
    }

    void WalkToTarget()
    {
        if (target == null)
            return;

        if (Time.time > pathFindingTargetTime)
        {
            pathCorrect = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
            pathFindingTargetTime = Time.time + pathFindingInterval;
        }

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
        }
        if (pathCorrect && path.corners.Length < 2)
            return;

        Vector3 direction = (target - transform.position).normalized;

        if (pathCorrect) direction = (path.corners[1] - path.corners[0]).normalized;
        if (!isAttacking)
        {
            rotationBody.rotation = Quaternion.LookRotation(Vector3.RotateTowards(rotationBody.forward, direction, 720 * Time.deltaTime, 0f));
            walker.MoveOnPlane(direction * movementSpeed);
        }
        else
        {
            walker.MoveOnPlane(Vector3.zero);
        }
    }

    void ManageStates()
    {
        if (currentState != StateTypes.Seek) seekPointSet = false;
        if (isEscaping) return;

        bool ifHit = Physics.Raycast(new Ray(transform.position, Player.Instance.transform.position - transform.position), out RaycastHit _hit, distanceToStartChase);

        if (ifHit && _hit.collider.gameObject == Player.Instance.gameObject)
        {
            if (Random.Range(1f, 100f) < fright && !dontEscape)
            {
                StartCoroutine(DontEscapeDelay());
                UpdateState(StateTypes.Escape);
            }
            else if (Vector3.Distance(Player.Instance.transform.position, transform.position) < distanceToStartAttack)
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
            float randomZ = Random.Range(initialPozition.z - seekRadius, initialPozition.z + seekRadius);

            Vector3 randomPoint = new Vector3(randomX, transform.position.y, randomZ);

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

        if (shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        isAttacking = false;
    }

    void EscapeToInitialPoint()
    {
        isEscaping = true;
        target = initialPozition;
        if (Vector3.Distance(transform.position, initialPozition) < 1.5f) isEscaping = false;

    }


    IEnumerator DontEscapeDelay()
    {
        dontEscape = true;
        yield return new WaitForSeconds(10f);
        dontEscape = false;
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
