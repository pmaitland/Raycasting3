using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour {

    public float moveSpeed = 4.0f;

    void Update() {
        GetComponent<Rigidbody>().velocity = transform.forward * moveSpeed;
    }

    void OnTriggerEnter(Collider other) {
        Health otherHealth = other.GetComponentInParent<Health>();
        if (otherHealth != null) otherHealth.ReduceHealth(1);

        Destroy(gameObject);
    }
}
