using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    public static PlayerInteractions Instance;

    [SerializeField] private float detectingDistance = 1f;

    private float savedObjectDetectionTime = 0;
    private GameObject savedObject;

    private Interaction currentInteracttion;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    private void Update()
    {
        if (CameraLooking.ViewCamera == null)
            return;

        DetectCurrentInteraction();

        if (currentInteracttion != null && Input.GetKeyDown(KeyCode.G))
            currentInteracttion.Interact();
    }
    private void DetectCurrentInteraction()
    {
        if (Physics.Raycast(CameraLooking.ViewCamera.transform.position, CameraLooking.ViewCamera.transform.forward, out RaycastHit hit, detectingDistance))
        {
            if (hit.collider.gameObject != savedObject)
            {
                if (hit.collider.TryGetComponent(out Interaction interaction) && interaction.IsInteractable)
                {
                    currentInteracttion = interaction;
                    if (PlayerUI.Instance != null)
                        PlayerUI.Instance.EnableInteractionPanel(interaction.InteractionText);
                }
                else
                {
                    currentInteracttion = null;
                    if (PlayerUI.Instance != null)
                        PlayerUI.Instance.DisableInteractionPanel();
                }
            }
            savedObject = hit.collider.gameObject;

        }
        else
        {
            currentInteracttion = null;
            savedObject = null;
            if (PlayerUI.Instance != null)
                PlayerUI.Instance.DisableInteractionPanel();
        }
    }
}
