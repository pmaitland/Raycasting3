using UnityEngine;

public class Health : MonoBehaviour {

    public int maxHealth;

    private int currentHealth;
    private bool destroyed;

    void Start() {
        currentHealth = maxHealth;
        destroyed = false;
    }

    public int GetCurrentHealth() {
        return currentHealth;
    }

    public void ReduceHealth(int amount) {
        if (destroyed) return;
        currentHealth -= amount;
        if (currentHealth <= 0) {
            Destroy(GetComponent<Rigidbody>());
            if (GetComponent<UnityEngine.AI.NavMeshAgent>() != null) GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
            if (GetComponentInChildren<Collider>() != null) GetComponentInChildren<Collider>().enabled = false;
            if (GetComponentInChildren<MeshCollider>() != null) GetComponentInChildren<MeshCollider>().enabled = false;
            if (transform.Find("Sprite").GetComponent<MeshCollider>() != null) transform.Find("Sprite").GetComponent<MeshCollider>().enabled = false;
            if (GetComponent<Pathfinding>() != null) GetComponent<Pathfinding>().enabled = false;
            destroyed = true;
        }
    }
}
