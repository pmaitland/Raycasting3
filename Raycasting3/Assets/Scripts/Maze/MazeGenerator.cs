using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MazeGenerator : MonoBehaviour {
    
    [Range(3, 49)]
    public int size = 21;
    [Range(0, 50)]
    public int roomCount = 6;

    public Texture2D stoneBrick;
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

    public GameObject floorPrefab;
    public GameObject ceilingPrefab;
    public GameObject wallPrefab;
    public GameObject doorPrefab;
    public GameObject hiddenDoorPrefab;

    public GameObject barrelPrefab;
    public GameObject torchPrefab;
    public GameObject chandelierPrefab;

    public GameObject turretPrefab;
    public GameObject knightPrefab;

    private GameObject floorParent;
    private GameObject ceilingParent;
    private GameObject wallParent;
    private GameObject doorParent;
    private GameObject propParent;
    private GameObject enemyParent;

    private GameBehaviour gameController;

    private MazeCell playerStart;

    void OnValidate() {
        if (size % 2 == 0) size -= 1;
    }

    void Start() {
        gameController = GameObject.Find("Controller").GetComponent<GameBehaviour>();

        floorParent = new GameObject("Floors");
        floorParent.transform.parent = transform;
        ceilingParent = new GameObject("Ceilings");
        ceilingParent.transform.parent = transform;
        wallParent = new GameObject("Walls");
        wallParent.transform.parent = transform;
        doorParent = new GameObject("Doors");
        doorParent.transform.parent = transform;
        propParent = new GameObject("Props");
        propParent.transform.parent = transform;
        enemyParent = new GameObject("Enemies");
        enemyParent.transform.parent = transform;

        GenerateMaze();
    }

    private void GenerateMaze() {
        MazeGrid grid = new MazeGrid(size);

        int[] roomDimensions = new int[] { 3, 5, 7 };

        List<MazeRoom> existingRooms = new List<MazeRoom>();
        for (int i = 0; i < roomCount; i++) {

            bool canBeCreated = false;
            int attemptCount = 0;

            while (!canBeCreated && attemptCount < 10) {
                canBeCreated = true;
                attemptCount++;

                int roomWidth  = roomDimensions[Random.Range(0, roomDimensions.Length - 1)];
                int roomHeight = roomDimensions[Random.Range(0, roomDimensions.Length - 1)];

                int roomX = Random.Range(3, (size - roomWidth) - 1);
                int roomY = Random.Range(3, (size - roomHeight) - 1);
                if (roomX % 2 == 0) roomX -= 1;
                if (roomY % 2 == 0) roomY -= 1;

                MazeRoom room = new MazeRoom(roomX, roomY, roomWidth, roomHeight);

                foreach (MazeRoom existingRoom in existingRooms) {
                    if (MazeRoom.Overlap(room, existingRoom)) canBeCreated = false;
                }

                if (canBeCreated) {
                    List<MazeCell> doorOptions = new List<MazeCell>();

                    for (int x = roomX; x < roomX + roomWidth; x++) {
                        for (int y = roomY; y < roomY + roomHeight; y++) {
                            if (i == 0) grid.SetCellType(x, y, MazeCellType.HIDDEN_ROOM);
                            else grid.SetCellType(x, y, MazeCellType.ROOM);

                            if (x % 2 == 1 && y % 2 == 1) {
                                if ((x == roomX || x == roomX + roomWidth - 1) || (y == roomY || y == roomY + roomHeight - 1)) {
                                    doorOptions.Add(grid.GetCell(x, y));
                                }
                            }
                        }
                    }

                    int doorCount = Random.Range(1, doorOptions.Count - 1);
                    for (int j = 0; j < doorCount; j++) {
                        MazeCell door = doorOptions[Random.Range(0, doorOptions.Count)];
                        if (i == 0) grid.SetCellType(door.GetX(), door.GetY(), MazeCellType.DISCONNECTED_HIDDEN_DOOR);
                        else grid.SetCellType(door.GetX(), door.GetY(), MazeCellType.DISCONNECTED_DOOR);
                        doorOptions.Remove(door);
                    }

                    existingRooms.Add(room);
                }
            }
        }

        MazeDepthFirstSearch(grid, 1, 1);

        grid.CleanUp();

        string path = "Assets/maze.txt";
        StreamWriter writer = new StreamWriter(path, false);

        List<MazeCell> deadEnds = new List<MazeCell>();

        int random = 0;
        MazeCell neighbour;
        MazeCellType neighbourType;
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                switch (grid.GetCell(x, y).GetCellType()) {
                    case MazeCellType.WALL:
                        neighbour = grid.GetNorthNeighbour(x, y);
                        if (neighbour != null) {
                            neighbourType = neighbour.GetCellType();
                            if (neighbourType != MazeCellType.WALL) {
                                if (neighbourType == MazeCellType.PASSAGE) {
                                    random = Random.Range(0, 100);
                                    if (random <= 5) CreateNorthWall(x, y, 0, banner);
                                    else CreateNorthWall(x, y, 0, stoneBrick);
                                } else {
                                    CreateNorthWall(x, y, 0, stoneBrick);
                                }
                            }
                            if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END
                                || neighbourType == MazeCellType.ROOM || neighbourType == MazeCellType.HIDDEN_ROOM) {
                                CreateNorthWall(x, y, 1, stoneBrick);
                            }
                        }

                        neighbour = grid.GetEastNeighbour(x, y);
                        if (neighbour != null) {
                            neighbourType = neighbour.GetCellType();
                            if (neighbourType != MazeCellType.WALL) {
                                if (neighbourType == MazeCellType.PASSAGE) {
                                    random = Random.Range(0, 100);
                                    if (random <= 5) CreateEastWall(x, y, 0, banner);
                                    else CreateEastWall(x, y, 0, stoneBrick);
                                } else {
                                    CreateEastWall(x, y, 0, stoneBrick);
                                }
                            }
                            if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END
                                || neighbourType == MazeCellType.ROOM || neighbourType == MazeCellType.HIDDEN_ROOM) {
                                CreateEastWall(x, y, 1, stoneBrick);
                            }
                        }

                        neighbour = grid.GetSouthNeighbour(x, y);
                        if (neighbour != null) {
                            neighbourType = neighbour.GetCellType();
                            if (neighbourType != MazeCellType.WALL) {
                                if (neighbourType == MazeCellType.PASSAGE) {
                                    random = Random.Range(0, 100);
                                    if (random <= 5) CreateSouthWall(x, y, 0, banner);
                                    else CreateSouthWall(x, y, 0, stoneBrick);
                                } else {
                                    CreateSouthWall(x, y, 0, stoneBrick);
                                }
                            }
                            if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END
                                || neighbourType == MazeCellType.ROOM || neighbourType == MazeCellType.HIDDEN_ROOM) {
                                CreateSouthWall(x, y, 1, stoneBrick);
                            }
                        }

                        neighbour = grid.GetWestNeighbour(x, y);
                        if (neighbour != null) {
                            neighbourType = neighbour.GetCellType();
                            if (neighbourType != MazeCellType.WALL) {
                                if (neighbourType == MazeCellType.PASSAGE) {
                                    random = Random.Range(0, 100);
                                    if (random <= 5) CreateWestWall(x, y, 0, banner);
                                    else CreateWestWall(x, y, 0, stoneBrick);
                                } else {
                                    CreateWestWall(x, y, 0, stoneBrick);
                                }
                            }
                            if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END
                                || neighbourType == MazeCellType.ROOM || neighbourType == MazeCellType.HIDDEN_ROOM) {
                                CreateWestWall(x, y, 1, stoneBrick);
                            }
                        }

                        break;
                    case MazeCellType.PASSAGE:
                        CreateCeiling(x, y, 1);

                        int passageNeighbourCount = 0;
                        bool hasPassageNorthNeighbour = false;
                        bool hasPassageEastNeighbour = false;
                        bool hasPassageSouthNeighbour = false;
                        bool hasPassageWestNeighbour = false;

                        neighbourType = grid.GetNorthNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.ROOM) {
                            CreateNorthWall(x, y, 1, stoneBrick);
                        } else if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END) {
                            passageNeighbourCount++;
                            hasPassageNorthNeighbour = true;
                        }

                        neighbourType = grid.GetEastNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.ROOM) {
                            CreateEastWall(x, y, 1, stoneBrick);
                        } else if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END) {
                            passageNeighbourCount++;
                            hasPassageEastNeighbour = true;
                        }

                        neighbourType = grid.GetSouthNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.ROOM) {
                            CreateSouthWall(x, y, 1, stoneBrick);
                        } else if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END) {
                            passageNeighbourCount++;
                            hasPassageSouthNeighbour = true;
                        }

                        neighbourType = grid.GetWestNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.ROOM) {
                            CreateWestWall(x, y, 1, stoneBrick);
                        } else if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END) {
                            passageNeighbourCount++;
                            hasPassageWestNeighbour = true;
                        }

                        if (passageNeighbourCount == 1) {
                            if (hasPassageNorthNeighbour) CreateFloor(x, y, carpetN);
                            else if (hasPassageEastNeighbour) CreateFloor(x, y, carpetE);
                            else if (hasPassageSouthNeighbour) CreateFloor(x, y, carpetS);
                            else if (hasPassageWestNeighbour) CreateFloor(x, y, carpetW);
                        } else if (passageNeighbourCount == 2) {
                            if (hasPassageNorthNeighbour) {
                                if (hasPassageEastNeighbour) CreateFloor(x, y, carpetNE);
                                else if (hasPassageSouthNeighbour) CreateFloor(x, y, carpetNS);
                                else if (hasPassageWestNeighbour) CreateFloor(x, y, carpetNW);
                            } else if (hasPassageEastNeighbour) {
                                if (hasPassageSouthNeighbour) CreateFloor(x, y, carpetES);
                                else if (hasPassageWestNeighbour) CreateFloor(x, y, carpetEW);
                            } else if (hasPassageSouthNeighbour) {
                                if (hasPassageWestNeighbour) CreateFloor(x, y, carpetSW);
                            }
                        } else if (passageNeighbourCount == 3) {
                            if (hasPassageNorthNeighbour && hasPassageEastNeighbour && hasPassageSouthNeighbour) CreateFloor(x, y, carpetNES);
                            else if (hasPassageEastNeighbour && hasPassageSouthNeighbour && hasPassageWestNeighbour) CreateFloor(x, y, carpetESW);
                            else if (hasPassageSouthNeighbour && hasPassageWestNeighbour && hasPassageNorthNeighbour) CreateFloor(x, y, carpetNSW);
                            else if (hasPassageWestNeighbour && hasPassageNorthNeighbour && hasPassageEastNeighbour) CreateFloor(x, y, carpetNEW);
                        } else if (passageNeighbourCount == 4) {
                            CreateFloor(x, y, carpetNESW);
                        } else {
                            CreateFloor(x, y, stoneBrick);
                        }

                        random = Random.Range(0, 100);
                        if (random >=  0 && random < 10) CreateChandelier(x, y, 1);

                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.white, false);

                        break;
                    case MazeCellType.DEAD_END:
                        CreateCeiling(x, y, 1);

                        MazeCellType northNeighbourType = grid.GetNorthNeighbour(x, y).GetCellType();
                        MazeCellType eastNeighbourType = grid.GetEastNeighbour(x, y).GetCellType();
                        MazeCellType southNeighbourType = grid.GetSouthNeighbour(x, y).GetCellType();
                        MazeCellType westNeighbourType = grid.GetWestNeighbour(x, y).GetCellType();

                        if (northNeighbourType != MazeCellType.WALL) {
                            if (northNeighbourType == MazeCellType.ROOM) CreateNorthWall(x, y, 0, stoneBrick);
                            CreateFloor(x, y, carpetN);
                            CreateSouthTorch(x, y, 0);
                        } else if (eastNeighbourType != MazeCellType.WALL) {
                            if (eastNeighbourType == MazeCellType.ROOM) CreateEastWall(x, y, 0, stoneBrick);
                            CreateFloor(x, y, carpetE);
                            CreateWestTorch(x, y, 0);
                        } else if (southNeighbourType != MazeCellType.WALL) {
                            if (southNeighbourType == MazeCellType.ROOM) CreateSouthWall(x, y, 0, stoneBrick);
                            CreateFloor(x, y, carpetS);
                            CreateNorthTorch(x, y, 0);
                        } else if (westNeighbourType != MazeCellType.WALL) {
                            if (westNeighbourType == MazeCellType.ROOM) CreateWestWall(x, y, 0, stoneBrick);
                            CreateFloor(x, y, carpetW);
                            CreateEastTorch(x, y, 0);
                        }

                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.white, false);

                        deadEnds.Add(grid.GetCell(x, y));

                        break;
                    case MazeCellType.ROOM:
                        CreateFloor(x, y, stoneBrick);
                        CreateCeiling(x, y, 1);

                        random = Random.Range(0, 100);
                        if (random >=  0 && random < 10) CreateTurret(x, y, 0);
                        if (random >= 10 && random < 30) CreateBarrel(x, y, 0);

                        random = Random.Range(0, 100);
                        if (random >=  0 && random < 10) {
                            if (grid.GetNorthNeighbour(x, y).GetCellType() == MazeCellType.WALL) CreateNorthTorch(x, y, 0);
                            else if (grid.GetEastNeighbour(x, y).GetCellType() == MazeCellType.WALL) CreateEastTorch(x, y, 0);
                            else if (grid.GetSouthNeighbour(x, y).GetCellType() == MazeCellType.WALL) CreateSouthTorch(x, y, 0);
                            else if (grid.GetWestNeighbour(x, y).GetCellType() == MazeCellType.WALL) CreateWestTorch(x, y, 0);
                        }

                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.gray, false);

                        break;
                    case MazeCellType.HIDDEN_ROOM:
                        CreateFloor(x, y, stoneBrick);
                        CreateCeiling(x, y, 1);

                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.black, false);

                        break;
                    case MazeCellType.DOOR:
                        CreateFloor(x, y, stoneBrick);
                        CreateCeiling(x, y, 0);

                        neighbourType = grid.GetNorthNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END || neighbourType == MazeCellType.ROOM) {
                            CreateNorthWall(x, y, 1, stoneBrick);
                            if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END) CreateSouthDoor(x, y);
                        }
                        neighbourType = grid.GetEastNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END || neighbourType == MazeCellType.ROOM) {
                            CreateEastWall(x, y, 1, stoneBrick);
                            if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END) CreateWestDoor(x, y);
                        }
                        neighbourType = grid.GetSouthNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END || neighbourType == MazeCellType.ROOM) {
                            CreateSouthWall(x, y, 1, stoneBrick);
                            if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END) CreateNorthDoor(x, y);
                        }
                        neighbourType = grid.GetWestNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END || neighbourType == MazeCellType.ROOM) {
                            CreateWestWall(x, y, 1, stoneBrick);
                            if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END) CreateEastDoor(x, y);
                        }

                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.gray, false);

                        break;
                    case MazeCellType.HIDDENDOOR:
                        CreateCeiling(x, y, 0);

                        neighbourType = grid.GetNorthNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END || neighbourType == MazeCellType.HIDDEN_ROOM) {
                            CreateNorthWall(x, y, 1, stoneBrick);
                            if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END) CreateSouthHiddenDoor(x, y);
                        }
                        neighbourType = grid.GetEastNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END || neighbourType == MazeCellType.HIDDEN_ROOM) {
                            CreateEastWall(x, y, 1, stoneBrick);
                            if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END) CreateWestHiddenDoor(x, y);
                        }
                        neighbourType = grid.GetSouthNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END || neighbourType == MazeCellType.HIDDEN_ROOM) {
                            CreateSouthWall(x, y, 1, stoneBrick);
                            if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END) CreateNorthHiddenDoor(x, y);
                        }
                        neighbourType = grid.GetWestNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END || neighbourType == MazeCellType.HIDDEN_ROOM) {
                            CreateWestWall(x, y, 1, stoneBrick);
                            if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END) CreateEastHiddenDoor(x, y);
                        }

                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.black, false);

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

        foreach (MazeCell deadEnd in deadEnds) CreateKnight(deadEnd.GetX(), deadEnd.GetY(), 0, grid);
    }

    private void MazeDepthFirstSearch(MazeGrid grid, int x, int y) {
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
                MazeDepthFirstSearch(grid, chosenNeighbour.GetX(), chosenNeighbour.GetY());
            } else if (chosenNeighbour.GetCellType() == MazeCellType.DISCONNECTED_DOOR) {
                if      (chosenNeighbour == grid.GetSecondNorthNeighbour(x, y)) grid.SetCellType(x,     y - 1, MazeCellType.DOOR);
                else if (chosenNeighbour == grid.GetSecondEastNeighbour(x, y))  grid.SetCellType(x + 1, y,     MazeCellType.DOOR);
                else if (chosenNeighbour == grid.GetSecondSouthNeighbour(x, y)) grid.SetCellType(x,     y + 1, MazeCellType.DOOR);
                else if (chosenNeighbour == grid.GetSecondWestNeighbour(x, y))  grid.SetCellType(x - 1, y,     MazeCellType.DOOR);
                grid.SetCellType(chosenNeighbour.GetX(), chosenNeighbour.GetY(), MazeCellType.ROOM);
            } else if (chosenNeighbour.GetCellType() == MazeCellType.DISCONNECTED_HIDDEN_DOOR) {
                if      (chosenNeighbour == grid.GetSecondNorthNeighbour(x, y)) grid.SetCellType(x,     y - 1, MazeCellType.HIDDENDOOR);
                else if (chosenNeighbour == grid.GetSecondEastNeighbour(x, y))  grid.SetCellType(x + 1, y,     MazeCellType.HIDDENDOOR);
                else if (chosenNeighbour == grid.GetSecondSouthNeighbour(x, y)) grid.SetCellType(x,     y + 1, MazeCellType.HIDDENDOOR);
                else if (chosenNeighbour == grid.GetSecondWestNeighbour(x, y))  grid.SetCellType(x - 1, y,     MazeCellType.HIDDENDOOR);
                grid.SetCellType(chosenNeighbour.GetX(), chosenNeighbour.GetY(), MazeCellType.HIDDEN_ROOM);
            }
            availableNeighbours.Remove(chosenNeighbour);
        }
    }

    private void CreateFloor(int x, int y, Texture2D texture) {
        GameObject floor = CreateMazePiece(floorPrefab, new Vector3(x, 0, y) + floorPrefab.transform.position, texture);
        floor.transform.parent = floorParent.transform;
    }

    private void CreateCeiling(int x, int y, int level) {
        GameObject ceiling = CreateMazePiece(ceilingPrefab, new Vector3(x, level, y) + ceilingPrefab.transform.position, stoneBrick);
        ceiling.transform.parent = ceilingParent.transform;
    }

    private void CreateNorthWall(int x, int y, int level, Texture2D texture) {
        CreateWall(new Vector3(x, level, y) + new Vector3(0, 0.5f, -0.5f), 0f, texture);
    }

    private void CreateEastWall(int x, int y, int level, Texture2D texture) {
        CreateWall(new Vector3(x, level, y) + new Vector3(0.5f, 0.5f, 0), 270f, texture);
    }

    private void CreateSouthWall(int x, int y, int level, Texture2D texture) {
        CreateWall(new Vector3(x, level, y) + new Vector3(0, 0.5f, 0.5f), 180f, texture);
    }

    private void CreateWestWall(int x, int y, int level, Texture2D texture) {
        CreateWall(new Vector3(x, level, y) + new Vector3(-0.5f, 0.5f, 0), 90f, texture);
    }

    private void CreateWall(Vector3 position, float rotation, Texture2D texture) {
        GameObject wall = CreateMazePiece(wallPrefab, position, texture);
        wall.transform.Rotate(0, rotation, 0, Space.Self);
        wall.transform.parent = wallParent.transform;
    }

    private void CreateNorthDoor(int x, int y) {
        CreateDoor(x, y, 0);
    }

    private void CreateEastDoor(int x, int y) {
        CreateDoor(x, y, 270);
    }

    private void CreateSouthDoor(int x, int y) {
        CreateDoor(x, y, 180);
    }

    private void CreateWestDoor(int x, int y) {
        CreateDoor(x, y, 90);
    }

    private void CreateDoor(int x, int y, float rotation) {
        GameObject door = CreateObject(doorPrefab, new Vector3(x, 0, y) + doorPrefab.transform.position);
        door.transform.Rotate(0, rotation, 0, Space.Self);
        door.transform.parent = doorParent.transform;
    }

    private void CreateNorthHiddenDoor(int x, int y) {
        GameObject hiddenDoor = CreateHiddenDoor(x, y, 0);
    }
    
    private void CreateEastHiddenDoor(int x, int y) {
        GameObject hiddenDoor = CreateHiddenDoor(x, y, 90);
        hiddenDoor.transform.Find("Top").Rotate(0, 0, 90);
    }

    private void CreateSouthHiddenDoor(int x, int y) {
        GameObject hiddenDoor = CreateHiddenDoor(x, y, 0);
    }

    private void CreateWestHiddenDoor(int x, int y) {
        GameObject hiddenDoor = CreateHiddenDoor(x, y, 90);
        hiddenDoor.transform.Find("Top").Rotate(0, 0, 90);
    }

    private GameObject CreateHiddenDoor(int x, int y, float rotation) {
        GameObject hiddenDoor = CreateObject(hiddenDoorPrefab, new Vector3(x, 0, y) + hiddenDoorPrefab.transform.position);
        hiddenDoor.transform.Rotate(0, rotation, 0, Space.Self);
        hiddenDoor.transform.parent = doorParent.transform;
        return hiddenDoor;
    }

    private GameObject CreateMazePiece(GameObject prefab, Vector3 position, Texture2D texture) {
        GameObject mazePiece = CreateObject(prefab, position);

        MeshRenderer meshRenderer = mazePiece.GetComponent<MeshRenderer>();
        if (meshRenderer != null) meshRenderer.material.mainTexture = texture;

        return mazePiece;
    }

    private void CreateBarrel(int x, int y, int level) {
        GameObject barrel = CreateObject(barrelPrefab, new Vector3(x, level, y) + new Vector3(barrelPrefab.transform.position.x, barrelPrefab.transform.position.y, 0));
        barrel.transform.parent = propParent.transform;
    }

    private void CreateNorthTorch(int x, int y, int level) {
        CreateTorch(new Vector3(x, level, y) + new Vector3(0, 0, -0.4f), 0f);
    }

    private void CreateEastTorch(int x, int y, int level) {
        CreateTorch(new Vector3(x, level, y) + new Vector3(0.4f, 0, 0), 270f);
    }

    private void CreateSouthTorch(int x, int y, int level) {
        CreateTorch(new Vector3(x, level, y) + new Vector3(0, 0, 0.4f), 180f);
    }

    private void CreateWestTorch(int x, int y, int level) {
        CreateTorch(new Vector3(x, level, y) + new Vector3(-0.4f, 0, 0), 90f);
    }

    private void CreateTorch(Vector3 position, float rotation) {
        GameObject torch = CreateObject(torchPrefab, position + new Vector3(0, 0.5f, 0));
        torch.transform.Rotate(0, rotation, 0, Space.Self);
        torch.transform.parent = propParent.transform;
        if (Application.isEditor) torch.transform.Find("Directional Light").gameObject.SetActive(false);
    }

    private void CreateChandelier(int x, int y, int level) {
        GameObject chandelier = CreateObject(chandelierPrefab, new Vector3(x, level, y) + new Vector3(0, 0.75f, 0));
        chandelier.transform.parent = propParent.transform;
        if (Application.isEditor) chandelier.transform.Find("Directional Light").gameObject.SetActive(false);
    }

    private void CreateTurret(int x, int y, int level) {
        GameObject turret = CreateObject(turretPrefab, new Vector3(x, level, y) + new Vector3(turretPrefab.transform.position.x, turretPrefab.transform.position.y, 0));
        turret.transform.parent = enemyParent.transform;
    }

    private void CreateKnight(int x, int y, int level, MazeGrid grid) {
        GameObject knight = CreateObject(knightPrefab, new Vector3(x, level, y) + new Vector3(knightPrefab.transform.position.x, knightPrefab.transform.position.y, 0));
        knight.transform.parent = enemyParent.transform;
        knight.GetComponentInChildren<KnightBehaviour>().SetPlayer(gameController.GetPlayer());
    }

    private GameObject CreateObject(GameObject prefab, Vector3 position) {
        GameObject obj = Instantiate(prefab, position, prefab.transform.rotation);
        return obj;
    }

    public int GetSize() {
        return size;
    }
}
