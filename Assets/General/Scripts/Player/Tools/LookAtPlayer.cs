using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField] private bool rotateAroundPlayerVerticalAxis = false;
    [SerializeField] private bool rotateAroundMineVerticalAxis = false;
    private void Update()
    {
        if (CameraLooking.ViewCamera == null || Player.Instance == null)
            return;

        Vector3 dir = CameraLooking.ViewCamera.transform.forward;
        Vector3 verticalAxis = Vector3.up;
        if (rotateAroundPlayerVerticalAxis)
        {
            dir = VectorMathf.RemoveDotVector(dir, Player.Instance.transform.up);
            verticalAxis = Player.Instance.transform.up;
        }
        else if (rotateAroundMineVerticalAxis)
        {
            dir = VectorMathf.RemoveDotVector(dir, transform.up);
            verticalAxis = transform.up;
        }
        transform.rotation = Quaternion.LookRotation(dir, verticalAxis);
    }
}
