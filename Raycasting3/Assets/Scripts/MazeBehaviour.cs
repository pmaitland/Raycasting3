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

    public List<Material> materials;

    public GameObject floorPrefab;
    public GameObject ceilingPrefab;
    public GameObject wallPrefab;
    public GameObject doorPrefab;

    public GameObject barrelPrefab;
    public GameObject torchPrefab;

    public GameObject turretPrefab;

    private GameObject floorParent;
    private GameObject ceilingParent;
    private GameObject wallParent;
    private GameObject doorParent;
    private GameObject propParent;
    private GameObject enemyParent;

    private GameBehaviour gameController;

    private MazeCell playerStart;

    public enum MazeCellType { Empty, Passage, DeadEnd, Room, DisconnectedDoor, Door };

    private class MazeGrid
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
            for (int i = 1; i < size - 1; i++) {
                for (int j = 1; j < size - 1; j++) {
                    if (grid[j][i].GetCellType() == MazeCellType.Passage) {
                        if (GetNeighboursOfType(i, j, MazeCellType.Empty).Count == 3 && GetNeighboursOfType(i, j, MazeCellType.Passage).Count == 1) {
                            grid[j][i].SetCellType(MazeCellType.DeadEnd);
                        }
                    } else if (grid[j][i].GetCellType() == MazeCellType.DisconnectedDoor) {
                        grid[j][i].SetCellType(MazeCellType.Room);
                    }
                }
            }
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
        for (int i = 0; i < roomCount; i++) {
            int roomWidth  = roomDimensions[Random.Range(0, roomDimensions.Length - 1)];
            int roomHeight = roomDimensions[Random.Range(0, roomDimensions.Length - 1)];

            int roomX = Random.Range(3, (int)(size - 1 - roomWidth) / 2) * 2 - 3;
            int roomY = Random.Range(3, (int)(size - 1 - roomHeight) / 2) * 2 - 3;

            int doorCount = Random.Range(1, 5);
            List<MazeCell> doorOptions = new List<MazeCell>();

            for (int x = roomX; x < roomX + roomWidth; x++) {
                for (int y = roomY; y < roomY + roomHeight; y++) {
                    grid.SetCellType(x, y, MazeCellType.Room);

                    if (x % 2 == 1 && y % 2 == 1) {
                        if ((x == roomX || x == roomX + roomWidth - 1) || (y == roomY || y == roomY + roomHeight - 1)) {
                            doorOptions.Add(grid.GetCell(x, y));
                        }
                    }
                }
            }

            for (int j = 0; j < doorCount; j++) {
                MazeCell door = doorOptions[Random.Range(0, doorOptions.Count)];
                grid.SetCellType(door.GetX(), door.GetY(), MazeCellType.DisconnectedDoor);
                doorOptions.Remove(door);
            }
        }

        MazeDepthFirstSearch(grid, 1, 1);

        grid.CleanUp();

        string path = "Assets/maze.txt";
        StreamWriter writer = new StreamWriter(path, false);

        List<MazeCell> deadEnds = new List<MazeCell>();

        int random = 0;
        MazeCell neighbour;
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                switch (grid.GetCellType(x, y)) {
                    case MazeCellType.Empty:
                        neighbour = grid.GetNorthNeighbour(x, y);
                        if (neighbour != null && neighbour.GetCellType() != MazeCellType.Empty) CreateNorthWall(x, y, 0);
                        if (neighbour != null && neighbour.GetCellType() == MazeCellType.Room) CreateNorthWall(x, y, 1);

                        neighbour = grid.GetEastNeighbour(x, y);
                        if (neighbour != null && neighbour.GetCellType() != MazeCellType.Empty) CreateEastWall(x, y, 0);
                        if (neighbour != null && neighbour.GetCellType() == MazeCellType.Room) CreateEastWall(x, y, 1);

                        neighbour = grid.GetSouthNeighbour(x, y);
                        if (neighbour != null && neighbour.GetCellType() != MazeCellType.Empty) CreateSouthWall(x, y, 0);
                        if (neighbour != null && neighbour.GetCellType() == MazeCellType.Room) CreateSouthWall(x, y, 1);

                        neighbour = grid.GetWestNeighbour(x, y);
                        if (neighbour != null && neighbour.GetCellType() != MazeCellType.Empty) CreateWestWall(x, y, 0);
                        if (neighbour != null && neighbour.GetCellType() == MazeCellType.Room) CreateWestWall(x, y, 1);

                        break;
                    case MazeCellType.Passage:
                        CreateFloor(x, y);
                        CreateCeiling(x, y, 0);

                        neighbour = grid.GetNorthNeighbour(x, y);
                        if (neighbour != null && neighbour.GetCellType() == MazeCellType.Room) CreateNorthWall(x, y, 1);

                        neighbour = grid.GetEastNeighbour(x, y);
                        if (neighbour != null && neighbour.GetCellType() == MazeCellType.Room) CreateEastWall(x, y, 1);

                        neighbour = grid.GetSouthNeighbour(x, y);
                        if (neighbour != null && neighbour.GetCellType() == MazeCellType.Room) CreateSouthWall(x, y, 1);

                        neighbour = grid.GetWestNeighbour(x, y);
                        if (neighbour != null && neighbour.GetCellType() == MazeCellType.Room) CreateWestWall(x, y, 1);

                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.white, false);

                        break;
                    case MazeCellType.DeadEnd:
                        CreateFloor(x, y);
                        CreateCeiling(x, y, 0);
                        
                        if (grid.GetNorthNeighbour(x, y).GetCellType() == MazeCellType.Room) CreateNorthWall(x, y, 0);
                        if (grid.GetEastNeighbour(x, y).GetCellType() == MazeCellType.Room) CreateEastWall(x, y, 0);
                        if (grid.GetSouthNeighbour(x, y).GetCellType() == MazeCellType.Room) CreateSouthWall(x, y, 0);
                        if (grid.GetWestNeighbour(x, y).GetCellType() == MazeCellType.Room) CreateWestWall(x, y, 0);

                        if (grid.GetNorthNeighbour(x, y).GetCellType() != MazeCellType.Empty) CreateSouthTorch(x, y, 0);
                        else if (grid.GetEastNeighbour(x, y).GetCellType() != MazeCellType.Empty) CreateWestTorch(x, y, 0);
                        else if (grid.GetSouthNeighbour(x, y).GetCellType() != MazeCellType.Empty) CreateNorthTorch(x, y, 0);
                        else if (grid.GetWestNeighbour(x, y).GetCellType() != MazeCellType.Empty) CreateEastTorch(x, y, 0);

                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.white, false);

                        deadEnds.Add(grid.GetCell(x, y));

                        break;
                    case MazeCellType.Room:
                        CreateFloor(x, y);
                        CreateCeiling(x, y, 1);

                        random = Random.Range(0, 100);
                        if (random >=  0 && random < 10) CreateTurret(x, y, 0);
                        if (random >= 10 && random < 30) CreateBarrel(x, y, 0);

                        random = Random.Range(0, 100);
                        if (random >=  0 && random < 10) {
                            if (grid.GetNorthNeighbour(x, y) != null && grid.GetNorthNeighbour(x, y).GetCellType() == MazeCellType.Empty) CreateNorthTorch(x, y, 0);
                            else if (grid.GetEastNeighbour(x, y) != null && grid.GetEastNeighbour(x, y).GetCellType() == MazeCellType.Empty) CreateEastTorch(x, y, 0);
                            else if (grid.GetSouthNeighbour(x, y) != null && grid.GetSouthNeighbour(x, y).GetCellType() == MazeCellType.Empty) CreateSouthTorch(x, y, 0);
                            else if (grid.GetWestNeighbour(x, y) != null && grid.GetWestNeighbour(x, y).GetCellType() == MazeCellType.Empty) CreateWestTorch(x, y, 0);
                        }

                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.gray, false);

                        break;
                    case MazeCellType.Door:
                        CreateFloor(x, y);
                        CreateCeiling(x, y, 0);

                        if (grid.GetNorthNeighbour(x, y) != null && grid.GetNorthNeighbour(x, y).GetCellType() == MazeCellType.Empty) CreateDoor(x, y, 90);
                        else CreateDoor(x, y, 0);

                        neighbour = grid.GetNorthNeighbour(x, y);
                        if (neighbour != null && neighbour.GetCellType() == MazeCellType.Room) CreateNorthWall(x, y, 1);

                        neighbour = grid.GetEastNeighbour(x, y);
                        if (neighbour != null && neighbour.GetCellType() == MazeCellType.Room) CreateEastWall(x, y, 1);

                        neighbour = grid.GetSouthNeighbour(x, y);
                        if (neighbour != null && neighbour.GetCellType() == MazeCellType.Room) CreateSouthWall(x, y, 1);

                        neighbour = grid.GetWestNeighbour(x, y);
                        if (neighbour != null && neighbour.GetCellType() == MazeCellType.Room) CreateWestWall(x, y, 1);

                        gameController.CreateMinimapCell(x, y, x + "," + y, Color.gray, false);

                        break;
                    default:
                        break;
                }
                writer.Write((int) grid.GetCellType(y, x) + " ");
            }
            writer.Write("\n");
        }
        writer.Close();

        playerStart = deadEnds[Random.Range(0, deadEnds.Count)];
        gameController.SetPlayerPosition(new Vector3(playerStart.GetX(), 1, playerStart.GetY()));
    }

    private void MazeDepthFirstSearch(MazeGrid grid, int x, int y)
    {
        grid.SetCellType(x, y, MazeCellType.Passage);

        List<MazeCell> availableNeighbours = grid.GetSecondNeighboursOfType(x, y, MazeCellType.Empty, MazeCellType.DisconnectedDoor);

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
            }
            availableNeighbours.Remove(chosenNeighbour);
        }
    }

    private void CreateFloor(int x, int y)
    {
        GameObject floor = CreateMazePiece(floorPrefab, new Vector3(x, 0, y) + floorPrefab.transform.position);
        floor.transform.parent = floorParent.transform;
    }

    private void CreateCeiling(int x, int y, int level)
    {
        GameObject ceiling = CreateMazePiece(ceilingPrefab, new Vector3(x, level, y) + ceilingPrefab.transform.position);
        ceiling.transform.parent = ceilingParent.transform;
    }

    private void CreateWalls(int x, int y, int level)
    {
        CreateNorthWall(x, y, level);
        CreateEastWall(x, y, level);
        CreateSouthWall(x, y, level);
        CreateWestWall(x, y, level);
    }

    private void CreateNorthWall(int x, int y, int level)
    {
        CreateWall(new Vector3(x, level, y) + new Vector3(0, 0.5f, -0.5f), 0f);
    }

    private void CreateEastWall(int x, int y, int level)
    {
        CreateWall(new Vector3(x, level, y) + new Vector3(0.5f, 0.5f, 0), 270f);
    }

    private void CreateSouthWall(int x, int y, int level)
    {
        CreateWall(new Vector3(x, level, y) + new Vector3(0, 0.5f, 0.5f), 180f);
    }

    private void CreateWestWall(int x, int y, int level)
    {
        CreateWall(new Vector3(x, level, y) + new Vector3(-0.5f, 0.5f, 0), 90f);
    }

    private void CreateWall(Vector3 position, float rotation)
    {
        GameObject wall = CreateMazePiece(wallPrefab, position);
        wall.transform.Rotate(0, rotation, 0, Space.Self);
        wall.transform.parent = wallParent.transform;
    }

    private void CreateDoor(int x, int y, float rotation)
    {
        GameObject door = CreateMazePiece(doorPrefab, new Vector3(x, 0, y) + doorPrefab.transform.position);
        door.transform.Rotate(0, rotation, 0, Space.Self);
        door.transform.parent = doorParent.transform;
    }

    private GameObject CreateMazePiece(GameObject prefab, Vector3 position)
    {
        GameObject mazePiece = CreateObject(prefab, position);
        Material material = materials[Random.Range(0, materials.Count)];

        MeshRenderer meshRenderer = mazePiece.GetComponent<MeshRenderer>();
        if (meshRenderer != null) meshRenderer.material = material;

        MeshRenderer[] childrenMeshRenderers = mazePiece.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRen in childrenMeshRenderers) meshRen.material = material;

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
    }

    private void CreateTurret(int x, int y, int level)
    {
        GameObject turret = CreateObject(turretPrefab, new Vector3(x, level, y) + new Vector3(turretPrefab.transform.position.x, turretPrefab.transform.position.y, 0));
        turret.transform.parent = enemyParent.transform;
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
