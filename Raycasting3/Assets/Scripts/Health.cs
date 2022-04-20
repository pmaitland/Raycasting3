using UnityEngine;

public class Health : MonoBehaviour {

    private const int MAX_MAX_HEALTH = 20;
    public int maxHealth;

    private int currentHealth;
    private bool destroyed;
    private GameObject killer;

    void Start() {
        currentHealth = maxHealth;
        destroyed = false;
    }

    public int GetMaxHealth() {
        return maxHealth;
    }

    public int GetMaxMaxHealth() {
        return MAX_MAX_HEALTH;
    }

    public int GetCurrentHealth() {
        return currentHealth;
    }

    public GameObject GetKiller() {
        return killer;
    }

    public void IncreaseHealth(int amount) {
        currentHealth += amount;
    }

    public void IncreaseMaxHealth(int amount) {
        maxHealth += amount;
        currentHealth += amount;
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
