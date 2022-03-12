using UnityEngine;

public class GameBehaviour : MonoBehaviour {

    public GameObject playerPrefab;
    public GameObject mazePrefab;
    public GameObject canvasPrefab;

    private GameObject player;
    private GameObject maze;
    private GameObject canvas;

    private PlayerBehaviour playerBehaviour;
    private MazeGenerator mazeGenerator;
    private MinimapBehaviour minimapBehaviour;

    void Awake() {
        player = Instantiate(playerPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        player.name = "Player";
        playerBehaviour = player.GetComponent<PlayerBehaviour>();

        canvas = Instantiate(canvasPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        canvas.name = "Canvas";
        minimapBehaviour = canvas.GetComponentInChildren<MinimapBehaviour>();

        maze = Instantiate(mazePrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        maze.name = "Maze";
        mazeGenerator = maze.GetComponent<MazeGenerator>();
    }

    public GameObject GetPlayer() {
        return player;
    }

    public void SetPlayerPosition(Vector3 position) {
        playerBehaviour.SetPosition(position);
    }

    public int GetMazeSize() {
        return mazeGenerator.GetSize();
    }

    public MazeCell GetMazeCell(float x, float y) {
        return mazeGenerator.GetMazeCell(x, y);
    }

    public void CreateMinimapCell(int x, int y, string name, Color color, bool active) {
        minimapBehaviour.CreateMinimapCell(x, y, name, color, active);
    }

    public void ActivateMinimapCell(int x, int y) {
        minimapBehaviour.ActivateMinimapCell(x, y);
    }

    public void MovePlayerMinimapCell(int x, int y) {
        minimapBehaviour.MovePlayerMinimapCell(x, y);
    }
}
