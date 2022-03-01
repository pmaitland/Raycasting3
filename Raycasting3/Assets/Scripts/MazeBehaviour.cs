using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MazeBehaviour : MonoBehaviour
{
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

    public enum MazeCellType { Empty, Passage, DeadEnd, Room, HiddenRoom, DisconnectedDoor, Door, DisconnectedHiddenDoor, HiddenDoor };

    public class MazeGrid
    {
        private int size;
        private List<List<MazeCell>> grid;

        public MazeGrid(int size)
        {
            this.size = size;
            grid = new List<List<MazeCell>>();
            for (int i = 0; i < size; i++) {
                grid.Add(new List<MazeCell>());
                for (int j = 0; j < size; j++) {
                    grid[i].Add(new MazeCell(j, i, MazeCellType.Empty));
                }
            }
        }

        public MazeCell GetCell(int x, int y)
        {
            return grid[y][x];
        }

        public MazeCellType GetCellType(int x, int y)
        {
            return grid[y][x].GetCellType();
        }

        public MazeCell GetNorthNeighbour(int x, int y)
        {
            return y > 0 ? grid[y - 1][x] : null;
        }

        public MazeCell GetSouthNeighbour(int x, int y)
        {
            return y < size - 1 ? grid[y + 1][x] : null;
        }

        public MazeCell GetEastNeighbour(int x, int y)
        {
            return x < size - 1 ? grid[y][x + 1] : null;
        }

        public MazeCell GetWestNeighbour(int x, int y)
        {
            return x > 0 ? grid[y][x - 1] : null;
        }

        public List<MazeCell> GetNeighboursOfType(MazeCell cell, params MazeCellType[] types)
        {
            return GetNeighboursOfType(cell.GetX(), cell.GetY(), types);
        }

        public List<MazeCell> GetNeighboursOfType(int x, int y, params MazeCellType[] types)
        {
            List<MazeCell> neighbours = new List<MazeCell>();
            MazeCell neighbour = GetNorthNeighbour(x, y);
            if (neighbour != null && types.Contains(neighbour.GetCellType())) neighbours.Add(neighbour);
            neighbour = GetEastNeighbour(x, y);
            if (neighbour != null && types.Contains(neighbour.GetCellType())) neighbours.Add(neighbour);
            neighbour = GetSouthNeighbour(x, y);
            if (neighbour != null && types.Contains(neighbour.GetCellType())) neighbours.Add(neighbour);
            neighbour = GetWestNeighbour(x, y);
            if (neighbour != null && types.Contains(neighbour.GetCellType())) neighbours.Add(neighbour);
            return neighbours;
        }

        public List<MazeCell> GetNeighboursNotOfType(MazeCell cell, params MazeCellType[] types)
        {
            return GetNeighboursNotOfType(cell.GetX(), cell.GetY(), types);
        }

        public List<MazeCell> GetNeighboursNotOfType(int x, int y, params MazeCellType[] types)
        {
            List<MazeCell> neighbours = new List<MazeCell>();
            MazeCell neighbour = GetNorthNeighbour(x, y);
            if (neighbour != null && !types.Contains(neighbour.GetCellType())) neighbours.Add(neighbour);
            neighbour = GetEastNeighbour(x, y);
            if (neighbour != null && !types.Contains(neighbour.GetCellType())) neighbours.Add(neighbour);
            neighbour = GetSouthNeighbour(x, y);
            if (neighbour != null && !types.Contains(neighbour.GetCellType())) neighbours.Add(neighbour);
            neighbour = GetWestNeighbour(x, y);
            if (neighbour != null && !types.Contains(neighbour.GetCellType())) neighbours.Add(neighbour);
            return neighbours;
        }

        public MazeCell GetSecondNorthNeighbour(int x, int y)
        {
            return y > 1 ? grid[y - 2][x] : null;
        }

        public MazeCell GetSecondSouthNeighbour(int x, int y)
        {
            return y < size - 2 ? grid[y + 2][x] : null;
        }

        public MazeCell GetSecondEastNeighbour(int x, int y)
        {
            return x < size - 2 ? grid[y][x + 2] : null;
        }

        public MazeCell GetSecondWestNeighbour(int x, int y)
        {
            return x > 1 ? grid[y][x - 2] : null;
        }

        public List<MazeCell> GetSecondNeighboursOfType(int x, int y, params MazeCellType[] types)
        {
            List<MazeCell> neighbours = new List<MazeCell>();
            MazeCell neighbour = GetSecondNorthNeighbour(x, y);
            if (neighbour != null && types.Contains(neighbour.GetCellType())) neighbours.Add(neighbour);
            neighbour = GetSecondEastNeighbour(x, y);
            if (neighbour != null && types.Contains(neighbour.GetCellType())) neighbours.Add(neighbour);
            neighbour = GetSecondSouthNeighbour(x, y);
            if (neighbour != null && types.Contains(neighbour.GetCellType())) neighbours.Add(neighbour);
            neighbour = GetSecondWestNeighbour(x, y);
            if (neighbour != null && types.Contains(neighbour.GetCellType())) neighbours.Add(neighbour);
            return neighbours;
        }

        public void SetCellType(int x, int y, MazeCellType newType)
        {
            grid[y][x].SetCellType(newType);
        }

        public void CleanUp()
        {
            MazeCellType cellType;
            for (int i = 1; i < size - 1; i++) {
                for (int j = 1; j < size - 1; j++) {
                    cellType = grid[j][i].GetCellType();
                    if (cellType == MazeCellType.Passage) {
                        if (GetNeighboursOfType(i, j, MazeCellType.Empty).Count == 3 && GetNeighboursOfType(i, j, MazeCellType.Passage).Count == 1) {
                            grid[j][i].SetCellType(MazeCellType.DeadEnd);
                        }
                    } else if (cellType == MazeCellType.DisconnectedDoor) {
                        grid[j][i].SetCellType(MazeCellType.Room);
                    } else if (cellType == MazeCellType.DisconnectedHiddenDoor) {
                        grid[j][i].SetCellType(MazeCellType.HiddenRoom);
                    }
                }
            }
        }

        public Vector3 GetNextCellInPath(Vector3 currentPosition, Vector3 targetPosition)
        {
            MazeCell currentCell = GetCell(Mathf.RoundToInt(currentPosition.x), Mathf.RoundToInt(currentPosition.z));
            MazeCell targetCell = GetCell(Mathf.RoundToInt(targetPosition.x), Mathf.RoundToInt(targetPosition.z));

            if (currentCell == targetCell) return new Vector3(currentCell.GetX(), 0, currentCell.GetY());

            List<MazeCell> suitableNeighbours = GetNeighboursOfType(currentCell, MazeCellType.Passage, MazeCellType.DeadEnd);
            foreach (MazeCell neighbour in suitableNeighbours) {
                if (IsNextCell(currentCell, neighbour, targetCell)) return new Vector3(neighbour.GetX(), 0, neighbour.GetY());
            }

            return new Vector3(currentCell.GetX(), 0, currentCell.GetY());
        }

        private bool IsNextCell(MazeCell previousCell, MazeCell currentCell, MazeCell targetCell)
        {
            List<MazeCell> suitableNeighbours = GetNeighboursOfType(currentCell, MazeCellType.Passage, MazeCellType.DeadEnd);
            foreach (MazeCell neighbour in suitableNeighbours) {
                if (neighbour == targetCell || (neighbour != previousCell && IsNextCell(currentCell, neighbour, targetCell))) return true;
            }
            return false;
        }
    }

    public class MazeCell
    {
        private int x;
        private int y;
        private MazeCellType type;

        public MazeCell(int x, int y, MazeCellType type)
        {
            this.x = x;
            this.y = y;
            this.type = type;
        }

        public int GetX()
        {
            return x;
        }

        public int GetY()
        {
            return y;
        }

        public MazeCellType GetCellType()
        {
            return type;
        }

        public void SetCellType(MazeCellType newType)
        {
            type = newType;
        }
    }

    public class Room
    {
        private int x;
        private int y;
        private int width;
        private int height;

        public Room(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public int GetX()
        {
            return x;
        }

        public int GetY()
        {
            return y;
        }

        public int GetWidth()
        {
            return width;
        }

        public int GetHeight()
        {
            return height;
        }

        public static bool Overlap(Room roomA, Room roomB)
        {
            return roomA.GetX() < roomB.GetX() + roomB.GetWidth()  && roomA.GetX() + roomA.GetWidth()  > roomB.GetX()
                && roomA.GetY() < roomB.GetY() + roomB.GetHeight() && roomA.GetY() + roomA.GetHeight() > roomB.GetY();
        }
    }

    void OnValidate()
    {
        if (size % 2 == 0) size -= 1;
    }

    void Start()
    {
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

    private void GenerateMaze()
    {
        MazeGrid grid = new MazeGrid(size);

        int[] roomDimensions = new int[] { 3, 5, 7 };

        List<Room> existingRooms = new List<Room>();
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

                Room room = new Room(roomX, roomY, roomWidth, roomHeight);

                foreach (Room existingRoom in existingRooms) {
                    if (Room.Overlap(room, existingRoom)) canBeCreated = false;
                }

                if (canBeCreated) {
                    List<MazeCell> doorOptions = new List<MazeCell>();

                    for (int x = roomX; x < roomX + roomWidth; x++) {
                        for (int y = roomY; y < roomY + roomHeight; y++) {
                            if (i == 0) grid.SetCellType(x, y, MazeCellType.HiddenRoom);
                            else grid.SetCellType(x, y, MazeCellType.Room);

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
                        if (i == 0) grid.SetCellType(door.GetX(), door.GetY(), MazeCellType.DisconnectedHiddenDoor);
                        else grid.SetCellType(door.GetX(), door.GetY(), MazeCellType.DisconnectedDoor);
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
                switch (grid.GetCellType(x, y)) {
                    case MazeCellType.Empty:
                        neighbour = grid.GetNorthNeighbour(x, y);
                        if (neighbour != null) {
                            neighbourType = neighbour.GetCellType();
                            if (neighbourType != MazeCellType.Empty) {
                                if (neighbourType == MazeCellType.Passage) {
                                    random = Random.Range(0, 100);
                                    if (random <= 5) CreateNorthWall(x, y, 0, banner);
                                    else CreateNorthWall(x, y, 0, stoneBrick);
                                } else {
                                    CreateNorthWall(x, y, 0, stoneBrick);
                                }
                            }
                            if (neighbourType == MazeCellType.Passage || neighbourType == MazeCellType.DeadEnd
                                || neighbourType == MazeCellType.Room || neighbourType == MazeCellType.HiddenRoom) {
                                CreateNorthWall(x, y, 1, stoneBrick);
                            }
                        }

                        neighbour = grid.GetEastNeighbour(x, y);
                        if (neighbour != null) {
                            neighbourType = neighbour.GetCellType();
                            if (neighbourType != MazeCellType.Empty) {
                                if (neighbourType == MazeCellType.Passage) {
                                    random = Random.Range(0, 100);
                                    if (random <= 5) CreateEastWall(x, y, 0, banner);
                                    else CreateEastWall(x, y, 0, stoneBrick);
                                } else {
                                    CreateEastWall(x, y, 0, stoneBrick);
                                }
                            }
                            if (neighbourType == MazeCellType.Passage || neighbourType == MazeCellType.DeadEnd
                                || neighbourType == MazeCellType.Room || neighbourType == MazeCellType.HiddenRoom) {
                                CreateEastWall(x, y, 1, stoneBrick);
                            }
                        }

                        neighbour = grid.GetSouthNeighbour(x, y);
                        if (neighbour != null) {
                            neighbourType = neighbour.GetCellType();
                            if (neighbourType != MazeCellType.Empty) {
                                if (neighbourType == MazeCellType.Passage) {
                                    random = Random.Range(0, 100);
                                    if (random <= 5) CreateSouthWall(x, y, 0, banner);
                                    else CreateSouthWall(x, y, 0, stoneBrick);
                                } else {
                                    CreateSouthWall(x, y, 0, stoneBrick);
                                }
                            }
                            if (neighbourType == MazeCellType.Passage || neighbourType == MazeCellType.DeadEnd
                                || neighbourType == MazeCellType.Room || neighbourType == MazeCellType.HiddenRoom) {
                                CreateSouthWall(x, y, 1, stoneBrick);
                            }
                        }

                        neighbour = grid.GetWestNeighbour(x, y);
                        if (neighbour != null) {
                            neighbourType = neighbour.GetCellType();
                            if (neighbourType != MazeCellType.Empty) {
                                if (neighbourType == MazeCellType.Passage) {
                                    random = Random.Range(0, 100);
                                    if (random <= 5) CreateWestWall(x, y, 0, banner);
                                    else CreateWestWall(x, y, 0, stoneBrick);
                                } else {
                                    CreateWestWall(x, y, 0, stoneBrick);
                                }
                            }
                            if (neighbourType == MazeCellType.Passage || neighbourType == MazeCellType.DeadEnd
                                || neighbourType == MazeCellType.Room || neighbourType == MazeCellType.HiddenRoom) {
                                CreateWestWall(x, y, 1, stoneBrick);
                            }
                        }

                        break;
                    case MazeCellType.Passage:
                        CreateCeiling(x, y, 1);

                        int passageNeighbourCount = 0;
                        bool hasPassageNorthNeighbour = false;
                        bool hasPassageEastNeighbour = false;
                        bool hasPassageSouthNeighbour = false;
                        bool hasPassageWestNeighbour = false;

                        neighbourType = grid.GetNorthNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.Room) {
                            CreateNorthWall(x, y, 1, stoneBrick);
                        } else if (neighbourType == MazeCellType.Passage || neighbourType == MazeCellType.DeadEnd) {
                            passageNeighbourCount++;
                            hasPassageNorthNeighbour = true;
                        }

                        neighbourType = grid.GetEastNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.Room) {
                            CreateEastWall(x, y, 1, stoneBrick);
                        } else if (neighbourType == MazeCellType.Passage || neighbourType == MazeCellType.DeadEnd) {
                            passageNeighbourCount++;
                            hasPassageEastNeighbour = true;
                        }

                        neighbourType = grid.GetSouthNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.Room) {
                            CreateSouthWall(x, y, 1, stoneBrick);
                        } else if (neighbourType == MazeCellType.Passage || neighbourType == MazeCellType.DeadEnd) {
                            passageNeighbourCount++;
                            hasPassageSouthNeighbour = true;
                        }

                        neighbourType = grid.GetWestNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.Room) {
                            CreateWestWall(x, y, 1, stoneBrick);
                        } else if (neighbourType == MazeCellType.Passage || neighbourType == MazeCellType.DeadEnd) {
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

                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.white, false);

                        break;
                    case MazeCellType.DeadEnd:
                        CreateCeiling(x, y, 1);

                        MazeCellType northNeighbourType = grid.GetNorthNeighbour(x, y).GetCellType();
                        MazeCellType eastNeighbourType  = grid.GetEastNeighbour(x, y).GetCellType();
                        MazeCellType southNeighbourType  = grid.GetSouthNeighbour(x, y).GetCellType();
                        MazeCellType westNeighbourType  = grid.GetWestNeighbour(x, y).GetCellType();

                        if (northNeighbourType != MazeCellType.Empty) {
                            if (northNeighbourType == MazeCellType.Room) CreateNorthWall(x, y, 0, stoneBrick);
                            CreateFloor(x, y, carpetN);
                            CreateSouthTorch(x, y, 0);
                        } else if (eastNeighbourType != MazeCellType.Empty) {
                            if (eastNeighbourType == MazeCellType.Room) CreateEastWall(x, y, 0, stoneBrick);
                            CreateFloor(x, y, carpetE);
                            CreateWestTorch(x, y, 0);
                        } else if (southNeighbourType != MazeCellType.Empty) {
                            if (southNeighbourType == MazeCellType.Room) CreateSouthWall(x, y, 0, stoneBrick);
                            CreateFloor(x, y, carpetS);
                            CreateNorthTorch(x, y, 0);
                        } else if (westNeighbourType != MazeCellType.Empty) {
                            if (westNeighbourType == MazeCellType.Room) CreateWestWall(x, y, 0, stoneBrick);
                            CreateFloor(x, y, carpetW);
                            CreateEastTorch(x, y, 0);
                        }

                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.white, false);

                        deadEnds.Add(grid.GetCell(x, y));

                        break;
                    case MazeCellType.Room:
                        CreateFloor(x, y, stoneBrick);
                        CreateCeiling(x, y, 1);

                        random = Random.Range(0, 100);
                        if (random >=  0 && random < 10) CreateTurret(x, y, 0);
                        if (random >= 10 && random < 30) CreateBarrel(x, y, 0);

                        random = Random.Range(0, 100);
                        if (random >=  0 && random < 10) {
                            if (grid.GetNorthNeighbour(x, y).GetCellType() == MazeCellType.Empty) CreateNorthTorch(x, y, 0);
                            else if (grid.GetEastNeighbour(x, y).GetCellType() == MazeCellType.Empty) CreateEastTorch(x, y, 0);
                            else if (grid.GetSouthNeighbour(x, y).GetCellType() == MazeCellType.Empty) CreateSouthTorch(x, y, 0);
                            else if (grid.GetWestNeighbour(x, y).GetCellType() == MazeCellType.Empty) CreateWestTorch(x, y, 0);
                        }

                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.gray, false);

                        break;
                    case MazeCellType.HiddenRoom:
                        CreateFloor(x, y, stoneBrick);
                        CreateCeiling(x, y, 1);

                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.black, false);

                        break;
                    case MazeCellType.Door:
                        CreateFloor(x, y, stoneBrick);
                        CreateCeiling(x, y, 0);

                        neighbourType = grid.GetNorthNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.Passage || neighbourType == MazeCellType.DeadEnd || neighbourType == MazeCellType.Room) {
                            CreateNorthWall(x, y, 1, stoneBrick);
                            if (neighbourType == MazeCellType.Passage || neighbourType == MazeCellType.DeadEnd) CreateSouthDoor(x, y);
                        }
                        neighbourType = grid.GetEastNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.Passage || neighbourType == MazeCellType.DeadEnd || neighbourType == MazeCellType.Room) {
                            CreateEastWall(x, y, 1, stoneBrick);
                            if (neighbourType == MazeCellType.Passage || neighbourType == MazeCellType.DeadEnd) CreateWestDoor(x, y);
                        }
                        neighbourType = grid.GetSouthNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.Passage || neighbourType == MazeCellType.DeadEnd || neighbourType == MazeCellType.Room) {
                            CreateSouthWall(x, y, 1, stoneBrick);
                            if (neighbourType == MazeCellType.Passage || neighbourType == MazeCellType.DeadEnd) CreateNorthDoor(x, y);
                        }
                        neighbourType = grid.GetWestNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.Passage || neighbourType == MazeCellType.DeadEnd || neighbourType == MazeCellType.Room) {
                            CreateWestWall(x, y, 1, stoneBrick);
                            if (neighbourType == MazeCellType.Passage || neighbourType == MazeCellType.DeadEnd) CreateEastDoor(x, y);
                        }

                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.gray, false);

                        break;
                    case MazeCellType.HiddenDoor:
                        CreateCeiling(x, y, 0);

                        neighbourType = grid.GetNorthNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.Passage || neighbourType == MazeCellType.DeadEnd || neighbourType == MazeCellType.HiddenRoom) {
                            CreateNorthWall(x, y, 1, stoneBrick);
                            if (neighbourType == MazeCellType.Passage || neighbourType == MazeCellType.DeadEnd) CreateSouthHiddenDoor(x, y);
                        }
                        neighbourType = grid.GetEastNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.Passage || neighbourType == MazeCellType.DeadEnd || neighbourType == MazeCellType.HiddenRoom) {
                            CreateEastWall(x, y, 1, stoneBrick);
                            if (neighbourType == MazeCellType.Passage || neighbourType == MazeCellType.DeadEnd) CreateWestHiddenDoor(x, y);
                        }
                        neighbourType = grid.GetSouthNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.Passage || neighbourType == MazeCellType.DeadEnd || neighbourType == MazeCellType.HiddenRoom) {
                            CreateSouthWall(x, y, 1, stoneBrick);
                            if (neighbourType == MazeCellType.Passage || neighbourType == MazeCellType.DeadEnd) CreateNorthHiddenDoor(x, y);
                        }
                        neighbourType = grid.GetWestNeighbour(x, y).GetCellType();
                        if (neighbourType == MazeCellType.Passage || neighbourType == MazeCellType.DeadEnd || neighbourType == MazeCellType.HiddenRoom) {
                            CreateWestWall(x, y, 1, stoneBrick);
                            if (neighbourType == MazeCellType.Passage || neighbourType == MazeCellType.DeadEnd) CreateEastHiddenDoor(x, y);
                        }

                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.black, false);

                        break;
                    default:
                        break;
                }
                if (Application.isEditor) writer.Write((int) grid.GetCellType(y, x) + " ");
            }
            if (Application.isEditor) writer.Write("\n");
        }
        if (Application.isEditor) writer.Close();

        playerStart = deadEnds[Random.Range(0, deadEnds.Count)];
        gameController.SetPlayerPosition(new Vector3(playerStart.GetX(), 1, playerStart.GetY()));
        deadEnds.Remove(playerStart);

        foreach (MazeCell deadEnd in deadEnds) CreateKnight(deadEnd.GetX(), deadEnd.GetY(), 0, grid);
    }

    private void MazeDepthFirstSearch(MazeGrid grid, int x, int y)
    {
        grid.SetCellType(x, y, MazeCellType.Passage);

        List<MazeCell> availableNeighbours = grid.GetSecondNeighboursOfType(x, y, MazeCellType.Empty, MazeCellType.DisconnectedDoor, MazeCellType.DisconnectedHiddenDoor);

        while (availableNeighbours.Count != 0) {
            MazeCell chosenNeighbour = availableNeighbours[Random.Range(0, availableNeighbours.Count)];
            if (chosenNeighbour.GetCellType() == MazeCellType.Empty) {
                if      (chosenNeighbour == grid.GetSecondNorthNeighbour(x, y)) grid.SetCellType(x,     y - 1, MazeCellType.Passage);
                else if (chosenNeighbour == grid.GetSecondEastNeighbour(x, y))  grid.SetCellType(x + 1, y,     MazeCellType.Passage);
                else if (chosenNeighbour == grid.GetSecondSouthNeighbour(x, y)) grid.SetCellType(x,     y + 1, MazeCellType.Passage);
                else if (chosenNeighbour == grid.GetSecondWestNeighbour(x, y))  grid.SetCellType(x - 1, y,     MazeCellType.Passage);
                grid.SetCellType(chosenNeighbour.GetX(), chosenNeighbour.GetY(), MazeCellType.Passage);
                MazeDepthFirstSearch(grid, chosenNeighbour.GetX(), chosenNeighbour.GetY());
            } else if (chosenNeighbour.GetCellType() == MazeCellType.DisconnectedDoor) {
                if      (chosenNeighbour == grid.GetSecondNorthNeighbour(x, y)) grid.SetCellType(x,     y - 1, MazeCellType.Door);
                else if (chosenNeighbour == grid.GetSecondEastNeighbour(x, y))  grid.SetCellType(x + 1, y,     MazeCellType.Door);
                else if (chosenNeighbour == grid.GetSecondSouthNeighbour(x, y)) grid.SetCellType(x,     y + 1, MazeCellType.Door);
                else if (chosenNeighbour == grid.GetSecondWestNeighbour(x, y))  grid.SetCellType(x - 1, y,     MazeCellType.Door);
                grid.SetCellType(chosenNeighbour.GetX(), chosenNeighbour.GetY(), MazeCellType.Room);
            } else if (chosenNeighbour.GetCellType() == MazeCellType.DisconnectedHiddenDoor) {
                if      (chosenNeighbour == grid.GetSecondNorthNeighbour(x, y)) grid.SetCellType(x,     y - 1, MazeCellType.HiddenDoor);
                else if (chosenNeighbour == grid.GetSecondEastNeighbour(x, y))  grid.SetCellType(x + 1, y,     MazeCellType.HiddenDoor);
                else if (chosenNeighbour == grid.GetSecondSouthNeighbour(x, y)) grid.SetCellType(x,     y + 1, MazeCellType.HiddenDoor);
                else if (chosenNeighbour == grid.GetSecondWestNeighbour(x, y))  grid.SetCellType(x - 1, y,     MazeCellType.HiddenDoor);
                grid.SetCellType(chosenNeighbour.GetX(), chosenNeighbour.GetY(), MazeCellType.HiddenRoom);
            }
            availableNeighbours.Remove(chosenNeighbour);
        }
    }

    private void CreateFloor(int x, int y, Texture2D texture)
    {
        GameObject floor = CreateMazePiece(floorPrefab, new Vector3(x, 0, y) + floorPrefab.transform.position, texture);
        floor.transform.parent = floorParent.transform;
    }

    private void CreateCeiling(int x, int y, int level)
    {
        GameObject ceiling = CreateMazePiece(ceilingPrefab, new Vector3(x, level, y) + ceilingPrefab.transform.position, stoneBrick);
        ceiling.transform.parent = ceilingParent.transform;
    }

    private void CreateNorthWall(int x, int y, int level, Texture2D texture)
    {
        CreateWall(new Vector3(x, level, y) + new Vector3(0, 0.5f, -0.5f), 0f, texture);
    }

    private void CreateEastWall(int x, int y, int level, Texture2D texture)
    {
        CreateWall(new Vector3(x, level, y) + new Vector3(0.5f, 0.5f, 0), 270f, texture);
    }

    private void CreateSouthWall(int x, int y, int level, Texture2D texture)
    {
        CreateWall(new Vector3(x, level, y) + new Vector3(0, 0.5f, 0.5f), 180f, texture);
    }

    private void CreateWestWall(int x, int y, int level, Texture2D texture)
    {
        CreateWall(new Vector3(x, level, y) + new Vector3(-0.5f, 0.5f, 0), 90f, texture);
    }

    private void CreateWall(Vector3 position, float rotation, Texture2D texture)
    {
        GameObject wall = CreateMazePiece(wallPrefab, position, texture);
        wall.transform.Rotate(0, rotation, 0, Space.Self);
        wall.transform.parent = wallParent.transform;
    }

    private void CreateNorthDoor(int x, int y)
    {
        CreateDoor(x, y, 0);
    }

    private void CreateEastDoor(int x, int y)
    {
        CreateDoor(x, y, 270);
    }

    private void CreateSouthDoor(int x, int y)
    {
        CreateDoor(x, y, 180);
    }

    private void CreateWestDoor(int x, int y)
    {
        CreateDoor(x, y, 90);
    }

    private void CreateDoor(int x, int y, float rotation)
    {
        GameObject door = CreateObject(doorPrefab, new Vector3(x, 0, y) + doorPrefab.transform.position);
        door.transform.Rotate(0, rotation, 0, Space.Self);
        door.transform.parent = doorParent.transform;
    }

    private void CreateNorthHiddenDoor(int x, int y)
    {
        GameObject hiddenDoor = CreateHiddenDoor(x, y, 0);
    }

    private void CreateEastHiddenDoor(int x, int y)
    {
        GameObject hiddenDoor = CreateHiddenDoor(x, y, 90);
        hiddenDoor.transform.Find("Top").Rotate(0, 0, 90);
    }

    private void CreateSouthHiddenDoor(int x, int y)
    {
        GameObject hiddenDoor = CreateHiddenDoor(x, y, 0);
    }

    private void CreateWestHiddenDoor(int x, int y)
    {
        GameObject hiddenDoor = CreateHiddenDoor(x, y, 90);
        hiddenDoor.transform.Find("Top").Rotate(0, 0, 90);
    }

    private GameObject CreateHiddenDoor(int x, int y, float rotation)
    {
        GameObject hiddenDoor = CreateObject(hiddenDoorPrefab, new Vector3(x, 0, y) + hiddenDoorPrefab.transform.position);
        hiddenDoor.transform.Rotate(0, rotation, 0, Space.Self);
        hiddenDoor.transform.parent = doorParent.transform;
        return hiddenDoor;
    }

    private GameObject CreateMazePiece(GameObject prefab, Vector3 position, Texture2D texture)
    {
        GameObject mazePiece = CreateObject(prefab, position);

        MeshRenderer meshRenderer = mazePiece.GetComponent<MeshRenderer>();
        if (meshRenderer != null) meshRenderer.material.mainTexture = texture;

        return mazePiece;
    }

    private void CreateBarrel(int x, int y, int level)
    {
        GameObject barrel = CreateObject(barrelPrefab, new Vector3(x, level, y) + new Vector3(barrelPrefab.transform.position.x, barrelPrefab.transform.position.y, 0));
        barrel.transform.parent = propParent.transform;
    }

    private void CreateNorthTorch(int x, int y, int level)
    {
        CreateTorch(new Vector3(x, level, y) + new Vector3(0, 0, -0.4f), 0f);
    }

    private void CreateEastTorch(int x, int y, int level)
    {
        CreateTorch(new Vector3(x, level, y) + new Vector3(0.4f, 0, 0), 270f);
    }

    private void CreateSouthTorch(int x, int y, int level)
    {
        CreateTorch(new Vector3(x, level, y) + new Vector3(0, 0, 0.4f), 180f);
    }

    private void CreateWestTorch(int x, int y, int level)
    {
        CreateTorch(new Vector3(x, level, y) + new Vector3(-0.4f, 0, 0), 90f);
    }

    private void CreateTorch(Vector3 position, float rotation)
    {
        GameObject torch = CreateObject(torchPrefab, position + new Vector3(0, 0.5f, 0));
        torch.transform.Rotate(0, rotation, 0, Space.Self);
        torch.transform.parent = propParent.transform;
        if (Application.isEditor) torch.transform.Find("Directional Light").gameObject.SetActive(false);
    }

    private void CreateTurret(int x, int y, int level)
    {
        GameObject turret = CreateObject(turretPrefab, new Vector3(x, level, y) + new Vector3(turretPrefab.transform.position.x, turretPrefab.transform.position.y, 0));
        turret.transform.parent = enemyParent.transform;
    }

    private void CreateKnight(int x, int y, int level, MazeGrid grid)
    {
        GameObject knight = CreateObject(knightPrefab, new Vector3(x, level, y) + new Vector3(knightPrefab.transform.position.x, knightPrefab.transform.position.y, 0));
        knight.transform.parent = enemyParent.transform;
        knight.GetComponentInChildren<KnightBehaviour>().SetPlayer(gameController.GetPlayer());
        knight.GetComponentInChildren<KnightBehaviour>().SetMaze(grid);
    }

    private GameObject CreateObject(GameObject prefab, Vector3 position)
    {
        GameObject obj = Instantiate(prefab, position, prefab.transform.rotation);
        return obj;
    }

    public int GetSize()
    {
        return size;
    }

}
