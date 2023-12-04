using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void VoidEventHandler();
public class Weapon : MonoBehaviour
{
    public Transform CameraParent { get => cameraParent; }

    [SerializeField] protected Transform cameraParent;

    public virtual void Open()
    {
        gameObject.SetActive(true);
    }
    public virtual void Close()
    {
        gameObject.SetActive(false);
    }
    public virtual void Remove()
    {
        Destroy(gameObject);
    }
}
