using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerMovement), typeof(PlayerBulletTime), typeof(PlayerHealth))]
public class Player : Entity
{
    public static Player Instance { get; private set; }
    public PlayerMovement Movement { get => movement; }
    public PlayerBulletTime BulletTime { get => bulletTime; }
    public PlayerHealth Health { get => health; }
    public bool Paused
    {
        get => paused;
        set
        {
            if ((value || dead) == paused)
                return;

            paused = value || dead;

            if (paused)
            {
                if (bulletTime != null)
                    bulletTime.enabled = false;
                Time.timeScale = 0f;
                CameraLooking.Instance.ShowCursor = true;
                CameraLooking.Instance.LockRotation = true;
            }
            else
            {
                Time.timeScale = 1f;
                if (bulletTime != null)
                    bulletTime.enabled = true;
                CameraLooking.Instance.ShowCursor = false;
                CameraLooking.Instance.LockRotation = false;
            }

            PlayerUI.Instance.SetPauseMenuActive(paused && !dead);
            PlayerUI.Instance.SetDieMenuActive(dead);
        }
    }
    public bool Dead
    {
        get => dead;
        set
        {
            if (value == dead)
                return;

            dead = value;
            Paused = dead;
            health.Regeneration = !dead;
        }
    }

    private PlayerMovement movement;
    private PlayerBulletTime bulletTime;
    private PlayerHealth health;
    [SerializeField] private bool paused = false;
    [SerializeField] private bool dead = false;
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
        health = GetComponent<PlayerHealth>();


        health.OnHealthEnded.AddListener(Die);

        Paused = true;
        Paused = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Paused = !Paused;
        }
    }
    private void Die()
    {
        Dead = true;
    }
}
