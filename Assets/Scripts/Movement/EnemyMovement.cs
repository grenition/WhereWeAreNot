using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Walker))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float pathFindingInterval = 1f;
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float stopDistance = 1f;
    [SerializeField] private float startDistance = 2f;

    private Walker walker;
    private NavMeshPath path;
    private float pathFindingTargetTime = 0f;
    private bool pathCorrect = false;

    private void Awake()
    {
        walker = GetComponent<Walker>();
        path = new NavMeshPath();
    }
    private void FixedUpdate()
    {
        if (target == null)
            return;

        if(Time.time > pathFindingTargetTime)
        {
            pathCorrect = NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
            pathFindingTargetTime = Time.time + pathFindingInterval;
        }

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
        }

        if (pathCorrect && path.corners.Length < 2)
            return;

        if (Vector3.Distance(target.position, transform.position) > stopDistance)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            if(pathCorrect)
                direction = (path.corners[1] - path.corners[0]).normalized;

            walker.MoveOnPlane(direction * movementSpeed);
        }
    }
}
