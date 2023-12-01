using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Walker))]
public class Entity : MonoBehaviour
{
    public Walker CurrentWalker { get => walker; }

    private Walker walker;
    protected virtual void Awake()
    {
        walker = GetComponent<Walker>();
    }
}
