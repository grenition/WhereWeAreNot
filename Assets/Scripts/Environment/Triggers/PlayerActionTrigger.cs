using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerActionTrigger : EntityTrigger
{
    public UnityEvent OnPlayerTriggerEnter;
    public UnityEvent OnPlayerTriggerStay;
    public UnityEvent OnPlayerTriggerExit;
    protected override void OnPlayerEnter(Player _player)
    {
        OnPlayerTriggerEnter?.Invoke();
    }
    protected override void OnEntityStay(Entity _entity)
    {
        OnPlayerTriggerStay?.Invoke();
    }
    protected override void OnPlayerExit(Player _player)
    {
        OnPlayerTriggerExit?.Invoke();
    }
}
