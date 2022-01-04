using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{

    public GameObject playerPrefab;
    public GameObject mazePrefab;
    public GameObject minimapPrefab;

    private GameObject player;
    private GameObject maze;
    private GameObject minimap;

    private PlayerBehaviour playerBehaviour;
    private MazeBehaviour mazeBehaviour;
    private MinimapBehaviour minimapBehaviour;

    void Awake()
    {
        player = Instantiate(playerPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        playerBehaviour = player.GetComponent<PlayerBehaviour>();

        minimap = Instantiate(minimapPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        minimapBehaviour = minimap.GetComponent<MinimapBehaviour>();

        maze = Instantiate(mazePrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        mazeBehaviour = maze.GetComponent<MazeBehaviour>();
    }

    public void SetPlayerPosition(Vector3 position)
    {
        playerBehaviour.SetPosition(position);
    }

    public int GetMazeSize()
    {
        return mazeBehaviour.GetSize();
    }

    public void CreateMinimapCell(int x, int y, string name, Color color, bool active)
    {
        minimapBehaviour.CreateMinimapCell(x, y, name, color, active);
    }

    public void ActivateMinimapCell(int x, int y)
    {
        minimapBehaviour.ActivateMinimapCell(x, y);
    }

    public void MovePlayerMinimapCell(int x, int y)
    {
        minimapBehaviour.MovePlayerMinimapCell(x, y);
    }
}
