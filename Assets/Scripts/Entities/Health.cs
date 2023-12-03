using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Health : MonoBehaviour
{
    public float HealthPoints
    {
        get => currentHealth;
        set
        {

        }
    }

    [SerializeField] private float currentHealth;

    public UnityAction OnHealthChanged;
    public UnityAction OnHealthEnded;
}
