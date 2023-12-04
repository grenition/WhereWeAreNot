using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ScreenResolutionEventHandler(ScreenResolution oldResolution, ScreenResolution newResolution);
[System.Serializable]
public struct ScreenResolution
{
    public int widht;
    public int height;

    public ScreenResolution(int _widht, int _height)
    {
        widht = _widht;
        height = _height;
    }
}
public class CameraLooking : MonoBehaviour
{
    public static CameraLooking Instance { get; private set; }
    public Transform CameraTransform
    {
        get
        {
            if (cameraTr != null)
                return cameraTr;
            else 
                return transform;
        }
    }

    [SerializeField] private float sensivity = 1f;
    [Range(0f, 90f)] [SerializeField] private float maxVerticalAngle = 90f;
    [Range(0f, 90f)] [SerializeField] private float minVerticalAngle = 90f;
    [Range(0f, 360f)] [SerializeField] private float rightMaxHorizontalAngle = 360f;
    [Range(0f, 360f)] [SerializeField] private float leftMaxHoriontalAngle = 360f;


    [Header("Optional")]
    [SerializeField] private Camera viewCamera;

    [Header("Apply if want to control head")]
    [SerializeField] private Walker walker;
    [SerializeField] private float verticalPositionOffset = 0.1f;

    public static Camera ViewCamera
    {
        get
        {
            if (Instance != null)
                return Instance.viewCamera;
            return null;
        }
    }
    public static float FOV
    {
        get
        {
            if (ViewCamera != null)
                return ViewCamera.fieldOfView;
            return 0;
        }
        set
        {
            if (ViewCamera != null)
                ViewCamera.fieldOfView = value;
        }
    }
    public bool ShowCursor
    {
        get => cursorIsVisible;
        set
        {
            Cursor.visible = value;
            Cursor.lockState = CursorLockMode.None;
            if (!value)
                Cursor.lockState = CursorLockMode.Locked;
            cursorIsVisible = value;
        }
    }
    public bool LockRotation { get; set; }
    public Vector3 Rotation { get; set; }
    public Vector2 MouseDelta { get => mouseDelta; }

    private Vector2 mouseDelta = Vector2.zero;
    private bool cursorIsVisible = false;
    private Transform tr;
    private Transform walkerTr;
    private Transform cameraTr;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        tr = transform;
        if (walker != null)
            walkerTr = walker.transform;
        cameraTr = viewCamera.transform;
    }
    private void OnEnable()
    {
        Rotation = transform.localEulerAngles;
        ShowCursor = false;
    }
    private void OnDisable()
    {
        ShowCursor = true;
    }
    private void Update()
    {
        if (walker != null)
        {
            tr.position = walkerTr.position + walker.Center + walkerTr.up * (walker.ColliderHeight / 2 - verticalPositionOffset);
        }
     
        if (LockRotation)
            return;

        mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        mouseDelta *= sensivity;

        Rotation += new Vector3(-mouseDelta.y, mouseDelta.x, 0f);

        Rotation = LoopEulers(Rotation);
        Rotation = ClampEulers(Rotation);

        tr.localEulerAngles = Rotation;
    }
    private Vector3 ClampEulers(Vector3 eulers)
    {
        return new Vector3
        {
            x = Mathf.Clamp(eulers.x, -maxVerticalAngle, minVerticalAngle),
            y = Mathf.Clamp(eulers.y, -leftMaxHoriontalAngle, rightMaxHorizontalAngle),
            z = eulers.z
        };
    }
    private Vector3 LoopEulers(Vector3 eulers)
    {
        return new Vector3
        {
            x = LoopMagnitude(eulers.x),
            y = LoopMagnitude(eulers.y),
            z = LoopMagnitude(eulers.z)
        };
    }
    private float LoopMagnitude(float value)
    {
        if (value >= 360f)
            value -= 360f;
        else if (value <= -360f)
            value += 360f;
        return value;
    }

    public void SetTransformParent(Transform _parent)
    {
        tr.SetParent(_parent);
    }

    public void AddRotation(Vector2 _rotation)
    {
        Rotation += new Vector3(-_rotation.y, _rotation.x, 0f);
    }
}
