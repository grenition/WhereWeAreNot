using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float lifeTime = 5f;
    [SerializeField] float damage = 25f;
    [SerializeField] float explosionRadius = 2.5f;

    [SerializeField] GameObject destroyPrefab;

    void Start() {
        StartCoroutine(DestroyAfterLifeTime());
    }

    IEnumerator DestroyAfterLifeTime() {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision) {
        foreach (var collider in Physics.OverlapSphere(transform.position, explosionRadius))
        {
            if(collider.TryGetComponent(out Health health))
            {
                health.Damage(damage);
            }
        }

        Destroy(gameObject);
    }

    void OnDestroy() {
        if (destroyPrefab != null) {
            Instantiate(destroyPrefab, transform.position, Quaternion.identity);
        }
    }
}
