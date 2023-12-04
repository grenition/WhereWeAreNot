using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerActionTrigger : EntityTrigger
{
    [SerializeField] private float holdTime = 10f;

    public UnityEvent OnPlayerTriggerEnter;
    public UnityEvent OnPlayerTriggerStay;
    public UnityEvent OnPlayerTriggerExit;
    public UnityEvent OnHoldTriggerEnter;
    protected override void OnPlayerEnter(Player _player)
    {
        OnPlayerTriggerEnter?.Invoke();
        Invoke("InvokeHoldTriggerEnter", holdTime);
    }
    protected override void OnEntityStay(Entity _entity)
    {
        OnPlayerTriggerStay?.Invoke();
    }
    protected override void OnPlayerExit(Player _player)
    {
        OnPlayerTriggerExit?.Invoke();
    }

    private void InvokeHoldTriggerEnter()
    {
        OnHoldTriggerEnter?.Invoke();
    }
}
