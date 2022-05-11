using UnityEngine;

public class GameBehaviour : MonoBehaviour {

    public GameObject playerPrefab;
    public GameObject mazePrefab;
    public GameObject canvasPrefab;

    public bool showMinimap;

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

        if (showMinimap) minimapBehaviour = canvas.GetComponentInChildren<MinimapBehaviour>();
        else canvas.transform.Find("HUD").Find("Minimap").gameObject.SetActive(false);

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

    public void AddLowerLightSource(GameObject lightSource, LightingType lightingType) {
        mazeGenerator.AddLowerLightSource(lightSource, lightingType);
    }

    public int GetMazeSize() {
        return mazeGenerator.GetSize();
    }

    public MazeCell GetMazeCell(float x, float y) {
        return mazeGenerator.GetMazeCell(x, y);
    }

    public void CreateMinimapCell(int x, int y, string name, Color color, bool active) {
        if (showMinimap) minimapBehaviour.CreateMinimapCell(x, y, name, color, active);
    }

    public void ActivateMinimapCell(int x, int y) {
        if (showMinimap) minimapBehaviour.ActivateMinimapCell(x, y);
    }

    public void MovePlayerMinimapCell(int x, int y) {
        if (showMinimap) minimapBehaviour.MovePlayerMinimapCell(x, y);
    }
}
