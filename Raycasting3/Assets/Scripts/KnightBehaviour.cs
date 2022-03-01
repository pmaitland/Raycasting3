using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KnightBehaviour : MonoBehaviour
{
    enum State { Chasing, Stopped, Destroyed }

    private Transform player;

    private MazeBehaviour.MazeGrid maze;

    private HealthBehaviour healthBehaviour;

    private State currentState = State.Chasing;

    private NavMeshAgent navMeshAgent;
    private NavMeshPath navMeshPath;

    void Start()
    {
        healthBehaviour = GetComponentInParent<HealthBehaviour>();

        navMeshAgent = GetComponentInParent<NavMeshAgent>();
        navMeshPath = new NavMeshPath();
    }

    void Update()
    {
        if (healthBehaviour.IsDestroyed()) currentState = State.Destroyed;

        float distanceToPlayer = Vector3.Distance(transform.parent.position, player.position);

        switch (currentState) {
            case State.Chasing:
                if (distanceToPlayer <= 1) {
                    currentState = State.Stopped;
                } else {
                    navMeshAgent.CalculatePath(player.position, navMeshPath);
                    navMeshAgent.SetPath(navMeshPath);
                }
                break;
            case State.Stopped:
                if (distanceToPlayer > 1) currentState = State.Chasing;
                break;
            case State.Destroyed:
                break;
            default:
                break;
        }
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player.transform;
    }

    public void SetMaze(MazeBehaviour.MazeGrid maze)
    {
        this.maze = maze;
    }

}
