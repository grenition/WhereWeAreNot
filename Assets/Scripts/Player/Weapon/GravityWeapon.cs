using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityWeapon : Weapon
{
    [SerializeField] private DynamicGravityTrigger gravityTriggerPrefab;
    [SerializeField] private float maxDistance = 100f;
    private DynamicGravityTrigger tempObject;
    private void Start()
    {
        tempObject = Instantiate(gravityTriggerPrefab);
        tempObject.TriggerEnabled = false;
    }
    void Update()
    {
        if (Physics.SphereCast(transform.position, 1f, transform.forward, out RaycastHit hit, maxDistance))
        {
            if (hit.collider.TryGetComponent(out Surface surf) && surf.GravityGunSurface)
            {
                tempObject.gameObject.SetActive(true);
                tempObject.transform.rotation = Quaternion.LookRotation(hit.normal, Vector3.forward);
                tempObject.SetPositionByPointOnPlane(hit.point);
            }
            else
                tempObject.gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
            Shoot();
    }
    private void Shoot()
    {
        if (gravityTriggerPrefab == null)
            return;
        if(Physics.SphereCast(transform.position, 1f, transform.forward, out RaycastHit hit, maxDistance))
        {
            if (hit.collider.TryGetComponent(out Surface surf) && surf.GravityGunSurface)
            {
                DynamicGravityTrigger trigger = Instantiate(gravityTriggerPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                trigger.SetPositionByPointOnPlane(hit.point);
            }
        }
    }
}
