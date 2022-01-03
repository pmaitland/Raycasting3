using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{

    public GameObject playerPrefab;
    public GameObject mazePrefab;

    private GameObject player;
    private GameObject maze;
    
    private MazeBehaviour mazeBehaviour;
    
    void Start()
    {
        maze = Instantiate(mazePrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        player = Instantiate(playerPrefab, maze.GetComponent<MazeBehaviour>().GetPlayerStart(), new Quaternion(0, 0, 0, 0));

        mazeBehaviour = maze.GetComponent<MazeBehaviour>();
    }

    public void ActivateMinimapCell(int x, int y)
    {
        mazeBehaviour.ActivateMinimapCell(x, y);
    }

    public void MovePlayerMinimapCell(int x, int y)
    {
        mazeBehaviour.MovePlayerMinimapCell(x, y);
    }
}
