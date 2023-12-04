using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieOnExitTrigger : EntityTrigger
{
    protected override void OnEntityExit(Entity _entity)
    {
        if (_entity.TryGetComponent(out Health health))
            health.Die();
    }
}
