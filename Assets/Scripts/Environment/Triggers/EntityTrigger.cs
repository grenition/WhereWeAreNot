using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityTrigger : MonoBehaviour
{
    public bool TriggerEnabled { get => triggerEnabled; set { triggerEnabled = value; } }

    public UnityAction OnPlayerTriggerEnter;
    public UnityAction OnPlayerTriggerExit;
    public List<Entity> EntitiesInTrigger { get => entities; }
    private List<Entity> entities = new List<Entity>();

    [SerializeField] private bool triggerEnabled = true;
    private void Awake()
    {
        gameObject.layer = 2;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!triggerEnabled)
            return;
        if (other.gameObject.TryGetComponent(out Entity _entity))
        {
            entities.Add(_entity);
            OnEntityEnter(_entity);
            if (_entity is Player)
            {
                OnPlayerTriggerEnter?.Invoke();
                OnPlayerEnter(_entity as Player);
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!triggerEnabled)
            return;
        foreach (var _entity in entities)
        {
            OnEntityStay(_entity);

            if (_entity is Player)
            {
                OnPlayerStay(_entity as Player);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!triggerEnabled)
            return;
        if (other.gameObject.TryGetComponent(out Entity _entity))
        {
            entities.Remove(_entity);
            OnEntityExit(_entity);

            if (_entity is Player)
            {
                OnPlayerTriggerExit?.Invoke();
                OnPlayerExit(_entity as Player);
            }
        }
    }
    #region Entity
    protected virtual void OnEntityEnter(Entity _entity) { }
    protected virtual void OnEntityExit(Entity _entity) { }
    protected virtual void OnEntityStay(Entity _entity) { }
    #endregion
    #region Player
    protected virtual void OnPlayerEnter(Player _player) { }
    protected virtual void OnPlayerExit(Player _player) { }
    protected virtual void OnPlayerStay(Player _player) { }
    #endregion
}
