using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieOnEnterTrigger : EntityTrigger
{
    protected override void OnEntityEnter(Entity _entity)
    {
        if (_entity.TryGetComponent(out Health health))
            health.Die();
    }
}
