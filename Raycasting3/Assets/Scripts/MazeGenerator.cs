using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Range(3, 49)]
    public int width = 21;
    [Range(3, 49)]
    public int height = 21;
    [Range(0, 50)]
    public int roomCount = 6;
    public List<Material> materials;
    public GameObject floorPrefab;
    public GameObject ceilingPrefab;
    public GameObject northWallPrefab;
    public GameObject eastWallPrefab;
    public GameObject southWallPrefab;
    public GameObject westWallPrefab;

    private GameObject floorParent;
    private GameObject ceilingParent;
    private GameObject wallParent;

    void OnValidate()
    {
        if (width % 2 == 0) width -= 1;
        if (height % 2 == 0) height -= 1;
    }

    void Start()
    {
        floorParent = new GameObject("Floors");
        floorParent.transform.parent = transform;
        ceilingParent = new GameObject("Ceilings");
        ceilingParent.transform.parent = transform;
        wallParent = new GameObject("Walls");
        wallParent.transform.parent = transform;

        GenerateMaze();
    }

    private void GenerateMaze()
    {
        List<List<string>> grid = new List<List<string>>();

        for (int i = 0; i < width; i++) {
            grid.Add(new List<string>());
            for (int j = 0; j < height; j++) {
                grid[i].Add("w");
            }
        }

        List<int> currentCell = new List<int>();
        currentCell.Add(Random.Range(1, (int) ((width - 1) / 2)) * 2 - 1);
        currentCell.Add(Random.Range(1, (int) ((height - 1) / 2)) * 2 - 1);
        MazeDepthFirstSearch(grid, currentCell);

        int minRoomWidth  = 2; int maxRoomWidth  = 5;
        int minRoomHeight = 2; int maxRoomHeight = 5;
        for (int i = 0; i < roomCount; i++) {
            int roomWidth  = Random.Range(minRoomWidth, maxRoomWidth);
            int roomHeight = Random.Range(minRoomHeight, maxRoomHeight);
            int roomX = Random.Range(1, width  - 1 - roomWidth);
            int roomY = Random.Range(1, height - 1 - roomHeight);
            for (int x = roomX; x < roomX + roomWidth; x++) {
                for (int y = roomY; y < roomY + roomHeight; y++) {
                    grid[x][y] = "r";
                }
            }
        }

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                switch (grid[i][j]) {
                    case "w":
                        if (i > 0) {
                            if (grid[i - 1][j] != "w") CreateWestWall(i, j, 0);
                            if (grid[i - 1][j] == "r") CreateWestWall(i, j, 1);
                        }
                        if (i < width - 1) {
                            if (grid[i + 1][j] != "w") CreateEastWall(i, j, 0);
                            if (grid[i + 1][j] == "r") CreateEastWall(i, j, 1);
                        }
                        if (j > 0) {
                            if (grid[i][j - 1] != "w") CreateSouthWall(i, j, 0);
                            if (grid[i][j - 1] == "r") CreateSouthWall(i, j, 1);
                        }
                        if (j < height - 1) {
                            if (grid[i][j + 1] != "w") CreateNorthWall(i, j, 0);
                            if (grid[i][j + 1] == "r") CreateNorthWall(i, j, 1);
                        }
                        break;
                    case "p":
                        CreateFloor(i, j);
                        CreateCeiling(i, j, 0);
                        if (i > 0          && grid[i - 1][j] == "r") CreateWestWall(i, j, 1);
                        if (i < width - 1  && grid[i + 1][j] == "r") CreateEastWall(i, j, 1);
                        if (j > 0          && grid[i][j - 1] == "r") CreateSouthWall(i, j, 1);
                        if (j < height - 1 && grid[i][j + 1] == "r") CreateNorthWall(i, j, 1);
                        break;
                    case "r":
                        CreateFloor(i, j);
                        CreateCeiling(i, j, 1);
                        break; 
                    default:
                        break;
                }
            }
        }
    }

    private void MazeDepthFirstSearch(List<List<string>> grid, List<int> currentCell)
    {
        grid[currentCell[0]][currentCell[1]] = "p";

        List<List<int>> neighbours = new List<List<int>>();
        List<int> neighbour;
        if (currentCell[0] >= 2 && grid[currentCell[0] - 2][currentCell[1]] == "w") {
            neighbour = new List<int>();
            neighbour.Add(currentCell[0] - 2);
            neighbour.Add(currentCell[1]);
            neighbour.Add(0);
            neighbours.Add(neighbour);
        }
        if (currentCell[0] <= (width - 1) - 2 && grid[currentCell[0] + 2][currentCell[1]] == "w") {
            neighbour = new List<int>();
            neighbour.Add(currentCell[0] + 2);
            neighbour.Add(currentCell[1]);
            neighbour.Add(1);
            neighbours.Add(neighbour);
        }
        if (currentCell[1] >= 2 && grid[currentCell[0]][currentCell[1] - 2] == "w") {
            neighbour = new List<int>();
            neighbour.Add(currentCell[0]);
            neighbour.Add(currentCell[1] - 2);
            neighbour.Add(2);
            neighbours.Add(neighbour);
        }
        if (currentCell[1] <= (height - 1) - 2 && grid[currentCell[0]][currentCell[1] + 2] == "w") {
            neighbour = new List<int>();
            neighbour.Add(currentCell[0]);
            neighbour.Add(currentCell[1] + 2);
            neighbour.Add(3);
            neighbours.Add(neighbour);
        }

        while (neighbours.Count != 0) {
            List<int> chosenNeighbour = neighbours[Random.Range(0, neighbours.Count)];
            if (grid[chosenNeighbour[0]][chosenNeighbour[1]] == "w") {
                grid[chosenNeighbour[0]][chosenNeighbour[1]] = "p";
                if (chosenNeighbour[2] == 0) grid[currentCell[0] - 1][currentCell[1]] = "p";
                if (chosenNeighbour[2] == 1) grid[currentCell[0] + 1][currentCell[1]] = "p";
                if (chosenNeighbour[2] == 2) grid[currentCell[0]][currentCell[1] - 1] = "p";
                if (chosenNeighbour[2] == 3) grid[currentCell[0]][currentCell[1] + 1] = "p";
                MazeDepthFirstSearch(grid, chosenNeighbour);
            }
            neighbours.Remove(chosenNeighbour);
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
        CreateWall(northWallPrefab, new Vector3(x, level, y) + new Vector3(0, northWallPrefab.transform.position.y, northWallPrefab.transform.position.z));
    }

    private void CreateEastWall(int x, int y, int level)
    {
        CreateWall(eastWallPrefab, new Vector3(x, level, y) + new Vector3(eastWallPrefab.transform.position.x, eastWallPrefab.transform.position.y, 0));
    }

    private void CreateSouthWall(int x, int y, int level)
    {
        CreateWall(southWallPrefab, new Vector3(x, level, y) + new Vector3(0, southWallPrefab.transform.position.y, southWallPrefab.transform.position.z));
    }

    private void CreateWestWall(int x, int y, int level)
    {
        CreateWall(westWallPrefab, new Vector3(x, level, y) + new Vector3(westWallPrefab.transform.position.x, westWallPrefab.transform.position.y, 0));
    }

    private void CreateWall(GameObject prefab, Vector3 position)
    {
        GameObject wall = CreateMazePiece(prefab, position);
        wall.transform.parent = wallParent.transform;
    }

    private GameObject CreateMazePiece(GameObject prefab, Vector3 position)
    {
        GameObject mazePiece = Instantiate(prefab, position, prefab.transform.rotation);
        mazePiece.GetComponent<MeshRenderer>().material = materials[Random.Range(0, materials.Count)];
        return mazePiece;
    }

}
