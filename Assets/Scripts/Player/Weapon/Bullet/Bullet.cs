using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject spawningAfterCollision;
    [SerializeField] private bool setNormalToNewObject = true;

    private Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetShot(Vector3 velocity)
    {
        rb.velocity = velocity;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (spawningAfterCollision != null)
        {
            GameObject obj = Instantiate(spawningAfterCollision, transform.position, Quaternion.identity);
            if (setNormalToNewObject)
            {
                //obj.transform.LookAt()
            }
        }

        Destroy(gameObject);
    }
}
