using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTrigger : EntityTrigger
{
    protected override void OnPlayerStay(Player _player)
    {
        _player.Movement.MoveInWater();
    }
}
