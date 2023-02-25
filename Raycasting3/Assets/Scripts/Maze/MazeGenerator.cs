using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MazeGenerator : MonoBehaviour {
    
    [Range(3, 49)]
    public int size = 21;
    [Range(0, 50)]
    public int roomCount = 6;

    public Texture2D banner;

    public Texture2D carpetN;
    public Texture2D carpetE;
    public Texture2D carpetS;
    public Texture2D carpetW; 
    public Texture2D carpetNS;
    public Texture2D carpetEW;
    public Texture2D carpetNE;
    public Texture2D carpetES;
    public Texture2D carpetSW;
    public Texture2D carpetNW;
    public Texture2D carpetNES;
    public Texture2D carpetESW;
    public Texture2D carpetNSW;
    public Texture2D carpetNEW;
    public Texture2D carpetNESW;

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

        GenerateMaze();
    }

    void Update() {
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                grid.GetCell(x, y).SetLightingLower(LightingType.DARKNESS);
                grid.GetCell(x, y).SetLightingUpper(LightingType.DARKNESS);
            }
        }

        lowerLightSources[gameController.GetPlayer()] = gameController.GetPlayer().GetComponent<PlayerBehaviour>().GetLighting();

        List<GameObject> lightSourcesToRemove = new List<GameObject>();

        foreach (KeyValuePair<GameObject, LightingType> entry in lowerLightSources) {
            GameObject lightSource = entry.Key;
            if (lightSource != null) {
                if (Vector3.Distance(gameController.GetPlayer().transform.position, lightSource.transform.position) < 15) {
                    MazeCell cell = grid.GetCell(lightSource.transform.position.x, lightSource.transform.position.z);
                    grid.SetLightingLower(cell, entry.Value);
                }
            } else {
                lightSourcesToRemove.Add(lightSource);
            }
        }
        foreach (GameObject lightSource in lightSourcesToRemove) lowerLightSources.Remove(lightSource);

        foreach (KeyValuePair<GameObject, LightingType> entry in upperLightSources) {
            GameObject lightSource = entry.Key;
            if (lightSource != null) {
                if (Vector3.Distance(gameController.GetPlayer().transform.position, lightSource.transform.position) < 15) {
                    MazeCell cell = grid.GetCell(lightSource.transform.position.x, lightSource.transform.position.z);
                    grid.SetLightingUpper(cell, entry.Value);
                }
            } else {
                lightSourcesToRemove.Add(lightSource);
            }
        }
        foreach (GameObject lightSource in lightSourcesToRemove) upperLightSources.Remove(lightSource);
    }

    public void AddLowerLightSource(GameObject lightSource, LightingType lightingType) {
        lowerLightSources.Add(lightSource, lightingType);
    }

    public void AddUpperLightSource(GameObject lightSource, LightingType lightingType) {
        upperLightSources.Add(lightSource, lightingType);
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
                            if (roomPiece.position.y == 0) AddLowerLightSource(roomPiece.gameObject, LightingType.TORCH_0);
                            else if (roomPiece.position.y == 1) AddUpperLightSource(roomPiece.gameObject, LightingType.TORCH_0);
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

        int random = 0;
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                MazeCell currentCell = grid.GetCell(x, y);
                switch (currentCell.GetCellType()) {
                    case MazeCellType.PASSAGE:
                        GameObject passage = Instantiate(passagePrefab, new Vector3(x, 0, y), passagePrefab.transform.rotation);

                        int passageNeighbourCount = 0;
                        bool hasPassageNorthNeighbour = false;
                        bool hasPassageEastNeighbour = false;
                        bool hasPassageSouthNeighbour = false;
                        bool hasPassageWestNeighbour = false;

                        MazeCellType neighbourType = grid.GetNorthNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.DOOR || neighbourType == MazeCellType.HIDDEN_DOOR) {
                            foreach (Transform mazePiece in passage.transform.Find("Maze Pieces")) {
                                if (mazePiece.name == "Wall-N-L0") mazePiece.gameObject.SetActive(false);
                            }
                        } else if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END) {
                            foreach (Transform mazePiece in passage.transform.Find("Maze Pieces")) {
                                if (mazePiece.name == "Wall-N-L0") mazePiece.gameObject.SetActive(false);
                                if (mazePiece.name == "Wall-N-L1") mazePiece.gameObject.SetActive(false);
                            }
                            passageNeighbourCount++;
                            hasPassageNorthNeighbour = true;
                        }

                        neighbourType = grid.GetEastNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.DOOR || neighbourType == MazeCellType.HIDDEN_DOOR) {
                            foreach (Transform mazePiece in passage.transform.Find("Maze Pieces")) {
                                if (mazePiece.name == "Wall-E-L0") mazePiece.gameObject.SetActive(false);
                            }
                        } else if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END) {
                            foreach (Transform mazePiece in passage.transform.Find("Maze Pieces")) {
                                if (mazePiece.name == "Wall-E-L0") mazePiece.gameObject.SetActive(false);
                                if (mazePiece.name == "Wall-E-L1") mazePiece.gameObject.SetActive(false);
                            }
                            passageNeighbourCount++;
                            hasPassageEastNeighbour = true;
                        }

                        neighbourType = grid.GetSouthNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.DOOR || neighbourType == MazeCellType.HIDDEN_DOOR) {
                            foreach (Transform mazePiece in passage.transform.Find("Maze Pieces")) {
                                if (mazePiece.name == "Wall-S-L0") mazePiece.gameObject.SetActive(false);
                            }
                        } else if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END) {
                            foreach (Transform mazePiece in passage.transform.Find("Maze Pieces")) {
                                if (mazePiece.name == "Wall-S-L0") mazePiece.gameObject.SetActive(false);
                                if (mazePiece.name == "Wall-S-L1") mazePiece.gameObject.SetActive(false);
                            }
                            passageNeighbourCount++;
                            hasPassageSouthNeighbour = true;
                        }

                        neighbourType = grid.GetWestNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.DOOR || neighbourType == MazeCellType.HIDDEN_DOOR) {
                            foreach (Transform mazePiece in passage.transform.Find("Maze Pieces")) {
                                if (mazePiece.name == "Wall-W-L0") mazePiece.gameObject.SetActive(false);
                            }
                        } else if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END) {
                            foreach (Transform mazePiece in passage.transform.Find("Maze Pieces")) {
                                if (mazePiece.name == "Wall-W-L0") mazePiece.gameObject.SetActive(false);
                                if (mazePiece.name == "Wall-W-L1") mazePiece.gameObject.SetActive(false);
                            }
                            passageNeighbourCount++;
                            hasPassageWestNeighbour = true;
                        }

                        foreach (Transform mazePiece in passage.transform.Find("Maze Pieces")) {
                            if (mazePiece.name.Contains("Wall") && mazePiece.name.Contains("L0") && !mazePiece.name.Contains("Window")) {
                                random = Random.Range(0, 100);
                                if (random >=  0 && random < 10) {
                                    mazePiece.GetComponent<MeshRenderer>().material.mainTexture = banner;
                                }
                            }
                        }

                        if (x % 2 == 0) {
                            if (y == 1 || y == size - 2) {
                                AddLowerLightSource(passage.transform.Find("Maze Pieces").Find("Ceiling").gameObject, LightingType.LIGHT_SPELL_0);
                                AddUpperLightSource(passage.transform.Find("Maze Pieces").Find("Floor").gameObject, LightingType.LIGHT_SPELL_0);

                                if (y == 1) {
                                    passage.transform.Find("Maze Pieces").Find("Wall-N-L0").gameObject.SetActive(false);
                                    passage.transform.Find("Maze Pieces").Find("Wall-N-L1").gameObject.SetActive(false);
                                    passage.transform.Find("Maze Pieces").Find("Wall-N-L0-Window").gameObject.SetActive(true);
                                    passage.transform.Find("Maze Pieces").Find("Wall-N-L1-Window").gameObject.SetActive(true);
                                }

                                if (y == size - 2) {
                                    passage.transform.Find("Maze Pieces").Find("Wall-S-L0").gameObject.SetActive(false);
                                    passage.transform.Find("Maze Pieces").Find("Wall-S-L1").gameObject.SetActive(false);
                                    passage.transform.Find("Maze Pieces").Find("Wall-S-L0-Window").gameObject.SetActive(true);
                                    passage.transform.Find("Maze Pieces").Find("Wall-S-L1-Window").gameObject.SetActive(true);
                                }
                            }
                        } else if (y % 2 == 0) {
                            if (x == 1 || x == size - 2) {
                                AddLowerLightSource(passage.transform.Find("Maze Pieces").Find("Ceiling").gameObject, LightingType.LIGHT_SPELL_0);
                                AddUpperLightSource(passage.transform.Find("Maze Pieces").Find("Floor").gameObject, LightingType.LIGHT_SPELL_0);

                                if (x == 1) {
                                    passage.transform.Find("Maze Pieces").Find("Wall-W-L0").gameObject.SetActive(false);
                                    passage.transform.Find("Maze Pieces").Find("Wall-W-L1").gameObject.SetActive(false);
                                    passage.transform.Find("Maze Pieces").Find("Wall-W-L0-Window").gameObject.SetActive(true);
                                    passage.transform.Find("Maze Pieces").Find("Wall-W-L1-Window").gameObject.SetActive(true);
                                }

                                if (x == size - 2) {
                                    passage.transform.Find("Maze Pieces").Find("Wall-E-L0").gameObject.SetActive(false);
                                    passage.transform.Find("Maze Pieces").Find("Wall-E-L1").gameObject.SetActive(false);
                                    passage.transform.Find("Maze Pieces").Find("Wall-E-L0-Window").gameObject.SetActive(true);
                                    passage.transform.Find("Maze Pieces").Find("Wall-E-L1-Window").gameObject.SetActive(true);
                                }
                            }
                        }

                        Material floorMaterial = passage.transform.Find("Maze Pieces").Find("Floor").GetComponent<MeshRenderer>().material;
                        if (passageNeighbourCount == 1) {
                            if (hasPassageNorthNeighbour) floorMaterial.mainTexture = carpetN;
                            else if (hasPassageEastNeighbour) floorMaterial.mainTexture = carpetE;
                            else if (hasPassageSouthNeighbour) floorMaterial.mainTexture = carpetS;
                            else if (hasPassageWestNeighbour) floorMaterial.mainTexture = carpetW;
                        } else if (passageNeighbourCount == 2) {
                            if (hasPassageNorthNeighbour) {
                                if (hasPassageEastNeighbour) floorMaterial.mainTexture = carpetNE;
                                else if (hasPassageSouthNeighbour) floorMaterial.mainTexture = carpetNS;
                                else if (hasPassageWestNeighbour) floorMaterial.mainTexture = carpetNW;
                            } else if (hasPassageEastNeighbour) {
                                if (hasPassageSouthNeighbour) floorMaterial.mainTexture = carpetES;
                                else if (hasPassageWestNeighbour) floorMaterial.mainTexture = carpetEW;
                            } else if (hasPassageSouthNeighbour) {
                                if (hasPassageWestNeighbour) floorMaterial.mainTexture = carpetSW;
                            }
                        } else if (passageNeighbourCount == 3) {
                            if (hasPassageNorthNeighbour && hasPassageEastNeighbour && hasPassageSouthNeighbour) floorMaterial.mainTexture = carpetNES;
                            else if (hasPassageEastNeighbour && hasPassageSouthNeighbour && hasPassageWestNeighbour) floorMaterial.mainTexture = carpetESW;
                            else if (hasPassageSouthNeighbour && hasPassageWestNeighbour && hasPassageNorthNeighbour) floorMaterial.mainTexture = carpetNSW;
                            else if (hasPassageWestNeighbour && hasPassageNorthNeighbour && hasPassageEastNeighbour) floorMaterial.mainTexture = carpetNEW;

                            passage.transform.Find("Maze Pieces").Find("Chandelier").gameObject.SetActive(true);
                            AddUpperLightSource(passage.transform.Find("Maze Pieces").Find("Chandelier").gameObject, LightingType.TORCH_0);
                        } else if (passageNeighbourCount == 4) {
                            floorMaterial.mainTexture = carpetNESW;
                            
                            passage.transform.Find("Maze Pieces").Find("Chandelier").gameObject.SetActive(true);
                            AddUpperLightSource(passage.transform.Find("Maze Pieces").Find("Chandelier").gameObject, LightingType.TORCH_0);
                        }

                        passage.transform.parent = passageParent.transform;

                        foreach (Transform mazePiece in passage.transform.Find("Maze Pieces")) grid.GetCell(x, y).AddToMazePieces(mazePiece.gameObject);

                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.white, false);

                        break;
                    case MazeCellType.DEAD_END:
                        float rotationAmount = 0;
                        if (grid.GetEastNeighbour(x, y).GetCellType() != MazeCellType.WALL) rotationAmount = 270;
                        else if (grid.GetSouthNeighbour(x, y).GetCellType() != MazeCellType.WALL) rotationAmount = 180;
                        else if (grid.GetWestNeighbour(x, y).GetCellType() != MazeCellType.WALL) rotationAmount = 90;

                        Quaternion rotation = Quaternion.Euler(0, rotationAmount, 0);

                        GameObject deadEnd = Instantiate(deadEndPrefab, new Vector3(x, 0, y), rotation);
                        deadEnd.transform.parent = deadEndParent.transform;

                        Transform floor = deadEnd.transform.Find("Maze Pieces").Find("Floor");
                        floor.Rotate(0, 0, rotationAmount);
                        Material material = floor.GetComponent<MeshRenderer>().material;
                        if (grid.GetNorthNeighbour(x, y).GetCellType() == MazeCellType.PASSAGE) {
                            material.mainTexture = carpetN;
                        } else if (grid.GetEastNeighbour(x, y).GetCellType() == MazeCellType.PASSAGE) {
                            material.mainTexture = carpetE;
                        } else if (grid.GetSouthNeighbour(x, y).GetCellType() == MazeCellType.PASSAGE) {
                            material.mainTexture = carpetS;
                        } else if (grid.GetWestNeighbour(x, y).GetCellType() == MazeCellType.PASSAGE) {
                            material.mainTexture = carpetW;
                        }

                        foreach (Transform mazePiece in deadEnd.transform.Find("Maze Pieces")) grid.GetCell(x, y).AddToMazePieces(mazePiece.gameObject);
                        
                        AddLowerLightSource(deadEnd.transform.Find("Maze Pieces").Find("Torch").gameObject, LightingType.TORCH_0);
                            
                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.white, false);

                        deadEnds.Add(grid.GetCell(x, y));

                        GameObject knight = Instantiate(knightPrefab, new Vector3(x, 0, y), rotation);
                        knight.transform.parent = knightParent.transform;

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
                if      (chosenNeighbour == grid.GetSecondNorthNeighbour(x, y)) grid.SetCellType(x,     y - 1, MazeCellType.PASSAGE);
                else if (chosenNeighbour == grid.GetSecondEastNeighbour(x, y))  grid.SetCellType(x + 1, y,     MazeCellType.PASSAGE);
                else if (chosenNeighbour == grid.GetSecondSouthNeighbour(x, y)) grid.SetCellType(x,     y + 1, MazeCellType.PASSAGE);
                else if (chosenNeighbour == grid.GetSecondWestNeighbour(x, y))  grid.SetCellType(x - 1, y,     MazeCellType.PASSAGE);
                grid.SetCellType(chosenNeighbour.GetX(), chosenNeighbour.GetY(), MazeCellType.PASSAGE);
                MazeDepthFirstSearch(chosenNeighbour.GetX(), chosenNeighbour.GetY());
            } else if (chosenNeighbour.GetCellType() == MazeCellType.DISCONNECTED_DOOR) {
                if      (chosenNeighbour == grid.GetSecondNorthNeighbour(x, y)) grid.SetCellType(x,     y - 1, MazeCellType.DOOR);
                else if (chosenNeighbour == grid.GetSecondEastNeighbour(x, y))  grid.SetCellType(x + 1, y,     MazeCellType.DOOR);
                else if (chosenNeighbour == grid.GetSecondSouthNeighbour(x, y)) grid.SetCellType(x,     y + 1, MazeCellType.DOOR);
                else if (chosenNeighbour == grid.GetSecondWestNeighbour(x, y))  grid.SetCellType(x - 1, y,     MazeCellType.DOOR);
                grid.SetCellType(chosenNeighbour.GetX(), chosenNeighbour.GetY(), MazeCellType.ROOM);
            } else if (chosenNeighbour.GetCellType() == MazeCellType.DISCONNECTED_HIDDEN_DOOR) {
                if      (chosenNeighbour == grid.GetSecondNorthNeighbour(x, y)) grid.SetCellType(x,     y - 1, MazeCellType.HIDDEN_DOOR);
                else if (chosenNeighbour == grid.GetSecondEastNeighbour(x, y))  grid.SetCellType(x + 1, y,     MazeCellType.HIDDEN_DOOR);
                else if (chosenNeighbour == grid.GetSecondSouthNeighbour(x, y)) grid.SetCellType(x,     y + 1, MazeCellType.HIDDEN_DOOR);
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
