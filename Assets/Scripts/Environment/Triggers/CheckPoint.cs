using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckPoint : EntityTrigger
{
    [SerializeField] private Transform checkPoint;

    public UnityEvent OnCheckPointEntered;

    protected override void OnPlayerEnter(Player _player)
    {
        Vector3 point = transform.position;
        if (checkPoint != null)
            point = checkPoint.position;
        RespawnController.SpawnPoint = point;
        OnCheckPointEntered?.Invoke();
    }
}
