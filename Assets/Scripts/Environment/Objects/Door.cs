using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private AudioSource source;

    [SerializeField] private AudioClip openClip;
    [SerializeField] private AudioClip closeClip;
    [SerializeField] private AudioClip handleClip;

    private bool isOpened = false;
    public bool isLocked = false;


    public void OpenDoor()
    {
        if(!isOpened && isLocked)
        {
            if (source != null)
                source.PlayOneShot(handleClip);
        }
        if (isOpened || isLocked)
            return;

        if (anim != null)
            anim.SetBool("Opened", true);
        if (source != null && openClip != null)
            source.PlayOneShot(openClip);

        isOpened = true;
    }
    public void CloseDoor()
    {
        if (!isOpened || isLocked)
            return;

        if (anim != null)
            anim.SetBool("Opened", false);
        if (source != null && closeClip != null)
            source.PlayOneShot(closeClip);

        isOpened = false;
    }
    public void ChangeDoorOpenedState()
    {
        if (isOpened)
            CloseDoor();
        else
            OpenDoor();
    }
    public void UnlockDoor()
    {
        isLocked = false;
    }
    public void LockDoor()
    {
        isLocked = true;
    }
}
