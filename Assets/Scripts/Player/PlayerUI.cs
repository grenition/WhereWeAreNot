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
    [SerializeField] private CanvasGroup interactionPanelGroup;
    [SerializeField] private TMP_Text interactionPanelText;
    [SerializeField] private float diePostProccesingTransitionTime = 0.1f;
    [SerializeField] private float interactionGroupInterpolationMultiplier = 5f;

    private PostProccesingType savedPPType;
    private float interactionGroupTargetAlpha = 0f;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;

    }
    private void Start()
    {     
        SetPauseMenuActive(Player.Instance.Paused);
        SetDieMenuActive(Player.Instance.Dead);

        interactionPanelGroup.alpha = 0f;
        DisableInteractionPanel();
    }

    private void Update()
    {
        UpdateBulletTimeBar();
        UpdateInteractions();
    }
    private void UpdateBulletTimeBar()
    {
        if (bulletTimeBar == null || PlayerBulletTime.Instance == null)
            return;
        bulletTimeBar.fillAmount = PlayerBulletTime.Instance.TimePoints;
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

    public void EnableInteractionPanel(string interactionText)
    {
        if (interactionPanelText != null)
            interactionPanelText.text = interactionText;
        interactionGroupTargetAlpha = 1f;
    }
    public void DisableInteractionPanel()
    {
        interactionGroupTargetAlpha = 0f;
    }
    private void UpdateInteractions()
    {
        if (interactionPanelGroup == null)
            return;

        interactionPanelGroup.alpha = Mathf.Lerp(interactionPanelGroup.alpha, interactionGroupTargetAlpha, Time.unscaledDeltaTime * interactionGroupInterpolationMultiplier);
    }
}
