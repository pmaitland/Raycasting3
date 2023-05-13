using UnityEngine;

public class KnightBehaviour : MonoBehaviour
{

    private enum State
    {
        CHASING,
        STOPPED,
        DESTROYED
    }

    private Health _health;
    private Pathfinding _pathfinding;

    private State _currentState = State.CHASING;
    public Transform Target { get; set; }

    void Start()
    {
        _health = GetComponentInParent<Health>();

        _pathfinding = GetComponentInParent<Pathfinding>();
        _pathfinding.Target = Target;
        _pathfinding.IsPathfinding = true;
    }

    void Update()
    {
        if (_health.CurrentHealth <= 0) _currentState = State.DESTROYED;

        float distanceToTarget = Vector3.Distance(transform.parent.position, Target.position);

        switch (_currentState)
        {
            case State.CHASING:
                if (distanceToTarget <= 1)
                {
                    _currentState = State.STOPPED;
                    _pathfinding.IsPathfinding = false;
                }
                break;
            case State.STOPPED:
                if (distanceToTarget > 1)
                {
                    _currentState = State.CHASING;
                    _pathfinding.IsPathfinding = true;
                }
                break;
            case State.DESTROYED:
                _pathfinding.IsPathfinding = false;
                break;
            default:
                break;
        }
    }

}
