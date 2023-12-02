using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportingOnExitTrigger : EntityTrigger
{
    [SerializeField] private Transform targetPoint;
    protected override void OnEntityExit(Entity _entity)
    {
        _entity.transform.position = targetPoint.position;
        _entity.CurrentWalker.SetRotation(targetPoint.up);
    }
}
