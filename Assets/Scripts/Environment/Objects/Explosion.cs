using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float damage = 40f;
    [SerializeField] private float radius = 5f;
    [SerializeField] private float destroyDelay = 4f;
    [SerializeField] private bool explodeOnAwake = true;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
        Gizmos.DrawSphere(transform.position, radius);
    }
    private void OnEnable()
    {
        if (explodeOnAwake)
            Explode();
    }
    public void Explode(bool destroyStopAction = true)
    {
        foreach (var col in Physics.OverlapSphere(transform.position, radius))
        {
            if(col.TryGetComponent(out Health health))
            {
                health.Damage(damage);
            }
        }
        if (destroyStopAction)
            Invoke("DestroyObject", destroyDelay);
    }
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
