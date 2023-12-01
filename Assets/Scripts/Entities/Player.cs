using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerMovement))]
public class Player : Entity
{
    public static Player Instance { get; private set; }
    public PlayerMovement Movement { get => movement; }

    private PlayerMovement movement;
    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        movement = GetComponent<PlayerMovement>();
    }
}
