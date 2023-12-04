using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UI_ButtonSoundPlayer : MonoBehaviour
{
    private Button button;
    private void OnEnable()
    {
        if (button == null)
            button = GetComponent<Button>();
        if(button != null)
            button.onClick.AddListener(PlayButtonSound);
    }
    private void OnDisable()
    {
        if(button != null)
            button.onClick.RemoveListener(PlayButtonSound);
    }

    public void PlayButtonSound()
    {
        AudioController.PlayButtonSound();
    }
}
