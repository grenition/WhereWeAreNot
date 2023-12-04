using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeGravityTrigger : EntityTrigger
{
    [SerializeField] private Vector3 newGravityDirection = Vector3.up;
    protected override void OnEntityEnter(Entity _entity)
    {
        _entity.CurrentWalker.SetTargetRotation(transform.TransformDirection(newGravityDirection));
    }
}
