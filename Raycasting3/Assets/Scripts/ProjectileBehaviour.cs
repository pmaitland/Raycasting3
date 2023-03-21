using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour {

    public float moveSpeed = 4.0f;

    private GameObject creator;

    void Update() {
        GetComponent<Rigidbody>().velocity = transform.forward * moveSpeed;

        if (Mathf.Abs(transform.position.x) > 100 || Mathf.Abs(transform.position.z) > 100) {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.transform.name == creator.name) return;
        
        Health otherHealth = other.GetComponentInParent<Health>();
        if (otherHealth != null) otherHealth.ReduceHealth(creator, 1);

        Destroy(gameObject);
    }

    public void SetCreator(GameObject c) {
        creator = c;
    }
}
