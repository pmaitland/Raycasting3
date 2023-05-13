using UnityEngine;
using UnityEngine.AI;

public class Pathfinding : MonoBehaviour
{

    public bool IsPathfinding { get; set; }
    public Transform Target { get; set; }

    private NavMeshAgent _navMeshAgent;
    private NavMeshPath _navMeshPath;

    void Start()
    {
        IsPathfinding = false;

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshPath = new();
    }

    void Update()
    {
        if (IsPathfinding)
        {
            _navMeshAgent.CalculatePath(Target.position, _navMeshPath);
        }
        else
        {
            _navMeshAgent.CalculatePath(transform.position, _navMeshPath);
        }

        _navMeshAgent.SetPath(_navMeshPath);
    }

}
