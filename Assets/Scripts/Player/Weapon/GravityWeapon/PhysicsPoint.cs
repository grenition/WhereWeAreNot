using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsPoint : MonoBehaviour
{
    [SerializeField] private float attractionVelocity = 10f;
    [SerializeField] private float minDitanceBetweenPoints = 1f;
    [SerializeField] private float lifeTime = 1f;

    private Rigidbody rb;
    private PhysicsPoint otherPoint;
    private float destroyTargetTime = 0f;

    public Action OnPointCycleEnded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }
    public void DestroyPoint()
    {
        Destroy(gameObject);
    }
    private void FixedUpdate()
    {
        if(otherPoint != null)
        {
            Vector3 velocity = (otherPoint.transform.position - transform.position).normalized * attractionVelocity;
            rb.velocity = velocity;
            if (Time.time > destroyTargetTime || Vector3.Distance(otherPoint.transform.position, transform.position) < minDitanceBetweenPoints)
            {
                otherPoint = null;
                OnPointCycleEnded?.Invoke();
            }
        }
    }
    public void MoveToOtherPoint(PhysicsPoint point)
    {
        rb.isKinematic = false;
        otherPoint = point;
        destroyTargetTime = Time.time + lifeTime;
    }
}
