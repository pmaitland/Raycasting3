using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int width = 20;
    public int height = 20;
    public Material material;
    public GameObject floorPrefab;
    public GameObject ceilingPrefab;
    public GameObject northWallPrefab;
    public GameObject eastWallPrefab;
    public GameObject southWallPrefab;
    public GameObject westWallPrefab;

    private GameObject floorParent;
    private GameObject ceilingParent;
    private GameObject wallParent;

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
        List<List<int>> grid = new List<List<int>>();

        for (int i = 0; i < width; i++) {
            grid.Add(new List<int>());
            for (int j = 0; j < height; j++) {
                grid[i].Add(1);
            }
        }

        List<int> currentCell = new List<int>();
        currentCell.Add(Random.Range(1, (int) ((width - 1) / 2)) * 2 - 1);
        currentCell.Add(Random.Range(1, (int) ((height - 1) / 2)) * 2 - 1);

        MazeDepthFirstSearch(grid, currentCell);

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                switch (grid[i][j]) {
                    case 0:
                        CreateFloor(i, j);
                        CreateCeiling(i, j);
                        break;
                    case 1:
                        CreateWalls(i, j);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void MazeDepthFirstSearch(List<List<int>> grid, List<int> currentCell)
    {
        grid[currentCell[0]][currentCell[1]] = 0;

        List<List<int>> neighbours = new List<List<int>>();
        List<int> neighbour;
        if (currentCell[0] >= 2 && grid[currentCell[0] - 2][currentCell[1]] == 1) {
            neighbour = new List<int>();
            neighbour.Add(currentCell[0] - 2);
            neighbour.Add(currentCell[1]);
            neighbour.Add(0);
            neighbours.Add(neighbour);
        }
        if (currentCell[0] <= (width - 1) - 2 && grid[currentCell[0] + 2][currentCell[1]] == 1) {
            neighbour = new List<int>();
            neighbour.Add(currentCell[0] + 2);
            neighbour.Add(currentCell[1]);
            neighbour.Add(1);
            neighbours.Add(neighbour);
        }
        if (currentCell[1] >= 2 && grid[currentCell[0]][currentCell[1] - 2] == 1) {
            neighbour = new List<int>();
            neighbour.Add(currentCell[0]);
            neighbour.Add(currentCell[1] - 2);
            neighbour.Add(2);
            neighbours.Add(neighbour);
        }
        if (currentCell[1] <= (height - 1) - 2 && grid[currentCell[0]][currentCell[1] + 2] == 1) {
            neighbour = new List<int>();
            neighbour.Add(currentCell[0]);
            neighbour.Add(currentCell[1] + 2);
            neighbour.Add(3);
            neighbours.Add(neighbour);
        }

        while (neighbours.Count != 0) {
            List<int> chosenNeighbour = neighbours[Random.Range(0, neighbours.Count)];
            if (grid[chosenNeighbour[0]][chosenNeighbour[1]] == 1) {
                grid[chosenNeighbour[0]][chosenNeighbour[1]] = 0;
                if (chosenNeighbour[2] == 0) grid[currentCell[0] - 1][currentCell[1]] = 0;
                if (chosenNeighbour[2] == 1) grid[currentCell[0] + 1][currentCell[1]] = 0;
                if (chosenNeighbour[2] == 2) grid[currentCell[0]][currentCell[1] - 1] = 0;
                if (chosenNeighbour[2] == 3) grid[currentCell[0]][currentCell[1] + 1] = 0;
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

    private void CreateCeiling(int x, int y)
    {
        GameObject ceiling = CreateMazePiece(ceilingPrefab, new Vector3(x, 0, y) + ceilingPrefab.transform.position);
        ceiling.transform.parent = ceilingParent.transform;
    }

    private void CreateWalls(int x, int y)
    {
        CreateNorthWall(x, y);
        CreateEastWall(x, y);
        CreateSouthWall(x, y);
        CreateWestWall(x, y);
    }

    private void CreateNorthWall(int x, int y)
    {
        CreateWall(northWallPrefab, new Vector3(x, 0, y) + new Vector3(0, northWallPrefab.transform.position.y, northWallPrefab.transform.position.z));
    }

    private void CreateEastWall(int x, int y)
    {
        CreateWall(eastWallPrefab, new Vector3(x, 0, y) + new Vector3(eastWallPrefab.transform.position.x, eastWallPrefab.transform.position.y, 0));
    }

    private void CreateSouthWall(int x, int y)
    {
        CreateWall(southWallPrefab, new Vector3(x, 0, y) + new Vector3(0, southWallPrefab.transform.position.y, southWallPrefab.transform.position.z));
    }

    private void CreateWestWall(int x, int y)
    {
        CreateWall(westWallPrefab, new Vector3(x, 0, y) + new Vector3(westWallPrefab.transform.position.x, westWallPrefab.transform.position.y, 0));
    }

    private void CreateWall(GameObject prefab, Vector3 position)
    {
        GameObject wall = CreateMazePiece(prefab, position);
        wall.transform.parent = wallParent.transform;
    }

    private GameObject CreateMazePiece(GameObject prefab, Vector3 position)
    {
        GameObject mazePiece = Instantiate(prefab, position, prefab.transform.rotation);
        mazePiece.GetComponent<MeshRenderer>().material = material;
        return mazePiece;
    }

}
