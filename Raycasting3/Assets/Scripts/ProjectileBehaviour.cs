using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public float moveSpeed = 4.0f;

    void Update()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * moveSpeed;
    }

    void OnTriggerEnter(Collider other)
    {
        HealthBehaviour otherHealth = other.GetComponentInParent<HealthBehaviour>();
        if (otherHealth != null) otherHealth.ReduceHealth(1);

        Destroy(gameObject);
    }
}
