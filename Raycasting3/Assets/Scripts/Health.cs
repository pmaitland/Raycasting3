using UnityEngine;

public class Health : MonoBehaviour {

    public int maxHealth;

    private int currentHealth;
    private bool destroyed;
    private GameObject killer;

    void Start() {
        currentHealth = maxHealth;
        destroyed = false;
    }

    public int GetCurrentHealth() {
        return currentHealth;
    }

    public GameObject GetKiller() {
        return killer;
    }

    public void ReduceHealth(GameObject attacker, int amount) {
        if (destroyed) return;
        currentHealth -= amount;
        if (currentHealth <= 0) {
            Destroy(GetComponent<Rigidbody>());
            if (GetComponent<UnityEngine.AI.NavMeshAgent>() != null) GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
            if (GetComponentInChildren<Collider>() != null) GetComponentInChildren<Collider>().enabled = false;
            if (GetComponentInChildren<MeshCollider>() != null) GetComponentInChildren<MeshCollider>().enabled = false;
            if (GetComponent<Pathfinding>() != null) GetComponent<Pathfinding>().enabled = false;
            destroyed = true;
            killer = attacker;
        }
    }
}
