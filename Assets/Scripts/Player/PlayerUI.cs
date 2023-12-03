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

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
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
}
