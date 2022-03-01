using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
            Destroy(GetComponent<Rigidbody>());
            if (GetComponent<NavMeshAgent>() != null) GetComponent<NavMeshAgent>().enabled = false;
            if (GetComponentInChildren<Collider>() != null) GetComponentInChildren<Collider>().enabled = false;
            if (GetComponentInChildren<MeshCollider>() != null) GetComponentInChildren<MeshCollider>().enabled = false;
            if (transform.Find("Sprite").GetComponent<MeshCollider>() != null) transform.Find("Sprite").GetComponent<MeshCollider>().enabled = false;
            destroyed = true;
        }
    }

    public bool IsDestroyed()
    {
        return destroyed;
    }

}
