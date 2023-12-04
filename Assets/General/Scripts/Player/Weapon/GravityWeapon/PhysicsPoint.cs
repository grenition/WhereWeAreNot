using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody), typeof(FixedJoint))]
public class PhysicsPoint : MonoBehaviour
{
    public bool IsConnected { get => joint.connectedBody != null; }
    public Rigidbody ConnectedBody { get => joint.connectedBody; }

    [SerializeField] private float detectingRadius = 0.5f;
    [SerializeField] private float attractionVelocity = 10f;
    [SerializeField] private float startVelocity = 0f;
    [SerializeField] private float minDitanceBetweenPoints = 1f;
    [SerializeField] private float lifeTime = 1f;
    [SerializeField] private float attractionMassScale = 500f;
    [SerializeField] private GameObject destroyEffect;

    private Rigidbody rb;
    private FixedJoint joint;
    private PhysicsPoint otherPoint;
    private float destroyTargetTime = 0f;
    private float currentMinDistance = float.MaxValue;
    private bool initialized = false;

    public Action OnPointCycleEnded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        joint = GetComponent<FixedJoint>();
    }
    private void Start()
    {
        if (!initialized)
            Initialize();
    }
    public void Initialize()
    {
        ConnectJoint();
        initialized = false;
    }
    public void DestroyPoint()
    {
        if (destroyEffect != null)
            Instantiate(destroyEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    private void FixedUpdate()
    {
        if(otherPoint != null)
        {
            float distance = Vector3.Distance(otherPoint.transform.position, transform.position);
            if (distance < currentMinDistance)
                currentMinDistance = distance;

            Vector3 velocity = (otherPoint.transform.position - transform.position).normalized * attractionVelocity * Time.fixedDeltaTime;
            rb.velocity += velocity;

            if (Time.time > destroyTargetTime || distance < minDitanceBetweenPoints || distance > currentMinDistance)
            {
                otherPoint = null;
                OnPointCycleEnded?.Invoke();
            }
        }
    }
    public void MoveToOtherPoint(PhysicsPoint point)
    {
        rb.isKinematic = false;
        rb.useGravity = false;
        otherPoint = point;
        rb.velocity = (otherPoint.transform.position - transform.position).normalized * startVelocity;
        joint.connectedMassScale = attractionMassScale;
        destroyTargetTime = Time.time + lifeTime;
    }
    private void ConnectJoint()
    {
        foreach (var col in Physics.OverlapSphere(transform.position, detectingRadius))
        {
            if (col.TryGetComponent(out ThrowableObject obj))
            {
                joint.connectedBody = obj._Rigidbody;
                rb.isKinematic = false;
                joint.connectedMassScale = 1;
                return;
            }
        }
        rb.isKinematic = true;
    }
}
