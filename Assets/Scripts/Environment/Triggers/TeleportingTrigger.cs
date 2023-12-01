using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportingTrigger : EntityTrigger
{
    [SerializeField] private Transform pointOnEnter;
    [SerializeField] private Transform pointOnExit;

    protected override void OnEntityEnter(Entity _entity)
    {
        TeleportToPoint(_entity, pointOnEnter);
    }   
    protected override void OnEntityExit(Entity _entity)
    {
        TeleportToPoint(_entity, pointOnExit);
    }
    private void TeleportToPoint(Entity _entity, Transform _point)
    {
        _entity.transform.position = _point.position;
        _entity.CurrentWalker.SetTargetRotation(_point.up);
    }
}
