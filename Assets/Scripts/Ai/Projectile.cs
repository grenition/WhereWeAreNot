using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] public float lifeTime = 5f;
    [SerializeField] public float damage = 15f;


    void Start() {
        StartCoroutine(DestroyAfterLifeTime());
    }

    IEnumerator DestroyAfterLifeTime() {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }


    void OnCollisionEnter(Collision collision) {
        if(collision.gameObject == Player.Instance.gameObject) {
            Player.Instance.GetComponent<Health>().Damage(damage);
        }

        Destroy(gameObject);
    }
}
