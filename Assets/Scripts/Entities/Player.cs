using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerMovement))]
public class Player : Entity
{
    public static Player Instance { get; private set; }
    public PlayerMovement Movement { get => movement; }

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
                if (PlayerBulletTime.Instance != null)
                    PlayerBulletTime.Instance.enabled = false;
                Time.timeScale = 0f;
                CameraLooking.Instance.ShowCursor = true;
                CameraLooking.Instance.LockRotation = true;
            }
            else
            {
                Time.timeScale = 1f;
                if (PlayerBulletTime.Instance != null)
                    PlayerBulletTime.Instance.enabled = true;
                CameraLooking.Instance.ShowCursor = false;
                CameraLooking.Instance.LockRotation = false;
            }

            if (PlayerUI.Instance != null)
            {
                PlayerUI.Instance.SetPauseMenuActive(paused && !dead);
                PlayerUI.Instance.SetDieMenuActive(dead);
            }
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
            if (PlayerHealth.Instance != null)
                PlayerHealth.Instance.Regeneration = !dead;
        }
    }

    [SerializeField] private PlayerMovement movement;
    [SerializeField] private GravityWeapon gravityWeapon;
    [SerializeField] private bool paused = false;
    [SerializeField] private bool dead = false;
    protected override void Awake()
    {
        base.Awake();
        movement = GetComponent<PlayerMovement>();

        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

    }
    private void Start()
    {
        if(PlayerHealth.Instance != null)
            PlayerHealth.Instance.OnHealthEnded.AddListener(Die);
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
    
    public void HideGravityWeapon()
    {
        if(gravityWeapon != null)
        gravityWeapon.gameObject.SetActive(false);
    }
    public void ShowGravityWeapon()
    {
        if(gravityWeapon != null)
            gravityWeapon.gameObject.SetActive(true);
    }
}
