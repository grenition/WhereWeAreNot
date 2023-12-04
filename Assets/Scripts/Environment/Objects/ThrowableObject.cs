using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThrowableObject : MonoBehaviour
{
    public Rigidbody _Rigidbody { get => rb; }

    [Header("Destruction")]
    [SerializeField] private bool destructOnCollision = false;
    [SerializeField] private float destructVelocity = 10f;
    [SerializeField] private GameObject destructionEffect;

    [Header("Collisions")]
    [SerializeField] private float minVelocityToVisualizeCollision = 3f;
    [SerializeField] private AudioSource source;
    [SerializeField] private CollisionResources collisionResources;

    [Header("Health damage")]
    [SerializeField] private bool allowCollisionDamage = true;
    [SerializeField] private float velocityToDamage = 20f;
    [SerializeField] private float damagePer100kg = 50f;

    private Rigidbody rb;
    private Health health;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<Bullet>() != null)
            return;

        float velocity = collision.relativeVelocity.magnitude;
        if (velocity > minVelocityToVisualizeCollision)
            VisualizeCollision(collision.contacts[0].point, collision.contacts[0].normal);
        if (destructOnCollision && velocity > destructVelocity)
            Destruct();
        if(allowCollisionDamage && health != null && velocity > velocityToDamage)
        {
            float otherMass = 0f;
            if (collision.collider.TryGetComponent(out Rigidbody otherRb))
                otherMass += otherRb.mass;
            health.Damage(damagePer100kg * (rb.mass + otherMass) / 100f);
        }
    }
    public void VisualizeCollision(Vector3 point, Vector3 normal)
    {
        if (collisionResources == null)
            return;

        if(source != null)
        {
            AudioClip clip = collisionResources.GetRandomCollisionClip();
            if (clip != null)
                source.PlayOneShot(clip);
        }

        GameObject effect = collisionResources.GetRandomCollisionEffect();
        if (effect != null)
            Instantiate(effect, point, Quaternion.LookRotation(normal));
    }
    public void Destruct()
    {
        if (destructionEffect != null)
            Instantiate(destructionEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
