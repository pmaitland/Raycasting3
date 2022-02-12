using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightBehaviour : MonoBehaviour
{
    enum State { Chasing, Stopped, Destroyed }

    private Transform player;

    private MazeBehaviour.MazeGrid maze;

    private HealthBehaviour healthBehaviour;

    private State currentState = State.Chasing;

    void Start()
    {
        healthBehaviour = GetComponentInParent<HealthBehaviour>();
    }

    void Update()
    {
        if (healthBehaviour.GetCurrentHealth() <= 0) currentState = State.Destroyed;

        float distanceToPlayer = Vector3.Distance(transform.parent.position, player.position);

        switch (currentState) {
            case State.Chasing:
                Vector3 targetCellPosition = maze.GetNextCellInPath(transform.parent.position, player.position);
                transform.parent.position = Vector3.MoveTowards(transform.parent.position, targetCellPosition, 0.02f);
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
