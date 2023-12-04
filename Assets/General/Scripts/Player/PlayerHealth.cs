using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerHealth : Health
{
    public static PlayerHealth Instance { get; private set; }
    public bool Regeneration { get => regeneration; set { regeneration = value; } }

    [SerializeField] private float regeneratingHealthsPerSecond = 10f;
    [SerializeField] private bool regeneration = true;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    private void Update()
    {
        GlobalPostProcessing.SetHealthVolumeWeight(1 - HealthPointsProportion);

        if (regeneration)
            HealthPoints += regeneratingHealthsPerSecond * Time.deltaTime;
    }
}
