using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerHealth : Health
{
    public static PlayerHealth Instance { get; private set; }

    [SerializeField] private float regeneratingHealthsPerSecond = 10f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    private void Update()
    {
        HealthPoints += regeneratingHealthsPerSecond * Time.deltaTime;
        GlobalPostProcessing.SetHealthVolumeWeight(1 - HealthPointsProportion);
    }
}
