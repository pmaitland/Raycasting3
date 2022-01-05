using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBehaviour : MonoBehaviour
{

    public int maxHealth;

    private int currentHealth;
    private bool destroyed;

    void Start()
    {
        currentHealth = maxHealth;
        destroyed = false;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void ReduceHealth(int amount)
    {
        if (destroyed) return;
        currentHealth -= amount;
        if (currentHealth <= 0) {
            GetComponentInChildren<Collider>().enabled = false;
            destroyed = true;
        }
    }

}
