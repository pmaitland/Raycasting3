using UnityEngine;
using UnityEngine.AI;

public class Pathfinding : MonoBehaviour {

    private bool isPathfiding;
    private NavMeshAgent navMeshAgent;
    private NavMeshPath navMeshPath;

    private Transform target;

    void Start() {
        isPathfiding = false;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshPath = new NavMeshPath();
    }

    void Update() {
        if (isPathfiding) {
            navMeshAgent.CalculatePath(target.position, navMeshPath);
            navMeshAgent.SetPath(navMeshPath);
        } else {
            navMeshAgent.CalculatePath(transform.position, navMeshPath);
            navMeshAgent.SetPath(navMeshPath);
        }
    }

    public void SetIsPathfinding(bool value) {
        isPathfiding = value;
    }

    public void SetTarget(Transform transform) {
        target = transform;
    }
}
