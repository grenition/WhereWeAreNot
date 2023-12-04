using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public delegate void GravityWeaponWorkModeEventHandler(GravityWeaponWorkMode workMode);
public enum GravityWeaponWorkMode
{
    Points,
    ChangingGravity
}
[System.Serializable]
public class PhysicsPair
{
    public PhysicsPoint point1;
    public PhysicsPoint point2;

    private LineRenderer currentLine;

    public void InitializeLineBetweenPoints(LineRenderer prefab)
    {
        if (IsEmpty())
            return;

        if (currentLine == null)
            currentLine = UnityEngine.Object.Instantiate(prefab);
        currentLine.SetPositions(new Vector3[] { point1.transform.position, point2.transform.position });
    }
    public void UpdateLineBetweenPoints()
    {
        if (IsEmpty())
            return;

        if (currentLine == null)
            return;
        currentLine.SetPositions(new Vector3[] { point1.transform.position, point2.transform.position });
    }
    public bool IsEmpty()
    {
        return point1 == null || point2 == null;
    }
    public bool IsHaveSameConnections()
    {
        return point1.ConnectedBody == point2.ConnectedBody;
    }
    public bool IsHaveConnection()
    {
        return point1.IsConnected || point2.IsConnected;
    }
    public void DestroyPair()
    {
        if (IsEmpty())
            return;
        point1.DestroyPoint();
        point2.DestroyPoint();
        if (currentLine != null)
            UnityEngine.Object.Destroy(currentLine);
    }
    public void StartPointsAttraction()
    {
        point1.MoveToOtherPoint(point2);
        point2.MoveToOtherPoint(point1);

        point1.OnPointCycleEnded += DestroyPair;
    }
    public PhysicsPair(PhysicsPoint _point1, PhysicsPoint _point2)
    {
        point1 = _point1;
        point2 = _point2;
    }
}
public class GravityWeapon : Weapon
{
    public GravityWeaponWorkMode WorkMode { get => workMode; }

    [Header("Placing points")]
    [SerializeField] private int maxPairsCount = 5;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private float bulletsSpeed = 30f;
    [SerializeField] private Transform bulletsOut;
    [SerializeField] private LineRenderer lineBetweenPointsPrefab;

    [Header("Changing gravitation")]
    [SerializeField] private int maxTriggersCount = 3;
    [SerializeField] private DynamicGravityTrigger gravityTriggerPrefab;
    [SerializeField] private Transform barrelEnd;
    [SerializeField] private float maxDistance = 100f;

    [Header("Delays")]
    [SerializeField] private float openTime = 0.3f;
    [SerializeField] private float shootDelay = 0.2f;
    [SerializeField] private float changingModeDelay = 0.2f;

    [Header("Keys")]
    [SerializeField] private KeyCode changeModeKey = KeyCode.E;
    [SerializeField] private KeyCode startPointsAttractionKey = KeyCode.F;
    [SerializeField] private KeyCode resetPointsKey = KeyCode.R;

    private float blockTime = 0f;
    private DynamicGravityTrigger tempGravityTrigger;
    private float radius = 1f;
    private List<DynamicGravityTrigger> spawnedTriggers = new List<DynamicGravityTrigger>();
    private GravityWeaponWorkMode workMode = GravityWeaponWorkMode.ChangingGravity;
    [SerializeField] private List<PhysicsPoint> physicsPoints = new List<PhysicsPoint>();
    [SerializeField] private List<PhysicsPair> physicsPairs = new List<PhysicsPair>();

    public event GravityWeaponWorkModeEventHandler OnWorkModeChanged;
    public Action OnPointsModeShooted;
    public Action OnGravityChangingModeShooted;

    private void Start()
    {
        tempGravityTrigger = Instantiate(gravityTriggerPrefab);
        tempGravityTrigger.IsPreview = true;

        radius = tempGravityTrigger.BoxScale.z / 2f + 0.01f;
    }
    private void OnEnable()
    {
        blockTime = Time.realtimeSinceStartup + changingModeDelay;
    }
    void Update()
    {
        if (Player.Instance.Paused)
            return;

        if (Input.GetKeyDown(changeModeKey))
            ChangeGravityMode();
        else if (Input.GetKeyDown(startPointsAttractionKey))
            StartPairsAttraction();
        else if (Input.GetKeyDown(resetPointsKey))
            DestroyPoints();
        UpdateLinesInPairs();
        if (workMode == GravityWeaponWorkMode.Points)
        {
            tempGravityTrigger.gameObject.SetActive(false);
            if(Input.GetKeyDown(KeyCode.Mouse0))
                ShootPhysicsPoint();
        }
        else
        {
            if (Physics.SphereCast(transform.position, radius, transform.forward, out RaycastHit hit, maxDistance))
            {
                if (hit.collider.TryGetComponent(out Surface surf) && surf.GravityGunSurface)
                {
                    tempGravityTrigger.gameObject.SetActive(true);

                    tempGravityTrigger.transform.rotation = Quaternion.LookRotation(hit.normal, Vector3.forward);
                    tempGravityTrigger.SetPositionByPointOnPlane(hit.point);
                }
                else
                {
                    tempGravityTrigger.gameObject.SetActive(false);
                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
                PlaceChangingGravityCube();
        }
    }
    private void PlaceChangingGravityCube()
    {
        if (gravityTriggerPrefab == null || Time.realtimeSinceStartup < blockTime)
            return;
        if(Physics.SphereCast(transform.position, radius, transform.forward, out RaycastHit hit, maxDistance))
        {
            if (hit.collider.TryGetComponent(out Surface surf) && surf.GravityGunSurface)
            {
                DynamicGravityTrigger trigger = Instantiate(gravityTriggerPrefab, hit.point, Quaternion.LookRotation(hit.normal, Vector3.forward));
                trigger.SetPositionByPointOnPlane(hit.point);

                List<DynamicGravityTrigger> tempList = new List<DynamicGravityTrigger>();
                foreach (var j in spawnedTriggers)
                {
                    if (j != null)
                        tempList.Add(j);
                }
                spawnedTriggers = tempList;

                if (spawnedTriggers.Count >= maxTriggersCount)
                {
                    var temp = spawnedTriggers[0].gameObject;
                    spawnedTriggers.RemoveAt(0);
                    Destroy(temp);
                }
                spawnedTriggers.Add(trigger);

                OnGravityChangingModeShooted?.Invoke();
                blockTime = Time.realtimeSinceStartup + shootDelay;
            }
        }
    }
    private void ShootPhysicsPoint()
    {
        if (bulletPrefab == null || CameraLooking.Instance.CameraTransform == null || Time.realtimeSinceStartup < blockTime)
            return;
        Transform cam = CameraLooking.Instance.CameraTransform;
        Bullet bul = Instantiate(bulletPrefab, bulletsOut.position, bulletsOut.rotation);
        bul.AddForce(cam.forward * bulletsSpeed);
        bul.OnPhysicsPointSpawned += OnPhysicsPointPlaced;

        OnPointsModeShooted?.Invoke();
        blockTime = Time.realtimeSinceStartup + shootDelay;
    }
    private void ChangeGravityMode()
    {
        if (workMode == GravityWeaponWorkMode.Points)
            workMode = GravityWeaponWorkMode.ChangingGravity;
        else if (workMode == GravityWeaponWorkMode.ChangingGravity)
            workMode = GravityWeaponWorkMode.Points;
        OnWorkModeChanged?.Invoke(workMode);
        blockTime = Time.realtimeSinceStartup + changingModeDelay;
    }
    private void OnPhysicsPointPlaced(PhysicsPoint point)
    {
        if (point == null)
            return;

        CheckLists();

        point.Initialize();
        physicsPoints.Add(point);
        if(physicsPoints.Count >= 2)
        {
            PhysicsPair pair = new PhysicsPair(physicsPoints[0], physicsPoints[1]);

            if (!pair.IsHaveConnection() || pair.IsHaveSameConnections())
            {
                physicsPoints.Clear();
                pair.DestroyPair();
                return; 
            }
            
            CheckLists();
            if (physicsPairs.Count >= maxPairsCount)
            {
                physicsPairs[0].DestroyPair();
                physicsPairs.RemoveAt(0);
            }

            pair.InitializeLineBetweenPoints(lineBetweenPointsPrefab);
            physicsPairs.Add(pair);
            physicsPoints.Clear();
        }
    }
    private void UpdateLinesInPairs()
    {
        foreach(var pair in physicsPairs)
        {
            pair.UpdateLineBetweenPoints();
        }
    }
    private void StartPairsAttraction()
    {
        CheckLists();
        foreach (var pair in physicsPairs)
        {
            pair.StartPointsAttraction();
        }
    }
    private void CheckLists()
    {
        CheckPairsList();
        CheckPointsList();
    }
    private void CheckPairsList()
    {
        List<PhysicsPair> tempList = new List<PhysicsPair>();
        foreach (var pair in physicsPairs)
        {
            if (!pair.IsEmpty())
                tempList.Add(pair);
        }
        physicsPairs = tempList;
    }
    private void CheckPointsList()
    {
        List<PhysicsPoint> tempList = new List<PhysicsPoint>();
        foreach (var point in physicsPoints)
        {
            if (point != null)
                tempList.Add(point);
        }
        physicsPoints = tempList;
    }
    private void DestroyPoints()
    {
        CheckLists();
        foreach (var point in physicsPoints)
            point.DestroyPoint();
        foreach (var pair in physicsPairs)
            pair.DestroyPair();
        CheckLists();
    }
}
