using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DynamicGravityTrigger : EntityTrigger
{
    public bool IsPreview
    {
        get => isPreview;
        set
        {
            isPreview = value;
            TriggerEnabled = !value;
        }
    }
    public Vector3 BoxScale { get => boxScale; }
    public bool IsInCorner { get; private set; }

    [Header("Important")]
    [SerializeField] private bool destroyAfterEntityEntered = false;

    [Header("Optional")]
    [SerializeField] private float checkingCornersDistance = 0.5f;
    [SerializeField] private LayerMask layerMask = 1 << 0;
    [SerializeField] private float gravityLockTimeWhileFlipping = 0.5f;
    [SerializeField] private GameObject[] flatPlaneObjects;
    [SerializeField] private GameObject[] cornerObjects;
   
    private BoxCollider boxCollider;
    private Vector3 boxScale = Vector3.one;
    private Vector3 boxOffset = Vector3.zero;
    private Vector3 raycastDirection = Vector3.zero;
    private bool isPreview = false;
    
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        #region setting values
        boxScale = new Vector3
        {
            x = transform.lossyScale.x * boxCollider.size.x,
            y = transform.lossyScale.y * boxCollider.size.y,
            z = transform.lossyScale.z * boxCollider.size.z
        };
        boxScale = transform.TransformDirection(boxScale);
        boxScale.x = Mathf.Abs(boxScale.x);
        boxScale.y = Mathf.Abs(boxScale.y);
        boxScale.z = Mathf.Abs(boxScale.z);

        boxOffset = new Vector3
        {
            x = transform.lossyScale.x * boxCollider.center.x,
            y = transform.lossyScale.y * boxCollider.center.y,
            z = transform.lossyScale.z * boxCollider.center.z
        };
        boxOffset = transform.TransformDirection(boxOffset);
        #endregion
    }
    private void Start()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, boxScale / 2f);
        foreach (var col in colliders)
        {
            if (col.gameObject == gameObject)
                return;
            if (col.TryGetComponent(out DynamicGravityTrigger trig))
            {
                if(!trig.IsPreview)
                    Destroy(col.gameObject);
            }
        }
    }
    private void Update()
    {
        if (!transform.hasChanged)
            return;

        IsInCorner = OverlapBoxCheck();
        foreach (var obj in flatPlaneObjects)
            obj.SetActive(!IsInCorner);
        foreach (var obj in cornerObjects)
            obj.SetActive(IsInCorner);

        transform.hasChanged = false;
    }
    [SerializeField] private Vector3 newGravityDirection = Vector3.up;
    protected override void OnEntityEnter(Entity _entity)
    {
        if (!IsInCorner)
        {
            _entity.CurrentWalker.GravityFollowTargetRotationNormalForTime(gravityLockTimeWhileFlipping);
            _entity.CurrentWalker.SetTargetRotation(-transform.forward);
        }
        else
        {
            _entity.CurrentWalker.SetTargetRotation(-GetRaycastDirection(_entity));
        }

        if(destroyAfterEntityEntered)
            Destroy(gameObject);
    }

    private Vector3 GetRaycastDirection(Entity _entity)
    {
        Vector3 direction = -transform.forward;
        if (Vector3.Angle(-_entity.transform.up, raycastDirection) > 85f)
            return direction = raycastDirection;
        return direction;
    }
    private bool OverlapBoxCheck()
    {
        Vector3 center = transform.position;

        bool raycastUp = Physics.Raycast(center, transform.up, boxScale.y / 2f + checkingCornersDistance, layerMask);
        bool raycastDown = Physics.Raycast(center, -transform.up, boxScale.y / 2f + checkingCornersDistance, layerMask);
        bool raycastRight = Physics.Raycast(center, transform.right, boxScale.x / 2f + checkingCornersDistance, layerMask);
        bool raycastLeft = Physics.Raycast(center, -transform.right, boxScale.x / 2f + checkingCornersDistance, layerMask);
        bool raycastForward = Physics.Raycast(center, transform.forward, boxScale.z / 2f + checkingCornersDistance, layerMask);

        if (raycastUp)
            raycastDirection = transform.up;
        if (raycastDown)
            raycastDirection = -transform.up;
        else if (raycastRight)
            raycastDirection = transform.right;
        else if (raycastLeft)
            raycastDirection = -transform.right;
        else if (raycastForward)
            raycastDirection = transform.forward;

        //Collider[] colliders = Physics.OverlapBox(transform.position + boxOffset, boxScale / 2f, transform.rotation);
        //foreach(var col in colliders)
        //{
        //    if (col.GetComponent<Entity>() == null && col.gameObject != gameObject)
        //        return true;
        //}
        return raycastUp || raycastDown || raycastRight || raycastLeft || raycastForward;
    }
    public void SetPositionByPointOnPlane(Vector3 point)
    {
        transform.position = point + transform.forward * boxScale.z / 2f;
    }
    public Vector3 GetCenterPosition()
    {
        return transform.position + transform.forward * boxScale.z / 2f;
    }
}
