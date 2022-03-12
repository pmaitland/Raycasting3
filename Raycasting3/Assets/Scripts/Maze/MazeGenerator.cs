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

    private MazeGrid grid;
    private MazeCell playerStart;

    private Dictionary<GameObject, LightingType> lightSources;

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

        lightSources = new Dictionary<GameObject, LightingType>();

        GenerateMaze();
    }

    void Update() {
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                grid.GetCell(x, y).SetLighting(LightingType.DARKNESS);
            }
        }

        foreach (KeyValuePair<GameObject, LightingType> entry in lightSources) {
            MazeCell cell = grid.GetCell(entry.Key.transform.position.x, entry.Key.transform.position.z);
            grid.SetLighting(cell, entry.Value);
        }
    }

    private void GenerateMaze() {
        grid = new MazeGrid(size);

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

        MazeDepthFirstSearch(1, 1);

        grid.CleanUp();

        string path = "Assets/maze.txt";
        StreamWriter writer = new StreamWriter(path, false);

        List<MazeCell> deadEnds = new List<MazeCell>();

        int random = 0;
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                MazeCell currentCell = grid.GetCell(x, y);
                switch (currentCell.GetCellType()) {
                    case MazeCellType.PASSAGE:
                        CreateCeiling(currentCell, 1);

                        int passageNeighbourCount = 0;
                        bool hasPassageNorthNeighbour = false;
                        bool hasPassageEastNeighbour = false;
                        bool hasPassageSouthNeighbour = false;
                        bool hasPassageWestNeighbour = false;

                        MazeCellType neighbourType = grid.GetNorthNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.WALL) {
                            CreateNorthWall(currentCell, 0, stoneBrick);
                            CreateNorthWall(currentCell, 1, stoneBrick);
                        } else if (neighbourType == MazeCellType.DOOR || neighbourType == MazeCellType.HIDDEN_DOOR) {
                            CreateNorthWall(currentCell, 1, stoneBrick);
                        } else if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END) {
                            passageNeighbourCount++;
                            hasPassageNorthNeighbour = true;
                        }

                        neighbourType = grid.GetEastNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.WALL) {
                            CreateEastWall(currentCell, 0, stoneBrick);
                            CreateEastWall(currentCell, 1, stoneBrick);
                        } else if (neighbourType == MazeCellType.DOOR || neighbourType == MazeCellType.HIDDEN_DOOR) {
                            CreateEastWall(currentCell, 1, stoneBrick);
                        } else if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END) {
                            passageNeighbourCount++;
                            hasPassageEastNeighbour = true;
                        }

                        neighbourType = grid.GetSouthNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.WALL) {
                            CreateSouthWall(currentCell, 0, stoneBrick);
                            CreateSouthWall(currentCell, 1, stoneBrick);
                        } else if (neighbourType == MazeCellType.DOOR || neighbourType == MazeCellType.HIDDEN_DOOR) {
                            CreateSouthWall(currentCell, 1, stoneBrick);
                        } else if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END) {
                            passageNeighbourCount++;
                            hasPassageSouthNeighbour = true;
                        }

                        neighbourType = grid.GetWestNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.WALL) {
                            CreateWestWall(currentCell, 0, stoneBrick);
                            CreateWestWall(currentCell, 1, stoneBrick);
                        } else if (neighbourType == MazeCellType.DOOR || neighbourType == MazeCellType.HIDDEN_DOOR) {
                            CreateWestWall(currentCell, 1, stoneBrick);
                        } else if (neighbourType == MazeCellType.PASSAGE || neighbourType == MazeCellType.DEAD_END) {
                            passageNeighbourCount++;
                            hasPassageWestNeighbour = true;
                        }

                        if (passageNeighbourCount == 1) {
                            if (hasPassageNorthNeighbour) CreateFloor(currentCell, carpetN);
                            else if (hasPassageEastNeighbour) CreateFloor(currentCell, carpetE);
                            else if (hasPassageSouthNeighbour) CreateFloor(currentCell, carpetS);
                            else if (hasPassageWestNeighbour) CreateFloor(currentCell, carpetW);
                        } else if (passageNeighbourCount == 2) {
                            if (hasPassageNorthNeighbour) {
                                if (hasPassageEastNeighbour) CreateFloor(currentCell, carpetNE);
                                else if (hasPassageSouthNeighbour) CreateFloor(currentCell, carpetNS);
                                else if (hasPassageWestNeighbour) CreateFloor(currentCell, carpetNW);
                            } else if (hasPassageEastNeighbour) {
                                if (hasPassageSouthNeighbour) CreateFloor(currentCell, carpetES);
                                else if (hasPassageWestNeighbour) CreateFloor(currentCell, carpetEW);
                            } else if (hasPassageSouthNeighbour) {
                                if (hasPassageWestNeighbour) CreateFloor(currentCell, carpetSW);
                            }
                        } else if (passageNeighbourCount == 3) {
                            if (hasPassageNorthNeighbour && hasPassageEastNeighbour && hasPassageSouthNeighbour) CreateFloor(currentCell, carpetNES);
                            else if (hasPassageEastNeighbour && hasPassageSouthNeighbour && hasPassageWestNeighbour) CreateFloor(currentCell, carpetESW);
                            else if (hasPassageSouthNeighbour && hasPassageWestNeighbour && hasPassageNorthNeighbour) CreateFloor(currentCell, carpetNSW);
                            else if (hasPassageWestNeighbour && hasPassageNorthNeighbour && hasPassageEastNeighbour) CreateFloor(currentCell, carpetNEW);
                        } else if (passageNeighbourCount == 4) {
                            CreateFloor(currentCell, carpetNESW);
                        } else {
                            CreateFloor(currentCell, stoneBrick);
                        }

                        // random = Random.Range(0, 100);
                        // if (random >=  0 && random < 10) CreateChandelier(currentCell, 1);

                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.white, false);

                        break;
                    case MazeCellType.DEAD_END:
                        CreateCeiling(currentCell, 1);

                        MazeCellType northNeighbourType = grid.GetNorthNeighbour(x, y).GetCellType();
                        MazeCellType eastNeighbourType = grid.GetEastNeighbour(x, y).GetCellType();
                        MazeCellType southNeighbourType = grid.GetSouthNeighbour(x, y).GetCellType();
                        MazeCellType westNeighbourType = grid.GetWestNeighbour(x, y).GetCellType();

                        if (northNeighbourType != MazeCellType.WALL) {
                            CreateFloor(currentCell, carpetN);

                            CreateEastWall(currentCell, 0, stoneBrick);
                            CreateSouthWall(currentCell, 0, stoneBrick);
                            CreateWestWall(currentCell, 0, stoneBrick);

                            CreateEastWall(currentCell, 1, stoneBrick);
                            CreateSouthWall(currentCell, 1, stoneBrick);
                            CreateWestWall(currentCell, 1, stoneBrick);
                            
                            CreateSouthTorch(currentCell, 0);
                        } else if (eastNeighbourType != MazeCellType.WALL) {
                            CreateFloor(currentCell, carpetE);

                            CreateNorthWall(currentCell, 0, stoneBrick);
                            CreateSouthWall(currentCell, 0, stoneBrick);
                            CreateWestWall(currentCell, 0, stoneBrick);

                            CreateNorthWall(currentCell, 1, stoneBrick);
                            CreateSouthWall(currentCell, 1, stoneBrick);
                            CreateWestWall(currentCell, 1, stoneBrick);
                            
                            CreateWestTorch(currentCell, 0);
                        } else if (southNeighbourType != MazeCellType.WALL) {
                            CreateFloor(currentCell, carpetS);

                            CreateNorthWall(currentCell, 0, stoneBrick);
                            CreateEastWall(currentCell, 0, stoneBrick);
                            CreateWestWall(currentCell, 0, stoneBrick);

                            CreateNorthWall(currentCell, 1, stoneBrick);
                            CreateEastWall(currentCell, 1, stoneBrick);
                            CreateWestWall(currentCell, 1, stoneBrick);
                            
                            CreateNorthTorch(currentCell, 0);
                        } else if (westNeighbourType != MazeCellType.WALL) {
                            CreateFloor(currentCell, carpetW);

                            CreateNorthWall(currentCell, 0, stoneBrick);
                            CreateEastWall(currentCell, 0, stoneBrick);
                            CreateSouthWall(currentCell, 0, stoneBrick);

                            CreateNorthWall(currentCell, 1, stoneBrick);
                            CreateEastWall(currentCell, 1, stoneBrick);
                            CreateSouthWall(currentCell, 1, stoneBrick);
                            
                            CreateEastTorch(currentCell, 0);
                        }

                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.white, false);

                        deadEnds.Add(grid.GetCell(x, y));

                        break;
                    case MazeCellType.ROOM:
                        CreateFloor(currentCell, stoneBrick);
                        CreateCeiling(currentCell, 1);

                        switch (grid.GetNorthNeighbour(x, y).GetCellType()) {
                            case MazeCellType.WALL:
                                CreateNorthWall(currentCell, 0, stoneBrick);
                                CreateNorthWall(currentCell, 1, stoneBrick);
                                
                                random = Random.Range(0, 100);
                                if (random >=  0 && random < 10) CreateNorthTorch(currentCell, 0);
                                break;
                            case MazeCellType.DOOR:
                                CreateNorthWall(currentCell, 1, stoneBrick);
                                break;
                            default:
                                break;
                        }

                        switch (grid.GetEastNeighbour(x, y).GetCellType()) {
                            case MazeCellType.WALL:
                                CreateEastWall(currentCell, 0, stoneBrick);
                                CreateEastWall(currentCell, 1, stoneBrick);
                                
                                random = Random.Range(0, 100);
                                if (random >=  0 && random < 10) CreateEastTorch(currentCell, 0);
                                break;
                            case MazeCellType.DOOR:
                                CreateEastWall(currentCell, 1, stoneBrick);
                                break;
                            default:
                                break;
                        }

                        switch (grid.GetSouthNeighbour(x, y).GetCellType()) {
                            case MazeCellType.WALL:
                                CreateSouthWall(currentCell, 0, stoneBrick);
                                CreateSouthWall(currentCell, 1, stoneBrick);
                                
                                random = Random.Range(0, 100);
                                if (random >=  0 && random < 10) CreateSouthTorch(currentCell, 0);
                                break;
                            case MazeCellType.DOOR:
                                CreateSouthWall(currentCell, 1, stoneBrick);
                                break;
                            default:
                                break;
                        }

                        switch (grid.GetWestNeighbour(x, y).GetCellType()) {
                            case MazeCellType.WALL:
                                CreateWestWall(currentCell, 0, stoneBrick);
                                CreateWestWall(currentCell, 1, stoneBrick);
                                
                                random = Random.Range(0, 100);
                                if (random >=  0 && random < 10) CreateWestTorch(currentCell, 0);
                                break;
                            case MazeCellType.DOOR:
                                CreateWestWall(currentCell, 1, stoneBrick);
                                break;
                            default:
                                break;
                        }

                        random = Random.Range(0, 100);
                        if (random >=  0 && random < 10) CreateTurret(x, y, 0);
                        if (random >= 10 && random < 30) CreateBarrel(x, y, 0);

                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.gray, false);

                        break;
                    case MazeCellType.HIDDEN_ROOM:
                        CreateFloor(currentCell, stoneBrick);
                        CreateCeiling(currentCell, 1);

                        switch (grid.GetNorthNeighbour(x, y).GetCellType()) {
                            case MazeCellType.WALL:
                                CreateNorthWall(currentCell, 0, stoneBrick);
                                CreateNorthWall(currentCell, 1, stoneBrick);
                                
                                random = Random.Range(0, 100);
                                if (random >=  0 && random < 10) CreateNorthTorch(currentCell, 0);
                                break;
                            case MazeCellType.HIDDEN_DOOR:
                                CreateNorthWall(currentCell, 1, stoneBrick);
                                break;
                            default:
                                break;
                        }

                        switch (grid.GetEastNeighbour(x, y).GetCellType()) {
                            case MazeCellType.WALL:
                                CreateEastWall(currentCell, 0, stoneBrick);
                                CreateEastWall(currentCell, 1, stoneBrick);
                                
                                random = Random.Range(0, 100);
                                if (random >=  0 && random < 10) CreateEastTorch(currentCell, 0);
                                break;
                            case MazeCellType.HIDDEN_DOOR:
                                CreateEastWall(currentCell, 1, stoneBrick);
                                break;
                            default:
                                break;
                        }

                        switch (grid.GetSouthNeighbour(x, y).GetCellType()) {
                            case MazeCellType.WALL:
                                CreateSouthWall(currentCell, 0, stoneBrick);
                                CreateSouthWall(currentCell, 1, stoneBrick);
                                
                                random = Random.Range(0, 100);
                                if (random >=  0 && random < 10) CreateSouthTorch(currentCell, 0);
                                break;
                            case MazeCellType.HIDDEN_DOOR:
                                CreateSouthWall(currentCell, 1, stoneBrick);
                                break;
                            default:
                                break;
                        }

                        switch (grid.GetWestNeighbour(x, y).GetCellType()) {
                            case MazeCellType.WALL:
                                CreateWestWall(currentCell, 0, stoneBrick);
                                CreateWestWall(currentCell, 1, stoneBrick);
                                
                                random = Random.Range(0, 100);
                                if (random >=  0 && random < 10) CreateWestTorch(currentCell, 0);
                                break;
                            case MazeCellType.HIDDEN_DOOR:
                                CreateWestWall(currentCell, 1, stoneBrick);
                                break;
                            default:
                                break;
                        }

                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.black, false);

                        break;
                    case MazeCellType.DOOR:
                        CreateFloor(currentCell, stoneBrick);
                        CreateCeiling(currentCell, 0);

                        if (grid.GetNorthNeighbour(x, y).GetCellType() == MazeCellType.ROOM) {
                            CreateSouthDoor(currentCell);

                            CreateEastWall(currentCell, 0, stoneBrick);
                            CreateWestWall(currentCell, 0, stoneBrick);
                        }
                        if (grid.GetEastNeighbour(x, y).GetCellType() == MazeCellType.ROOM){
                            CreateWestDoor(currentCell);

                            CreateNorthWall(currentCell, 0, stoneBrick);
                            CreateSouthWall(currentCell, 0, stoneBrick);
                        }
                        if (grid.GetSouthNeighbour(x, y).GetCellType() == MazeCellType.ROOM) {
                            CreateNorthDoor(currentCell);

                            CreateEastWall(currentCell, 0, stoneBrick);
                            CreateWestWall(currentCell, 0, stoneBrick);
                        }
                        if (grid.GetWestNeighbour(x, y).GetCellType() == MazeCellType.ROOM) {
                            CreateWestDoor(currentCell);

                            CreateNorthWall(currentCell, 0, stoneBrick);
                            CreateSouthWall(currentCell, 0, stoneBrick);
                        }

                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.gray, false);

                        break;
                    case MazeCellType.HIDDEN_DOOR:
                        HiddenDoorMazeCell newCell = new HiddenDoorMazeCell(currentCell.GetX(), currentCell.GetY());

                        CreateCeiling(newCell, 0);

                        if (grid.GetNorthNeighbour(x, y).GetCellType() == MazeCellType.HIDDEN_ROOM) {
                            CreateSouthHiddenDoor(newCell);

                            CreateEastWall(newCell, 0, stoneBrick);
                            CreateWestWall(newCell, 0, stoneBrick);
                        }
                        if (grid.GetEastNeighbour(x, y).GetCellType() == MazeCellType.HIDDEN_ROOM){
                            CreateWestHiddenDoor(newCell);

                            CreateNorthWall(newCell, 0, stoneBrick);
                            CreateSouthWall(newCell, 0, stoneBrick);
                        }
                        if (grid.GetSouthNeighbour(x, y).GetCellType() == MazeCellType.HIDDEN_ROOM) {
                            CreateNorthHiddenDoor(newCell);

                            CreateEastWall(newCell, 0, stoneBrick);
                            CreateWestWall(newCell, 0, stoneBrick);
                        }
                        if (grid.GetWestNeighbour(x, y).GetCellType() == MazeCellType.HIDDEN_ROOM) {
                            CreateWestHiddenDoor(newCell);

                            CreateNorthWall(newCell, 0, stoneBrick);
                            CreateSouthWall(newCell, 0, stoneBrick);
                        }

                        grid.SetCell(newCell);

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

    private void CreateFloor(MazeCell mazeCell, Texture2D texture) {
        GameObject floor = CreateMazePiece(floorPrefab, new Vector3(mazeCell.GetX(), 0, mazeCell.GetY()) + floorPrefab.transform.position, texture);
        floor.transform.parent = floorParent.transform;
        mazeCell.AddToMazePieces(floor);
    }

    private void CreateCeiling(MazeCell mazeCell, int level) {
        GameObject ceiling = CreateMazePiece(ceilingPrefab, new Vector3(mazeCell.GetX(), level, mazeCell.GetY()) + ceilingPrefab.transform.position, stoneBrick);
        ceiling.transform.parent = ceilingParent.transform;
        ceiling.name = "Ceiling-L" + level;
        mazeCell.AddToMazePieces(ceiling);
    }

    private void CreateNorthWall(MazeCell mazeCell, int level, Texture2D texture) {
        GameObject wall = CreateWall(mazeCell, new Vector3(mazeCell.GetX(), level, mazeCell.GetY()) + new Vector3(0, 0.5f, -0.5f), 180f, texture);
        wall.name = "Wall-L" + level;
    }

    private void CreateEastWall(MazeCell mazeCell, int level, Texture2D texture) {
        GameObject wall = CreateWall(mazeCell, new Vector3(mazeCell.GetX(), level, mazeCell.GetY()) + new Vector3(0.5f, 0.5f, 0), 90f, texture);
        wall.name = "Wall-L" + level;
    }

    private void CreateSouthWall(MazeCell mazeCell, int level, Texture2D texture) {
        GameObject wall = CreateWall(mazeCell, new Vector3(mazeCell.GetX(), level, mazeCell.GetY()) + new Vector3(0, 0.5f, 0.5f), 0f, texture);
        wall.name = "Wall-L" + level;
    }

    private void CreateWestWall(MazeCell mazeCell, int level, Texture2D texture) {
        GameObject wall = CreateWall(mazeCell, new Vector3(mazeCell.GetX(), level, mazeCell.GetY()) + new Vector3(-0.5f, 0.5f, 0), 270f, texture);
        wall.name = "Wall-L" + level;
    }

    private GameObject CreateWall(MazeCell mazeCell, Vector3 position, float rotation, Texture2D texture) {
        GameObject wall = CreateMazePiece(wallPrefab, position, texture);
        wall.transform.Rotate(0, rotation, 0, Space.Self);
        wall.transform.parent = wallParent.transform;
        mazeCell.AddToMazePieces(wall);
        return wall;
    }

    private void CreateNorthDoor(MazeCell mazeCell) {
        CreateDoor(mazeCell, 0);
    }

    private void CreateEastDoor(MazeCell mazeCell) {
        CreateDoor(mazeCell, 270);
    }

    private void CreateSouthDoor(MazeCell mazeCell) {
        CreateDoor(mazeCell, 180);
    }

    private void CreateWestDoor(MazeCell mazeCell) {
        CreateDoor(mazeCell, 90);
    }

    private void CreateDoor(MazeCell mazeCell, float rotation) {
        GameObject door = CreateObject(doorPrefab, new Vector3(mazeCell.GetX(), 0, mazeCell.GetY()) + doorPrefab.transform.position);
        door.transform.Rotate(0, rotation, 0, Space.Self);
        door.transform.parent = doorParent.transform;
        mazeCell.AddToMazePieces(door.transform.Find("Door 1/Face 1").gameObject);
        mazeCell.AddToMazePieces(door.transform.Find("Door 1/Face 2").gameObject);
        mazeCell.AddToMazePieces(door.transform.Find("Door 1/Face 3").gameObject);
        mazeCell.AddToMazePieces(door.transform.Find("Door 1/Face 4").gameObject);
        mazeCell.AddToMazePieces(door.transform.Find("Door 2/Face 1").gameObject);
        mazeCell.AddToMazePieces(door.transform.Find("Door 2/Face 2").gameObject);
        mazeCell.AddToMazePieces(door.transform.Find("Door 2/Face 3").gameObject);
        mazeCell.AddToMazePieces(door.transform.Find("Door 2/Face 4").gameObject);
    }

    private void CreateNorthHiddenDoor(HiddenDoorMazeCell mazeCell) {
        GameObject hiddenDoor = CreateHiddenDoor(mazeCell, 0);
        grid.GetNorthNeighbour(mazeCell).AddToMazePieces(hiddenDoor.transform.Find("Face 1").gameObject);
        grid.GetSouthNeighbour(mazeCell).AddToMazePieces(hiddenDoor.transform.Find("Face 2").gameObject);
        mazeCell.SetHiddenDoor(hiddenDoor);
    }
    
    private void CreateEastHiddenDoor(HiddenDoorMazeCell mazeCell) {
        GameObject hiddenDoor = CreateHiddenDoor(mazeCell, 90);
        hiddenDoor.transform.Find("Top").Rotate(0, 0, 90);
        grid.GetEastNeighbour(mazeCell).AddToMazePieces(hiddenDoor.transform.Find("Face 1").gameObject);
        grid.GetWestNeighbour(mazeCell).AddToMazePieces(hiddenDoor.transform.Find("Face 2").gameObject);
        mazeCell.SetHiddenDoor(hiddenDoor);
    }

    private void CreateSouthHiddenDoor(HiddenDoorMazeCell mazeCell) {
        GameObject hiddenDoor = CreateHiddenDoor(mazeCell, 0);
        grid.GetNorthNeighbour(mazeCell).AddToMazePieces(hiddenDoor.transform.Find("Face 1").gameObject);
        grid.GetSouthNeighbour(mazeCell).AddToMazePieces(hiddenDoor.transform.Find("Face 2").gameObject);
        mazeCell.SetHiddenDoor(hiddenDoor);
    }

    private void CreateWestHiddenDoor(HiddenDoorMazeCell mazeCell) {
        GameObject hiddenDoor = CreateHiddenDoor(mazeCell, 90);
        hiddenDoor.transform.Find("Top").Rotate(0, 0, 90);
        grid.GetEastNeighbour(mazeCell).AddToMazePieces(hiddenDoor.transform.Find("Face 2").gameObject);
        grid.GetWestNeighbour(mazeCell).AddToMazePieces(hiddenDoor.transform.Find("Face 1").gameObject);
        mazeCell.SetHiddenDoor(hiddenDoor);
    }

    private GameObject CreateHiddenDoor(MazeCell mazeCell, float rotation) {
        GameObject hiddenDoor = CreateObject(hiddenDoorPrefab, new Vector3(mazeCell.GetX(), 0, mazeCell.GetY()) + hiddenDoorPrefab.transform.position);
        hiddenDoor.transform.Rotate(0, rotation, 0, Space.Self);
        hiddenDoor.transform.parent = doorParent.transform;
        mazeCell.AddToMazePieces(hiddenDoor.transform.Find("Top").gameObject);
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

    private void CreateNorthTorch(MazeCell mazeCell, int level) {
        CreateTorch(mazeCell, new Vector3(mazeCell.GetX(), level, mazeCell.GetY()) + new Vector3(0, 0, -0.4f), 0f);
    }

    private void CreateEastTorch(MazeCell mazeCell, int level) {
        CreateTorch(mazeCell, new Vector3(mazeCell.GetX(), level, mazeCell.GetY()) + new Vector3(0.4f, 0, 0), 270f);
    }

    private void CreateSouthTorch(MazeCell mazeCell, int level) {
        CreateTorch(mazeCell, new Vector3(mazeCell.GetX(), level, mazeCell.GetY()) + new Vector3(0, 0, 0.4f), 180f);
    }

    private void CreateWestTorch(MazeCell mazeCell, int level) {
        CreateTorch(mazeCell, new Vector3(mazeCell.GetX(), level, mazeCell.GetY()) + new Vector3(-0.4f, 0, 0), 90f);
    }

    private void CreateTorch(MazeCell mazeCell, Vector3 position, float rotation) {
        GameObject torch = CreateObject(torchPrefab, position + new Vector3(0, 0.5f, 0));
        torch.transform.Rotate(0, rotation, 0, Space.Self);
        torch.transform.parent = propParent.transform;
        lightSources.Add(torch, LightingType.TORCH);
    }

    private void CreateChandelier(MazeCell mazeCell, int level) {
        GameObject chandelier = CreateObject(chandelierPrefab, new Vector3(mazeCell.GetX(), level, mazeCell.GetY()) + new Vector3(0, 0.75f, 0));
        chandelier.transform.parent = propParent.transform;
        lightSources.Add(chandelier, LightingType.TORCH);
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

    public MazeCell GetMazeCell(float x, float y) {
        return grid.GetCell(x, y);
    }
}
