using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerMovement), typeof(PlayerBulletTime))]
public class Player : Entity
{
    public static Player Instance { get; private set; }
    public PlayerMovement Movement { get => movement; }
    public PlayerBulletTime BulletTime { get => bulletTime; }

    private PlayerMovement movement;
    private PlayerBulletTime bulletTime;
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
        bulletTime = GetComponent<PlayerBulletTime>();
    }
}
