using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance;

    [Header("UI Elements")]
    [SerializeField] private Image bulletTimeBar;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject[] disablingObjectOnPauseActivation;
    [SerializeField] private GameObject diePanel;
    [SerializeField] private float diePostProccesingTransitionTime = 0.1f;

    private PostProccesingType savedPPType;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

    }
    private void Start()
    {     
        SetPauseMenuActive(Player.Instance.Paused);
        SetDieMenuActive(Player.Instance.Dead);
    }

    private void Update()
    {
        UpdateBulletTimeBar();
    }
    private void UpdateBulletTimeBar()
    {
        if (bulletTimeBar == null || Player.Instance.BulletTime == null)
            return;
        bulletTimeBar.fillAmount = Player.Instance.BulletTime.TimePoints;
    }

    public void Unpause()
    {
        Player.Instance.Paused = false;
    }
    public void ExitMenu()
    {

    }
    public void SetPauseMenuActive(bool value)
    {
        if (pausePanel.activeSelf == value)
            return;

        pausePanel.SetActive(value);
        if (value)
            foreach (var obj in disablingObjectOnPauseActivation)
                obj.SetActive(false);
    }
    public void SetDieMenuActive(bool value)
    {
        if (diePanel.activeSelf == value)
            return;

        diePanel.SetActive(value);

        savedPPType = GlobalPostProcessing.Instance.CurrentPPType;
        if (value == false)
            GlobalPostProcessing.SetProfile(savedPPType);
        else
            GlobalPostProcessing.SetProfile(PostProccesingType.die, diePostProccesingTransitionTime);
    }
    public bool GetPauseMenuActiveSelf()
    {
        return pausePanel.activeSelf;
    }
    public void Respawn()
    {
        RespawnController.Respawn();
    }
}
