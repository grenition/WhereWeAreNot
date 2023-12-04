using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowGroundNormalTrigger : EntityTrigger
{
    protected override void OnEntityStay(Entity _entity)
    {
        _entity.CurrentWalker.FollowGroundNormal();
    }
    protected override void OnEntityExit(Entity _entity)
    {
        _entity.CurrentWalker.DontFollowGroundNormal();
    }
}
