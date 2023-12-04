using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interaction : MonoBehaviour
{
    public bool IsInteractable { get => !(interacted && oneTimeUse); }
    public string InteractionText { get => interactionText; }
    public bool Interacted { get => interacted; set { interacted = value; } }

    [SerializeField] private string interactionText = "Взаимодействовать";
    [SerializeField] private bool oneTimeUse = true;
    public UnityEvent interactionEvent;

    private bool interacted = false;

    public void Interact()
    {
        if (interacted && oneTimeUse)
            return;
        interactionEvent?.Invoke();
        interacted = true;
    }
}
