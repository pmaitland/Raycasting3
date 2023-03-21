using UnityEngine;

public class GameBehaviour : MonoBehaviour {

    public GameObject playerPrefab;
    public GameObject mazePrefab;
    public GameObject canvasPrefab;

    public bool showMinimap;

    private bool paused;
    private string pauseKey = "escape";

    private GameObject player;
    private GameObject maze;
    private GameObject canvas;

    private PlayerBehaviour playerBehaviour;
    private MazeGenerator mazeGenerator;
    private MinimapBehaviour minimapBehaviour;
    private PauseScreenBehaviour pauseScreenBehaviour;

    private Color mazeColour;

    void Awake() {
        player = Instantiate(playerPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        player.name = "Player";
        playerBehaviour = player.GetComponent<PlayerBehaviour>();

        canvas = Instantiate(canvasPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        canvas.name = "Canvas";

        if (showMinimap) minimapBehaviour = canvas.GetComponentInChildren<MinimapBehaviour>();
        else canvas.transform.Find("HUD").Find("Minimap").gameObject.SetActive(false);

        PickColour();
        GenerateMaze();

        pauseScreenBehaviour = canvas.transform.Find("Pause Screen").GetComponent<PauseScreenBehaviour>();
        paused = false;
    }

    void Update () {
        if (Input.GetKeyDown("space")) {
            if (paused) Application.Quit();
        }

        if (Input.GetKeyDown(pauseKey)) {
            if (paused) Unpause();
            else Pause();
        }
    }

    public void GenerateMaze() {
        if (maze != null) {
            Destroy(maze);
        }
        maze = Instantiate(mazePrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        maze.name = "Maze";
        mazeGenerator = maze.GetComponent<MazeGenerator>();
    }

    private void Pause() {
        paused = true;
        pauseScreenBehaviour.Pause();
        Time.timeScale = 0;
    }

    private void Unpause() {
        paused = false;
        pauseScreenBehaviour.Unpause();
        Time.timeScale = 1;
    }

    public bool IsPaused() {
        return paused;
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

    public void AddUpperLightSource(GameObject lightSource, LightingType lightingType) {
        mazeGenerator.AddUpperLightSource(lightSource, lightingType);
    }

    public void AddTemporaryLowerLightSource(GameObject lightSource, LightingType lightingType) {
        mazeGenerator.AddTemporaryLowerLightSource(lightSource, lightingType);
    }

    public int GetMazeSize() {
        return mazeGenerator.GetSize();
    }

    public Color GetMazeColour() {
        return mazeColour;
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

    private void PickColour() {
        Color[] colours = {
            Color.red,
            Color.magenta,
            Color.blue
        };
        mazeColour = colours[Random.Range(0, colours.Length)];
    }
}
