using UnityEngine;
using UnityEngine.AI;

public class KnightBehaviour : MonoBehaviour {

    private enum State {
        CHASING,
        STOPPED,
        DESTROYED
    }

    private State currentState = State.CHASING;
    
    private Health health;
    private Pathfinding pathfinding;

    private Transform player;

    void Start() {
        health = GetComponentInParent<Health>();

        pathfinding = GetComponentInParent<Pathfinding>();
        pathfinding.SetTarget(player);
        pathfinding.SetIsPathfinding(true);
    }

    void Update() {
        if (health.GetCurrentHealth() <= 0) currentState = State.DESTROYED;

        float distanceToPlayer = Vector3.Distance(transform.parent.position, player.position);

        switch (currentState) {
            case State.CHASING:
                if (distanceToPlayer <= 1) {
                    currentState = State.STOPPED;
                    pathfinding.SetIsPathfinding(false);
                }
                break;
            case State.STOPPED:
                if (distanceToPlayer > 1) {
                    currentState = State.CHASING;
                    pathfinding.SetIsPathfinding(true);
                }
                break;
            case State.DESTROYED:
                pathfinding.SetIsPathfinding(false);
                break;
            default:
                break;
        }
    }

    public void SetPlayer(GameObject player) {
        this.player = player.transform;
    }
}
