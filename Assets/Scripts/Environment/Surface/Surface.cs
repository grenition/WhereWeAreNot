using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surface : MonoBehaviour
{
    public bool GravityGunSurface { get => gravityGunSurface; }
    public bool RotationGroundSurface { get => rotationGroundSurface; }

    [SerializeField] private SurfaceResources surfaceResources;
    [SerializeField] private bool gravityGunSurface = false;
    [SerializeField] private bool rotationGroundSurface = false;
    public SurfaceResources Resources { get => surfaceResources; private set { surfaceResources = value; } }

    private void Awake()
    {
        Resources = surfaceResources;
    }
}
