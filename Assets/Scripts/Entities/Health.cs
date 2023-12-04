using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Health : MonoBehaviour
{
    public float HealthPointsProportion
    {
        get => HealthPoints / maxHealthPoints;
    }
    public float HealthPoints
    {
        get => currentHealth;
        set
        {
            value = Math.Clamp(value, 0f, maxHealthPoints);
            if (value == currentHealth)
                return;
            currentHealth = Math.Clamp(value, 0f, maxHealthPoints);
            OnHealthChanged?.Invoke();
            if (currentHealth == 0f)
                OnHealthEnded?.Invoke();
            if (currentHealth == maxHealthPoints)
                OnRegenerate?.Invoke();
        }
    }

    [SerializeField] private float currentHealth = 100f;
    [SerializeField] private float maxHealthPoints = 100f;


    public UnityEvent OnHealthChanged;
    public UnityEvent OnHealthEnded;
    public UnityEvent OnRegenerate;

    public void Regenerate()
    {
        HealthPoints = maxHealthPoints;
    }
    public void Die()
    {
        HealthPoints = 0f;
    }
    public virtual void Damage(float damage)
    {
        HealthPoints -= damage;
    }
}
