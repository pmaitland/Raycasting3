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

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (x == 0) {
                    CreateEastWall(x, y);
                    CreateWestWall(height - 1, y);
                }

                if (y == 0) {
                    CreateNorthWall(x, y);
                    CreateSouthWall(x, width - 1);
                }

                CreateFloor(x, y);
                CreateCeiling(x, y);
            }
        }
    }

    private void CreateFloor(int x, int y) {
        GameObject floor = Instantiate(floorPrefab, new Vector3(x, 0, y), floorPrefab.transform.rotation);
        floor.GetComponent<MeshRenderer>().material = material;
        floor.transform.parent = floorParent.transform;
    }

    private void CreateCeiling(int x, int y) {
        GameObject ceiling = Instantiate(ceilingPrefab, new Vector3(x, 0, y) + ceilingPrefab.transform.position, ceilingPrefab.transform.rotation);
        ceiling.GetComponent<MeshRenderer>().material = material;
        ceiling.transform.parent = ceilingParent.transform;
    }

    private void CreateNorthWall(int x, int y) {
        CreateWall(northWallPrefab, new Vector3(x, 0, y) + new Vector3(0, northWallPrefab.transform.position.y, northWallPrefab.transform.position.z));
    }

    private void CreateEastWall(int x, int y) {
        CreateWall(eastWallPrefab, new Vector3(x, 0, y) + new Vector3(eastWallPrefab.transform.position.x, eastWallPrefab.transform.position.y, 0));
    }

    private void CreateSouthWall(int x, int y) {
        CreateWall(southWallPrefab, new Vector3(x, 0, y) + new Vector3(0, southWallPrefab.transform.position.y, southWallPrefab.transform.position.z));
    }

    private void CreateWestWall(int x, int y) {
        CreateWall(westWallPrefab, new Vector3(x, 0, y) + new Vector3(westWallPrefab.transform.position.x, westWallPrefab.transform.position.y, 0));
    }

    private void CreateWall(GameObject prefab, Vector3 position) {
        GameObject wall = Instantiate(prefab, position, prefab.transform.rotation);
        wall.GetComponent<MeshRenderer>().material = material;
        wall.transform.parent = wallParent.transform;
    }

}
