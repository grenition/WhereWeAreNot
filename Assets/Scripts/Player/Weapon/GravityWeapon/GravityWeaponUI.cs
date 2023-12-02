using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GravityWeaponUI : MonoBehaviour
{
    [SerializeField] private GravityWeapon gravityWeapon;

    [Header("UI")]
    [SerializeField] private TMP_Text weaponModeText;

    private void OnEnable()
    {
        if (gravityWeapon == null)
        {
            enabled = false;
            return;
        }

        gravityWeapon.OnWorkModeChanged += GravityWeapon_OnWorkModeChanged;
    }
    private void Start()
    {
        GravityWeapon_OnWorkModeChanged(gravityWeapon.WorkMode);
    }

    private void GravityWeapon_OnWorkModeChanged(GravityWeaponWorkMode workMode)
    {
        string text = "Work mode: ";
        if (workMode == GravityWeaponWorkMode.ChangingGravity)
            text += "Changing gravity";
        else if (workMode == GravityWeaponWorkMode.Points)
            text += "Placing points";
        weaponModeText.text = text;
    }

    private void OnDisable()
    {
        if (gravityWeapon == null)
            return;
    }
}
