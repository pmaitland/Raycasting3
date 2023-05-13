using UnityEngine;

public class GameBehaviour : MonoBehaviour
{

    public GameObject PlayerPrefab;
    public GameObject DungeonMazePrefab;
    public GameObject CastleMazePrefab;
    public GameObject CanvasPrefab;

    public bool ShowMinimap;

    public bool Paused { get; private set; }
    private readonly string _pauseKey = "escape";

    public GameObject Player { get; private set; }
    private GameObject _maze;
    private GameObject _canvas;

    private PlayerBehaviour _playerBehaviour;
    private MazeGenerator _mazeGenerator;
    private MinimapBehaviour _minimapBehaviour;
    private PauseScreenBehaviour _pauseScreenBehaviour;

    public Color MazeColour { get; private set; }

    private int _currentLevel = 0;

    public bool GeneratingMaze { get; private set; } = false;

    void Awake()
    {
        Player = Instantiate(PlayerPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        Player.name = "Player";
        _playerBehaviour = Player.GetComponent<PlayerBehaviour>();

        _canvas = Instantiate(CanvasPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        _canvas.name = "Canvas";

        if (ShowMinimap) _minimapBehaviour = _canvas.GetComponentInChildren<MinimapBehaviour>();
        else _canvas.transform.Find("HUD").Find("Minimap").gameObject.SetActive(false);

        PickColour();
        GenerateMaze();

        _pauseScreenBehaviour = _canvas.transform.Find("Pause Screen").GetComponent<PauseScreenBehaviour>();
        Paused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (Paused) Application.Quit();
        }

        if (Input.GetKeyDown(_pauseKey))
        {
            if (Paused) Unpause();
            else Pause();
        }
    }

    public void GenerateMaze()
    {
        GeneratingMaze = true;

        _currentLevel++;

        if (_maze != null)
        {
            Destroy(_maze);
        }

        // if (_currentLevel <= 5)
        // {
        //     _maze = Instantiate(DungeonMazePrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        // }
        // else
        // {
        _maze = Instantiate(CastleMazePrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        // }

        _maze.name = "Maze";
        _mazeGenerator = _maze.GetComponent<MazeGenerator>();

        GeneratingMaze = false;
    }

    private void Pause()
    {
        Paused = true;
        _pauseScreenBehaviour.Pause();
        Time.timeScale = 0;
    }

    private void Unpause()
    {
        Paused = false;
        _pauseScreenBehaviour.Unpause();
        Time.timeScale = 1;
    }

    public void SetPlayerPosition(Vector3 position)
    {
        _playerBehaviour.SetPosition(position);
    }

    public void AddLowerLightSource(GameObject lightSource, Lighting.Type lightingType)
    {
        _mazeGenerator.AddLowerLightSource(lightSource, lightingType);
    }

    public void AddUpperLightSource(GameObject lightSource, Lighting.Type lightingType)
    {
        _mazeGenerator.AddUpperLightSource(lightSource, lightingType);
    }

    public void AddTemporaryLowerLightSource(GameObject lightSource, Lighting.Type lightingType)
    {
        _mazeGenerator.AddTemporaryLowerLightSource(lightSource, lightingType);
    }

    public int GetMazeSize()
    {
        return _mazeGenerator.GetSize();
    }

    public MazeCell GetMazeCell(float x, float y)
    {
        return _mazeGenerator.GetMazeCell(x, y);
    }

    public void CreateMinimapCell(int x, int y, string name, Color color, bool active)
    {
        if (ShowMinimap) _minimapBehaviour.CreateMinimapCell(x, y, name, color, active);
    }

    public void ActivateMinimapCell(int x, int y)
    {
        if (ShowMinimap) _minimapBehaviour.ActivateMinimapCell(x, y);
    }

    public void MovePlayerMinimapCell(int x, int y)
    {
        if (ShowMinimap) _minimapBehaviour.MovePlayerMinimapCell(x, y);
    }

    private void PickColour()
    {
        Color[] colours = {
            Color.red,
            Color.magenta,
            Color.blue
        };
        MazeColour = colours[Random.Range(0, colours.Length)];
    }

}
