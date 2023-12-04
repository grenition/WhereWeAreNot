using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject spawningAfterCollision;
    [SerializeField] private bool setNormalToNewObject = true;
    [SerializeField] private float lifeTime = 5f;
    [Header("DestroyScenario")]
    [SerializeField] private bool useDestroyScenario = false;
    [SerializeField] private float destroyTime = 0.5f;
    [SerializeField] private MeshRenderer[] disableOnStartDestroying;
    [SerializeField] private AudioSource source;

    private Rigidbody rb;
    private bool isDestroyed = false;
    private float targetLifeTime = 0;

    public Action<PhysicsPoint> OnPhysicsPointSpawned;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        targetLifeTime = Time.time + lifeTime;
    }
    private void Update()
    {
        if (Time.time > targetLifeTime)
            Destroy(gameObject);
    }
    public void AddForce(Vector3 velocity)
    {
        rb.velocity = velocity;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (isDestroyed)
            return;

        if (spawningAfterCollision != null)
        {
            GameObject obj = Instantiate(spawningAfterCollision, transform.position, Quaternion.identity);
            if (setNormalToNewObject)
            {
                obj.transform.LookAt(collision.contacts[0].normal, Vector3.up);
            }

            if (obj.TryGetComponent(out PhysicsPoint point))
                OnPhysicsPointSpawned?.Invoke(point);
        }

        rb.isKinematic = true;
        isDestroyed = true;
        DestroyBullet();
    }
    public void DestroyBullet()
    {
        if (!useDestroyScenario)
            Destroy(gameObject);
        else
        {
            StopAllCoroutines();
            StartCoroutine(DestroyScenarioCoroutine());
        }
            
    }
    private IEnumerator DestroyScenarioCoroutine()
    {
        foreach (var mesh in disableOnStartDestroying)
            mesh.enabled = false;
        float targetTime = Time.time + destroyTime;
        float startVolume = 0f;
        if (source != null)
            startVolume = source.volume;
        while(Time.time < targetTime)
        {
            float t = 1 - ((targetTime - Time.time) / destroyTime);
            if (source != null)
                source.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }
        Destroy(gameObject);
    }
}
