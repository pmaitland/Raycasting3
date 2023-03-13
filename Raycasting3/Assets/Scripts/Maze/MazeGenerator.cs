using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MazeGenerator : MonoBehaviour {
    
    [Range(3, 49)]
    public int size = 21;
    [Range(0, 50)]
    public int roomCount = 6;

    public GameObject passagePrefab;
    public GameObject deadEndPrefab;

    public GameObject[] roomPrefabs3x3;
    public GameObject[] roomPrefabs3x5;
    public GameObject[] roomPrefabs5x3;
    public GameObject[] roomPrefabs5x5;

    public GameObject knightPrefab;

    private GameObject knightParent;

    private GameObject passageParent;
    private GameObject deadEndParent;
    private GameObject roomParent;

    private GameBehaviour gameController;

    private MazeGrid grid;
    private MazeCell playerStart;

    private Dictionary<GameObject, LightingType> lowerLightSources;
    private Dictionary<GameObject, LightingType> upperLightSources;

    private Dictionary<GameObject, LightingType> temporaryLowerLightSources;

    private List<Vector2> lowerLightSourcesToClear;

    void OnValidate() {
        if (size % 2 == 0) size -= 1;
    }

    void Start() {
        gameController = GameObject.Find("Controller").GetComponent<GameBehaviour>();

        knightParent = new GameObject("Knights");
        knightParent.transform.parent = transform;

        deadEndParent = new GameObject("Dead Ends");
        deadEndParent.transform.parent = transform;
        passageParent = new GameObject("Passages");
        passageParent.transform.parent = transform;
        roomParent = new GameObject("Rooms");
        roomParent.transform.parent = transform;

        lowerLightSources = new Dictionary<GameObject, LightingType>();
        upperLightSources = new Dictionary<GameObject, LightingType>();

        temporaryLowerLightSources = new Dictionary<GameObject, LightingType>();

        lowerLightSourcesToClear = new List<Vector2>();

        GenerateMaze();
        SetInitialLighting();
    }

    void Update() {
        foreach (Vector2 cellPosition in lowerLightSourcesToClear) {
            for (int y = Mathf.Max(0, Mathf.FloorToInt(cellPosition.y) - 4); y < Mathf.Min(size, Mathf.FloorToInt(cellPosition.y) + 4); y++) {
                for (int x = Mathf.Max(0, Mathf.FloorToInt(cellPosition.x) - 4); x < Mathf.Min(size, Mathf.FloorToInt(cellPosition.x) + 4); x++) {
                    grid.GetCell(x, y).SetTemporaryLightingLower(LightingType.DARKNESS);
                    grid.GetCell(x, y).SetTemporaryLightingUpper(LightingType.DARKNESS);
                }
            }
        }
        lowerLightSourcesToClear.Clear();

        List<GameObject> lightSourcesToRemove = new List<GameObject>();
        foreach (KeyValuePair<GameObject, LightingType> entry in temporaryLowerLightSources) {
            GameObject lightSource = entry.Key;
            if (lightSource != null) {
                grid.SetTemporaryLightingLower(grid.GetCell(lightSource.transform.position.x, lightSource.transform.position.z), entry.Value);
                lowerLightSourcesToClear.Add(new Vector2(lightSource.transform.position.x, lightSource.transform.position.z));
            } else {
                lightSourcesToRemove.Add(lightSource);
            }
        }
        foreach (GameObject lightSource in lightSourcesToRemove) lowerLightSources.Remove(lightSource);

        PlayerBehaviour player = gameController.GetPlayer().GetComponent<PlayerBehaviour>();
        grid.SetTemporaryLightingLower(grid.GetCell(player.transform.position.x, player.transform.position.z), player.GetLighting());
        lowerLightSourcesToClear.Add(new Vector2(player.transform.position.x, player.transform.position.z));
    }

    private void SetInitialLighting() {
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                grid.GetCell(x, y).SetLightingLower(LightingType.DARKNESS);
                grid.GetCell(x, y).SetLightingUpper(LightingType.DARKNESS);
            }
        }

        foreach (KeyValuePair<GameObject, LightingType> entry in lowerLightSources) AddLowerLightSource(entry.Key, entry.Value);
        foreach (KeyValuePair<GameObject, LightingType> entry in upperLightSources) AddUpperLightSource(entry.Key, entry.Value);
    }

    public void AddLowerLightSource(GameObject lightSource, LightingType lightingType) {
        grid.SetLightingLower(grid.GetCell(lightSource.transform.position.x, lightSource.transform.position.z), lightingType);
    }

    public void AddToLowerLightSources(GameObject lightSource, LightingType lightingType) {
        lowerLightSources.Add(lightSource, lightingType);
    }

    public void AddUpperLightSource(GameObject lightSource, LightingType lightingType) {
        grid.SetLightingUpper(grid.GetCell(lightSource.transform.position.x, lightSource.transform.position.z), lightingType);
    }

    public void AddToUpperLightSources(GameObject lightSource, LightingType lightingType) {
        upperLightSources.Add(lightSource, lightingType);
    }

    public void AddTemporaryLowerLightSource(GameObject lightSource, LightingType lightingType) {
        temporaryLowerLightSources.Add(lightSource, lightingType);
    }

    private void GenerateMaze() {
        grid = new MazeGrid(size);

        int[] roomDimensions = new int[] { 3, 5 };

        List<MazeRoom> existingRooms = new List<MazeRoom>();
        for (int i = 0; i < roomCount; i++) {

            bool canBeCreated = false;
            int attemptCount = 0;

            while (!canBeCreated && attemptCount < 10) {
                canBeCreated = true;
                attemptCount++;

                int roomWidth = roomDimensions[Random.Range(0, roomDimensions.Length)];
                int roomHeight = roomDimensions[Random.Range(0, roomDimensions.Length)];

                int roomX = Random.Range(3, (size - roomWidth) - 1);
                int roomY = Random.Range(3, (size - roomHeight) - 1);
                if (roomX % 2 == 0) roomX -= 1;
                if (roomY % 2 == 0) roomY -= 1;

                MazeRoom room = new MazeRoom(roomX, roomY, roomWidth, roomHeight);

                foreach (MazeRoom existingRoom in existingRooms) {
                    if (MazeRoom.Overlap(room, existingRoom)) canBeCreated = false;
                }

                if (canBeCreated) {
                    GameObject chosenRoom = roomPrefabs3x3[Random.Range(0, roomPrefabs3x3.Length)];
                    
                    switch (roomWidth) {
                        case 3:
                            switch (roomHeight) {
                                case 3:
                                    chosenRoom = roomPrefabs3x3[Random.Range(0, roomPrefabs3x3.Length)];
                                    break;
                                case 5:
                                    chosenRoom = roomPrefabs3x5[Random.Range(0, roomPrefabs3x5.Length)];
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 5:
                            switch (roomHeight) {
                                case 3:
                                    chosenRoom = roomPrefabs5x3[Random.Range(0, roomPrefabs5x3.Length)];
                                    break;
                                case 5:
                                    chosenRoom = roomPrefabs5x5[Random.Range(0, roomPrefabs5x5.Length)];
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    
                    GameObject roomObject = Instantiate(chosenRoom, new Vector3(roomX, 0, roomY), chosenRoom.transform.rotation);
                    roomObject.transform.parent = roomParent.transform;

                    for (int x = roomX; x < roomX + roomWidth; x++) {
                        for (int y = roomY; y < roomY + roomHeight; y++) {
                            grid.SetCellType(x, y, MazeCellType.ROOM);
                            gameController.CreateMinimapCell(x, y, x + "," + y, Color.white, false);
                        }
                    }

                    foreach (Transform roomPiece in roomObject.transform) {
                        if (roomPiece.name.Contains("Torch") || roomPiece.name.Contains("Chandelier")) {
                            if (roomPiece.position.y == 0) lowerLightSources.Add(roomPiece.gameObject, LightingType.TORCH_0);
                            else if (roomPiece.position.y == 1) upperLightSources.Add(roomPiece.gameObject, LightingType.TORCH_0);
                        } else if (roomPiece.name.Contains("Door")) {
                            string[] details = roomPiece.name.Split('-');

                            int doorX = roomX + int.Parse(details[1]);
                            int doorY = roomY + int.Parse(details[2]);

                            switch (details[3]) {
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

                            grid.GetCell(doorX, doorY).AddToMazePieces(roomPiece.gameObject);

                            grid.SetCellType(doorX, doorY, MazeCellType.DISCONNECTED_DOOR);
                            gameController.CreateMinimapCell(doorX, doorY, doorX + "," + doorY, Color.white, false);
                        } else {
                            grid.GetCell(roomPiece.position.x, roomPiece.position.z).AddToMazePieces(roomPiece.gameObject);
                        }
                    }

                    existingRooms.Add(room);
                }
            }
        }

        MazeDepthFirstSearch(1, 1);

        grid.CleanUp();

        string path;
        StreamWriter writer = null;
        if (Application.isEditor) {
            path = "Assets/maze.txt";
            writer = new StreamWriter(path, false);
        }

        List<MazeCell> deadEnds = new List<MazeCell>();

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                MazeCell currentCell = grid.GetCell(x, y);
                switch (currentCell.GetCellType()) {
                    case MazeCellType.PASSAGE:
                        GameObject passage = Instantiate(passagePrefab, new Vector3(x, 0, y), passagePrefab.transform.rotation);

                        passage.transform.parent = passageParent.transform;

                        foreach (Transform mazePiece in passage.transform.Find("Maze Pieces")) grid.GetCell(x, y).AddToMazePieces(mazePiece.gameObject);

                        Passage.WallState northState = Passage.WallState.CLOSED;
                        Passage.WallState eastState = Passage.WallState.CLOSED;
                        Passage.WallState southState = Passage.WallState.CLOSED;
                        Passage.WallState westState = Passage.WallState.CLOSED;

                        MazeCellType neighbourType = grid.GetNorthNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.DOOR || neighbourType == MazeCellType.HIDDEN_DOOR) {
                            northState = Passage.WallState.LOWER_OPEN;
                        } else if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END) {
                            northState = Passage.WallState.OPEN;
                        }

                        neighbourType = grid.GetEastNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.DOOR || neighbourType == MazeCellType.HIDDEN_DOOR) {
                            eastState = Passage.WallState.LOWER_OPEN;
                        } else if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END) {
                            eastState = Passage.WallState.OPEN;
                        }

                        neighbourType = grid.GetSouthNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.DOOR || neighbourType == MazeCellType.HIDDEN_DOOR) {
                            southState = Passage.WallState.LOWER_OPEN;
                        } else if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END) {
                            southState = Passage.WallState.OPEN;
                        }

                        neighbourType = grid.GetWestNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.DOOR || neighbourType == MazeCellType.HIDDEN_DOOR) {
                            westState = Passage.WallState.LOWER_OPEN;
                        } else if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END) {
                            westState = Passage.WallState.OPEN;
                        }

                        if (x % 2 == 0) {
                            if (y == size - 2) { northState = Passage.WallState.WINDOW; }
                            else if (y == 1) { southState = Passage.WallState.WINDOW; }
                        } else if (y % 2 == 0) {
                            if (x == 1) { westState = Passage.WallState.WINDOW; }
                            else if (x == size - 2) { eastState = Passage.WallState.WINDOW; }
                        }

                        passage.transform.GetComponent<Passage>().Setup(transform, northState, eastState, southState, westState);

                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.white, false);

                        break;
                    case MazeCellType.DEAD_END:
                        GameObject deadEnd = Instantiate(deadEndPrefab, new Vector3(x, 0, y), Quaternion.identity);
                        deadEnd.transform.parent = deadEndParent.transform;

                        if (grid.GetNorthNeighbour(x, y).GetCellType() != MazeCellType.WALL) deadEnd.GetComponent<DeadEnd>().SetDirectionNorth();
                        else if (grid.GetEastNeighbour(x, y).GetCellType() != MazeCellType.WALL) deadEnd.GetComponent<DeadEnd>().SetDirectionEast();
                        else if (grid.GetSouthNeighbour(x, y).GetCellType() != MazeCellType.WALL) deadEnd.GetComponent<DeadEnd>().SetDirectionSouth();
                        else if (grid.GetWestNeighbour(x, y).GetCellType() != MazeCellType.WALL) deadEnd.GetComponent<DeadEnd>().SetDirectionWest();

                        foreach (Transform mazePiece in deadEnd.transform.Find("Maze Pieces")) grid.GetCell(x, y).AddToMazePieces(mazePiece.gameObject);
                        
                        lowerLightSources.Add(deadEnd.transform.Find("Maze Pieces").Find("Torch").gameObject, LightingType.TORCH_0);
                            
                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.white, false);

                        deadEnds.Add(grid.GetCell(x, y));

                        break;
                    default:
                        break;
                }
                if (Application.isEditor) writer.Write((int) grid.GetCell(y, x).GetCellType() + " ");
            }
            if (Application.isEditor) writer.Write("\n");
        }
        if (Application.isEditor) writer.Close();

        playerStart = deadEnds[Random.Range(0, deadEnds.Count)];
        gameController.SetPlayerPosition(new Vector3(playerStart.GetX(), 1, playerStart.GetY()));
        deadEnds.Remove(playerStart);

        lowerLightSources.Add(gameController.GetPlayer(), gameController.GetPlayer().GetComponent<PlayerBehaviour>().GetLighting());

        foreach (Transform knight in knightParent.transform) {
            knight.Find("Body").gameObject.GetComponent<KnightBehaviour>().SetPlayer(gameController.GetPlayer());
        }
    }

    private void MazeDepthFirstSearch(int x, int y) {
        grid.SetCellType(x, y, MazeCellType.PASSAGE);

        List<MazeCell> availableNeighbours = grid.GetSecondNeighboursOfType(x, y, MazeCellType.WALL, MazeCellType.DISCONNECTED_DOOR, MazeCellType.DISCONNECTED_HIDDEN_DOOR);

        while (availableNeighbours.Count != 0) {
            MazeCell chosenNeighbour = availableNeighbours[Random.Range(0, availableNeighbours.Count)];
            if (chosenNeighbour.GetCellType() == MazeCellType.WALL) {
                if      (chosenNeighbour == grid.GetSecondNorthNeighbour(x, y)) grid.SetCellType(x,     y + 1, MazeCellType.PASSAGE);
                else if (chosenNeighbour == grid.GetSecondEastNeighbour(x, y))  grid.SetCellType(x + 1, y,     MazeCellType.PASSAGE);
                else if (chosenNeighbour == grid.GetSecondSouthNeighbour(x, y)) grid.SetCellType(x,     y - 1, MazeCellType.PASSAGE);
                else if (chosenNeighbour == grid.GetSecondWestNeighbour(x, y))  grid.SetCellType(x - 1, y,     MazeCellType.PASSAGE);
                grid.SetCellType(chosenNeighbour.GetX(), chosenNeighbour.GetY(), MazeCellType.PASSAGE);
                MazeDepthFirstSearch(chosenNeighbour.GetX(), chosenNeighbour.GetY());
            } else if (chosenNeighbour.GetCellType() == MazeCellType.DISCONNECTED_DOOR) {
                if      (chosenNeighbour == grid.GetSecondNorthNeighbour(x, y)) grid.SetCellType(x,     y + 1, MazeCellType.DOOR);
                else if (chosenNeighbour == grid.GetSecondEastNeighbour(x, y))  grid.SetCellType(x + 1, y,     MazeCellType.DOOR);
                else if (chosenNeighbour == grid.GetSecondSouthNeighbour(x, y)) grid.SetCellType(x,     y - 1, MazeCellType.DOOR);
                else if (chosenNeighbour == grid.GetSecondWestNeighbour(x, y))  grid.SetCellType(x - 1, y,     MazeCellType.DOOR);
                grid.SetCellType(chosenNeighbour.GetX(), chosenNeighbour.GetY(), MazeCellType.ROOM);
            } else if (chosenNeighbour.GetCellType() == MazeCellType.DISCONNECTED_HIDDEN_DOOR) {
                if      (chosenNeighbour == grid.GetSecondNorthNeighbour(x, y)) grid.SetCellType(x,     y + 1, MazeCellType.HIDDEN_DOOR);
                else if (chosenNeighbour == grid.GetSecondEastNeighbour(x, y))  grid.SetCellType(x + 1, y,     MazeCellType.HIDDEN_DOOR);
                else if (chosenNeighbour == grid.GetSecondSouthNeighbour(x, y)) grid.SetCellType(x,     y - 1, MazeCellType.HIDDEN_DOOR);
                else if (chosenNeighbour == grid.GetSecondWestNeighbour(x, y))  grid.SetCellType(x - 1, y,     MazeCellType.HIDDEN_DOOR);
                grid.SetCellType(chosenNeighbour.GetX(), chosenNeighbour.GetY(), MazeCellType.HIDDEN_ROOM);
            }
            availableNeighbours.Remove(chosenNeighbour);
        }
    }

    public int GetSize() {
        return size;
    }

    public MazeCell GetMazeCell(float x, float y) {
        return grid.GetCell(x, y);
    }
}
