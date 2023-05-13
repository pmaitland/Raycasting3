using UnityEngine;

public class Health : MonoBehaviour
{

    public int MAX_MAX_HEALTH { get; private set; } = 20;
    public int MaxHealth;

    public int CurrentHealth { get; private set; }
    private bool _destroyed = false;
    public GameObject Killer { get; private set; }

    void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public void IncreaseHealth(int amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;
    }

    public void IncreaseMaxHealth(int amount)
    {
        MaxHealth += amount;
        if (MaxHealth > MAX_MAX_HEALTH) MaxHealth = MAX_MAX_HEALTH;
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;
    }

    public void ReduceHealth(GameObject attacker, int amount)
    {
        if (_destroyed) return;

        CurrentHealth -= amount;
        if (GetComponent<PlayerBehaviour>() != null) GetComponent<PlayerBehaviour>().FlashDamage();

        if (CurrentHealth <= 0)
        {
            Destroy(GetComponent<Rigidbody>());
            if (GetComponent<UnityEngine.AI.NavMeshAgent>() != null) GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
            if (GetComponentInChildren<Collider>() != null) GetComponentInChildren<Collider>().enabled = false;
            if (GetComponentInChildren<MeshCollider>() != null) GetComponentInChildren<MeshCollider>().enabled = false;
            if (GetComponent<Pathfinding>() != null) GetComponent<Pathfinding>().enabled = false;
            _destroyed = true;
            Killer = attacker;
        }
    }
}
