using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class MazeGenerator : MonoBehaviour
{

    [Range(3, 49)]
    public int Size = 21;
    [Range(0, 50)]
    public int RoomCount = 6;

    public GameObject PassagePrefab;
    public GameObject DeadEndPrefab;

    public bool GenerateRooms;

    public GameObject StartRoom;
    public GameObject ExitRoom;

    public GameObject[] RoomPrefabs3x3;
    public GameObject[] RoomPrefabs3x5;
    public GameObject[] RoomPrefabs5x3;
    public GameObject[] RoomPrefabs5x5;

    private GameObject _passageParent;
    private GameObject _deadEndParent;
    private GameObject _roomParent;

    private GameBehaviour _gameController;

    private MazeGrid _grid;
    private MazeCell _playerStart;

    private Dictionary<GameObject, Lighting.Type> _lowerLightSources;
    private Dictionary<GameObject, Lighting.Type> _upperLightSources;

    private Dictionary<GameObject, Lighting.Type> _temporaryLowerLightSources;

    private List<Vector2> _lowerLightSourcesToClear;

    private Color _colour;

    void OnValidate()
    {
        if (Size % 2 == 0) Size -= 1;
    }

    void Start()
    {
        _gameController = GameObject.Find("Controller").GetComponent<GameBehaviour>();

        _deadEndParent = new GameObject("Dead Ends");
        _deadEndParent.transform.parent = transform;
        _passageParent = new GameObject("Passages");
        _passageParent.transform.parent = transform;
        _roomParent = new GameObject("Rooms");
        _roomParent.transform.parent = transform;

        _lowerLightSources = new Dictionary<GameObject, Lighting.Type>();
        _upperLightSources = new Dictionary<GameObject, Lighting.Type>();

        _temporaryLowerLightSources = new Dictionary<GameObject, Lighting.Type>();

        _lowerLightSourcesToClear = new List<Vector2>();

        _colour = _gameController.MazeColour;

        GenerateMaze();
        SetInitialLighting();
    }

    void Update()
    {
        foreach (Vector2 cellPosition in _lowerLightSourcesToClear)
        {
            for (int y = Mathf.Max(0, Mathf.FloorToInt(cellPosition.y) - 4); y < Mathf.Min(Size, Mathf.FloorToInt(cellPosition.y) + 4); y++)
            {
                for (int x = Mathf.Max(0, Mathf.FloorToInt(cellPosition.x) - 4); x < Mathf.Min(Size, Mathf.FloorToInt(cellPosition.x) + 4); x++)
                {
                    _grid.GetCell((float)x, y).TemporaryLightingLower = Lighting.Type.DARKNESS;
                    _grid.GetCell((float)x, y).TemporaryLightingUpper = Lighting.Type.DARKNESS;
                }
            }
        }
        _lowerLightSourcesToClear.Clear();

        List<GameObject> lightSourcesToRemove = new();
        foreach (KeyValuePair<GameObject, Lighting.Type> entry in _temporaryLowerLightSources)
        {
            GameObject lightSource = entry.Key;
            if (lightSource != null)
            {
                _grid.SetTemporaryLightingLower(_grid.GetCell(lightSource.transform.position.x, lightSource.transform.position.z), entry.Value);
                _lowerLightSourcesToClear.Add(new Vector2(lightSource.transform.position.x, lightSource.transform.position.z));
            }
            else
            {
                lightSourcesToRemove.Add(lightSource);
            }
        }
        foreach (GameObject lightSource in lightSourcesToRemove) _lowerLightSources.Remove(lightSource);

        PlayerBehaviour player = _gameController.Player.GetComponent<PlayerBehaviour>();
        _grid.SetTemporaryLightingLower(_grid.GetCell(player.transform.position.x, player.transform.position.z), player.CurrentLighting);
        _lowerLightSourcesToClear.Add(new Vector2(player.transform.position.x, player.transform.position.z));
    }

    private void SetInitialLighting()
    {
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                _grid.GetCell(x, y).LightingLower = Lighting.Type.DARKNESS;
                _grid.GetCell(x, y).LightingUpper = Lighting.Type.DARKNESS;
            }
        }

        foreach (KeyValuePair<GameObject, Lighting.Type> entry in _lowerLightSources) AddLowerLightSource(entry.Key, entry.Value);
        foreach (KeyValuePair<GameObject, Lighting.Type> entry in _upperLightSources) AddUpperLightSource(entry.Key, entry.Value);
    }

    public void AddLowerLightSource(GameObject lightSource, Lighting.Type lightingType)
    {
        _grid.SetLightingLower(_grid.GetCell(lightSource.transform.position.x, lightSource.transform.position.z), lightingType);
    }

    public void AddToLowerLightSources(GameObject lightSource, Lighting.Type lightingType)
    {
        _lowerLightSources.Add(lightSource, lightingType);
    }

    public void AddUpperLightSource(GameObject lightSource, Lighting.Type lightingType)
    {
        _grid.SetLightingUpper(_grid.GetCell(lightSource.transform.position.x, lightSource.transform.position.z), lightingType);
    }

    public void AddToUpperLightSources(GameObject lightSource, Lighting.Type lightingType)
    {
        _upperLightSources.Add(lightSource, lightingType);
    }

    public void AddTemporaryLowerLightSource(GameObject lightSource, Lighting.Type lightingType)
    {
        _temporaryLowerLightSources.Add(lightSource, lightingType);
    }

    private void GenerateMaze()
    {
        _grid = new MazeGrid(Size, _colour);

        int[] roomDimensions = new int[] { 3, 5 };

        List<MazeRoom> existingRooms = new List<MazeRoom>();

        int startRoomX = Random.Range(3, (Size - 3) - 1);
        int startRoomY = Random.Range(3, (Size - 3) - 1);
        if (startRoomX % 2 == 0) startRoomX -= 1;
        if (startRoomY % 2 == 0) startRoomY -= 1;
        GameObject start = Instantiate(StartRoom, new Vector3(startRoomX, 0, startRoomY), StartRoom.transform.rotation);
        _playerStart = _grid.GetCell(startRoomX + 1, startRoomY + 1);
        start.transform.parent = _roomParent.transform;
        for (int x = startRoomX; x < startRoomX + 3; x++)
        {
            for (int y = startRoomY; y < startRoomY + 3; y++)
            {
                _grid.SetCellType(x, y, MazeCell.Type.ROOM);
                _gameController.CreateMinimapCell(x, y, x + "," + y, Color.white, false);
            }
        }
        foreach (Transform roomPiece in start.transform)
        {
            if (roomPiece.name.Contains("Torch") || roomPiece.name.Contains("Chandelier"))
            {
                if (roomPiece.position.y == 0) _lowerLightSources.Add(roomPiece.gameObject, Lighting.Type.TORCH_0);
                else if (roomPiece.position.y == 1) _upperLightSources.Add(roomPiece.gameObject, Lighting.Type.TORCH_0);
            }
            else if (roomPiece.name.Contains("Door"))
            {
                string[] details = roomPiece.name.Split('-');

                int doorX = startRoomX + int.Parse(details[1]);
                int doorY = startRoomY + int.Parse(details[2]);

                switch (details[3])
                {
                    case "N":
                        doorX -= 1;
                        break;
                    case "E":
                        doorY += 1;
                        break;
                    case "S":
                        doorX += 1;
                        break;
                    case "W":
                        doorY -= 1;
                        break;
                    default:
                        break;
                }

                _grid.GetCell(doorX, doorY).AddToMazePieces(roomPiece.gameObject);

                _grid.SetCellType(doorX, doorY, MazeCell.Type.DISCONNECTED_DOOR);
                _gameController.CreateMinimapCell(doorX, doorY, doorX + "," + doorY, Color.white, false);
            }
            else
            {
                _grid.GetCell(roomPiece.position.x, roomPiece.position.z).AddToMazePieces(roomPiece.gameObject);
            }
        }
        existingRooms.Add(new MazeRoom(startRoomX, startRoomY, 3, 3));

        int exitRoomX = Random.Range(3, (Size - 3) - 1);
        int exitRoomY = Random.Range(3, (Size - 3) - 1);
        if (exitRoomX % 2 == 0) exitRoomX -= 1;
        if (exitRoomY % 2 == 0) exitRoomY -= 1;
        GameObject exit = Instantiate(ExitRoom, new Vector3(exitRoomX, 0, exitRoomY), ExitRoom.transform.rotation);
        exit.transform.parent = _roomParent.transform;
        for (int x = exitRoomX; x < exitRoomX + 3; x++)
        {
            for (int y = exitRoomY; y < exitRoomY + 3; y++)
            {
                _grid.SetCellType(x, y, MazeCell.Type.ROOM);
                _gameController.CreateMinimapCell(x, y, x + "," + y, Color.white, false);
            }
        }
        foreach (Transform roomPiece in exit.transform)
        {
            if (roomPiece.name.Contains("Torch") || roomPiece.name.Contains("Chandelier"))
            {
                if (roomPiece.position.y == 0) _lowerLightSources.Add(roomPiece.gameObject, Lighting.Type.TORCH_0);
                else if (roomPiece.position.y == 1) _upperLightSources.Add(roomPiece.gameObject, Lighting.Type.TORCH_0);
            }
            else if (roomPiece.name.Contains("Door"))
            {
                string[] details = roomPiece.name.Split('-');

                int doorX = exitRoomX + int.Parse(details[1]);
                int doorY = exitRoomY + int.Parse(details[2]);

                switch (details[3])
                {
                    case "N":
                        doorX -= 1;
                        break;
                    case "E":
                        doorY += 1;
                        break;
                    case "S":
                        doorX += 1;
                        break;
                    case "W":
                        doorY -= 1;
                        break;
                    default:
                        break;
                }

                _grid.GetCell(doorX, doorY).AddToMazePieces(roomPiece.gameObject);

                _grid.SetCellType(doorX, doorY, MazeCell.Type.DISCONNECTED_DOOR);
                _gameController.CreateMinimapCell(doorX, doorY, doorX + "," + doorY, Color.white, false);
            }
            else
            {
                _grid.GetCell(roomPiece.position.x, roomPiece.position.z).AddToMazePieces(roomPiece.gameObject);
            }
        }
        existingRooms.Add(new MazeRoom(exitRoomX, exitRoomY, 3, 3));

        if (GenerateRooms)
        {
            for (int i = 0; i < RoomCount; i++)
            {

                bool canBeCreated = false;
                int attemptCount = 0;

                while (!canBeCreated && attemptCount < 10)
                {
                    canBeCreated = true;
                    attemptCount++;

                    int roomWidth = roomDimensions[Random.Range(0, roomDimensions.Length)];
                    int roomHeight = roomDimensions[Random.Range(0, roomDimensions.Length)];

                    int roomX = Random.Range(3, (Size - roomWidth) - 1);
                    int roomY = Random.Range(3, (Size - roomHeight) - 1);
                    if (roomX % 2 == 0) roomX -= 1;
                    if (roomY % 2 == 0) roomY -= 1;

                    MazeRoom room = new MazeRoom(roomX, roomY, roomWidth, roomHeight);

                    foreach (MazeRoom existingRoom in existingRooms)
                    {
                        if (MazeRoom.Overlap(room, existingRoom)) canBeCreated = false;
                    }

                    if (canBeCreated)
                    {
                        GameObject chosenRoom = RoomPrefabs3x3[Random.Range(0, RoomPrefabs3x3.Length)];

                        switch (roomWidth)
                        {
                            case 3:
                                switch (roomHeight)
                                {
                                    case 3:
                                        chosenRoom = RoomPrefabs3x3[Random.Range(0, RoomPrefabs3x3.Length)];
                                        break;
                                    case 5:
                                        chosenRoom = RoomPrefabs3x5[Random.Range(0, RoomPrefabs3x5.Length)];
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case 5:
                                switch (roomHeight)
                                {
                                    case 3:
                                        chosenRoom = RoomPrefabs5x3[Random.Range(0, RoomPrefabs5x3.Length)];
                                        break;
                                    case 5:
                                        chosenRoom = RoomPrefabs5x5[Random.Range(0, RoomPrefabs5x5.Length)];
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            default:
                                break;
                        }

                        GameObject roomObject = Instantiate(chosenRoom, new Vector3(roomX, 0, roomY), chosenRoom.transform.rotation);
                        roomObject.transform.parent = _roomParent.transform;

                        for (int x = roomX; x < roomX + roomWidth; x++)
                        {
                            for (int y = roomY; y < roomY + roomHeight; y++)
                            {
                                _grid.SetCellType(x, y, MazeCell.Type.ROOM);
                                _gameController.CreateMinimapCell(x, y, x + "," + y, Color.white, false);
                            }
                        }

                        foreach (Transform roomPiece in roomObject.transform)
                        {
                            if (roomPiece.name.Contains("Torch") || roomPiece.name.Contains("Chandelier"))
                            {
                                if (roomPiece.position.y == 0) _lowerLightSources.Add(roomPiece.gameObject, Lighting.Type.TORCH_0);
                                else if (roomPiece.position.y == 1) _upperLightSources.Add(roomPiece.gameObject, Lighting.Type.TORCH_0);
                            }
                            else if (roomPiece.name.Contains("Door"))
                            {
                                string[] details = roomPiece.name.Split('-');

                                int doorX = roomX + int.Parse(details[1]);
                                int doorY = roomY + int.Parse(details[2]);

                                switch (details[3])
                                {
                                    case "N":
                                        doorX -= 1;
                                        break;
                                    case "E":
                                        doorY += 1;
                                        break;
                                    case "S":
                                        doorX += 1;
                                        break;
                                    case "W":
                                        doorY -= 1;
                                        break;
                                    default:
                                        break;
                                }

                                _grid.GetCell(doorX, doorY).AddToMazePieces(roomPiece.gameObject);

                                _grid.SetCellType(doorX, doorY, MazeCell.Type.DISCONNECTED_DOOR);
                                _gameController.CreateMinimapCell(doorX, doorY, doorX + "," + doorY, Color.white, false);
                            }
                            else
                            {
                                _grid.GetCell(roomPiece.position.x, roomPiece.position.z).AddToMazePieces(roomPiece.gameObject);
                            }
                        }

                        existingRooms.Add(room);
                    }
                }
            }
        }

        MazeDepthFirstSearch(1, 1);

        _grid.CleanUp();

        string path;
        StreamWriter writer = null;
        if (Application.isEditor)
        {
            path = "Assets/maze.txt";
            writer = new StreamWriter(path, false);
        }

        List<MazeCell> deadEnds = new List<MazeCell>();

        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                MazeCell currentCell = _grid.GetCell(x, y);
                switch (currentCell.CurrentType)
                {
                    case MazeCell.Type.PASSAGE:
                        GameObject passage = Instantiate(PassagePrefab, new Vector3(x, 0, y), PassagePrefab.transform.rotation);

                        passage.transform.parent = _passageParent.transform;

                        foreach (Transform mazePiece in passage.transform.Find("Maze Pieces")) _grid.GetCell(x, y).AddToMazePieces(mazePiece.gameObject);

                        Passage.WallState northState = Passage.WallState.CLOSED;
                        Passage.WallState eastState = Passage.WallState.CLOSED;
                        Passage.WallState southState = Passage.WallState.CLOSED;
                        Passage.WallState westState = Passage.WallState.CLOSED;

                        MazeCell.Type neighbourType = _grid.GetNorthNeighbour(x, y).CurrentType;
                        if (neighbourType == MazeCell.Type.DOOR || neighbourType == MazeCell.Type.HIDDEN_DOOR)
                        {
                            northState = Passage.WallState.LOWER_OPEN;
                        }
                        else if (neighbourType == MazeCell.Type.PASSAGE || neighbourType == MazeCell.Type.DEAD_END)
                        {
                            northState = Passage.WallState.OPEN;
                        }

                        neighbourType = _grid.GetEastNeighbour(x, y).CurrentType;
                        if (neighbourType == MazeCell.Type.DOOR || neighbourType == MazeCell.Type.HIDDEN_DOOR)
                        {
                            eastState = Passage.WallState.LOWER_OPEN;
                        }
                        else if (neighbourType == MazeCell.Type.PASSAGE || neighbourType == MazeCell.Type.DEAD_END)
                        {
                            eastState = Passage.WallState.OPEN;
                        }

                        neighbourType = _grid.GetSouthNeighbour(x, y).CurrentType;
                        if (neighbourType == MazeCell.Type.DOOR || neighbourType == MazeCell.Type.HIDDEN_DOOR)
                        {
                            southState = Passage.WallState.LOWER_OPEN;
                        }
                        else if (neighbourType == MazeCell.Type.PASSAGE || neighbourType == MazeCell.Type.DEAD_END)
                        {
                            southState = Passage.WallState.OPEN;
                        }

                        neighbourType = _grid.GetWestNeighbour(x, y).CurrentType;
                        if (neighbourType == MazeCell.Type.DOOR || neighbourType == MazeCell.Type.HIDDEN_DOOR)
                        {
                            westState = Passage.WallState.LOWER_OPEN;
                        }
                        else if (neighbourType == MazeCell.Type.PASSAGE || neighbourType == MazeCell.Type.DEAD_END)
                        {
                            westState = Passage.WallState.OPEN;
                        }

                        if (x % 2 == 0)
                        {
                            if (y == Size - 2) { northState = Passage.WallState.WINDOW; }
                            else if (y == 1) { southState = Passage.WallState.WINDOW; }
                        }
                        else if (y % 2 == 0)
                        {
                            if (x == 1) { westState = Passage.WallState.WINDOW; }
                            else if (x == Size - 2) { eastState = Passage.WallState.WINDOW; }
                        }

                        passage.transform.GetComponent<Passage>().Setup(transform, northState, eastState, southState, westState, _colour);

                        _gameController.CreateMinimapCell(x, y, x + "," + y, Color.white, false);

                        break;
                    case MazeCell.Type.DEAD_END:
                        GameObject deadEnd = Instantiate(DeadEndPrefab, new Vector3(x, 0, y), Quaternion.identity);
                        deadEnd.transform.parent = _deadEndParent.transform;

                        if (_grid.GetNorthNeighbour(x, y).CurrentType != MazeCell.Type.WALL) deadEnd.GetComponent<DeadEnd>().SetDirectionNorth();
                        else if (_grid.GetEastNeighbour(x, y).CurrentType != MazeCell.Type.WALL) deadEnd.GetComponent<DeadEnd>().SetDirectionEast();
                        else if (_grid.GetSouthNeighbour(x, y).CurrentType != MazeCell.Type.WALL) deadEnd.GetComponent<DeadEnd>().SetDirectionSouth();
                        else if (_grid.GetWestNeighbour(x, y).CurrentType != MazeCell.Type.WALL) deadEnd.GetComponent<DeadEnd>().SetDirectionWest();

                        foreach (Transform mazePiece in deadEnd.transform.Find("Maze Pieces")) _grid.GetCell(x, y).AddToMazePieces(mazePiece.gameObject);

                        _lowerLightSources.Add(deadEnd.transform.Find("Maze Pieces").Find("Torch").gameObject, Lighting.Type.TORCH_0);

                        _gameController.CreateMinimapCell(x, y, x + "," + y, Color.white, false);

                        deadEnds.Add(_grid.GetCell(x, y));

                        break;
                    default:
                        break;
                }
                if (Application.isEditor) writer.Write((int)_grid.GetCell(y, x).CurrentType + " ");
            }
            if (Application.isEditor) writer.Write("\n");
        }
        if (Application.isEditor) writer.Close();

        _gameController.SetPlayerPosition(new Vector3(_playerStart.X, 1, _playerStart.Y));

        _lowerLightSources.Add(_gameController.Player, _gameController.Player.GetComponent<PlayerBehaviour>().CurrentLighting);
    }

    private void MazeDepthFirstSearch(int x, int y)
    {
        _grid.SetCellType(x, y, MazeCell.Type.PASSAGE);

        List<MazeCell> availableNeighbours = _grid.GetSecondNeighboursOfType(x, y, MazeCell.Type.WALL, MazeCell.Type.DISCONNECTED_DOOR, MazeCell.Type.DISCONNECTED_HIDDEN_DOOR);

        while (availableNeighbours.Count != 0)
        {
            MazeCell chosenNeighbour = availableNeighbours[Random.Range(0, availableNeighbours.Count)];
            if (chosenNeighbour.CurrentType == MazeCell.Type.WALL)
            {
                if (chosenNeighbour == _grid.GetSecondNorthNeighbour(x, y)) _grid.SetCellType(x, y + 1, MazeCell.Type.PASSAGE);
                else if (chosenNeighbour == _grid.GetSecondEastNeighbour(x, y)) _grid.SetCellType(x + 1, y, MazeCell.Type.PASSAGE);
                else if (chosenNeighbour == _grid.GetSecondSouthNeighbour(x, y)) _grid.SetCellType(x, y - 1, MazeCell.Type.PASSAGE);
                else if (chosenNeighbour == _grid.GetSecondWestNeighbour(x, y)) _grid.SetCellType(x - 1, y, MazeCell.Type.PASSAGE);
                _grid.SetCellType(chosenNeighbour.X, chosenNeighbour.Y, MazeCell.Type.PASSAGE);
                MazeDepthFirstSearch(chosenNeighbour.X, chosenNeighbour.Y);
            }
            else if (chosenNeighbour.CurrentType == MazeCell.Type.DISCONNECTED_DOOR)
            {
                if (chosenNeighbour == _grid.GetSecondNorthNeighbour(x, y)) _grid.SetCellType(x, y + 1, MazeCell.Type.DOOR);
                else if (chosenNeighbour == _grid.GetSecondEastNeighbour(x, y)) _grid.SetCellType(x + 1, y, MazeCell.Type.DOOR);
                else if (chosenNeighbour == _grid.GetSecondSouthNeighbour(x, y)) _grid.SetCellType(x, y - 1, MazeCell.Type.DOOR);
                else if (chosenNeighbour == _grid.GetSecondWestNeighbour(x, y)) _grid.SetCellType(x - 1, y, MazeCell.Type.DOOR);
                _grid.SetCellType(chosenNeighbour.X, chosenNeighbour.Y, MazeCell.Type.ROOM);
            }
            else if (chosenNeighbour.CurrentType == MazeCell.Type.DISCONNECTED_HIDDEN_DOOR)
            {
                if (chosenNeighbour == _grid.GetSecondNorthNeighbour(x, y)) _grid.SetCellType(x, y + 1, MazeCell.Type.HIDDEN_DOOR);
                else if (chosenNeighbour == _grid.GetSecondEastNeighbour(x, y)) _grid.SetCellType(x + 1, y, MazeCell.Type.HIDDEN_DOOR);
                else if (chosenNeighbour == _grid.GetSecondSouthNeighbour(x, y)) _grid.SetCellType(x, y - 1, MazeCell.Type.HIDDEN_DOOR);
                else if (chosenNeighbour == _grid.GetSecondWestNeighbour(x, y)) _grid.SetCellType(x - 1, y, MazeCell.Type.HIDDEN_DOOR);
                _grid.SetCellType(chosenNeighbour.X, chosenNeighbour.Y, MazeCell.Type.HIDDEN_ROOM);
            }
            availableNeighbours.Remove(chosenNeighbour);
        }
    }

    public int GetSize()
    {
        return Size;
    }

    public MazeCell GetMazeCell(float x, float y)
    {
        return _grid?.GetCell(x, y);
    }

}
