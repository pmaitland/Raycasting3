using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEnd : MonoBehaviour {

    public Texture2D carpetN;
    public Texture2D carpetE;
    public Texture2D carpetS;
    public Texture2D carpetW;

    public void SetDirectionNorth() {
        Transform floor = transform.Find("Maze Pieces").Find("Floor");
        floor.GetComponent<MeshRenderer>().material.mainTexture = carpetN;
        Rotate(floor, 180);
    }

    public void SetDirectionEast() {
        Transform floor = transform.Find("Maze Pieces").Find("Floor");
        floor.GetComponent<MeshRenderer>().material.mainTexture = carpetE;
        Rotate(floor, 270);
    }

    public void SetDirectionSouth() {
        Transform floor = transform.Find("Maze Pieces").Find("Floor");
        floor.GetComponent<MeshRenderer>().material.mainTexture = carpetS;
    }

    public void SetDirectionWest() {
        Transform floor = transform.Find("Maze Pieces").Find("Floor");
        floor.GetComponent<MeshRenderer>().material.mainTexture = carpetW;
        Rotate(floor, 90);
    }

    private void Rotate(Transform floor, float amount) {
        transform.Rotate(0, amount, 0);
        floor.Rotate(0, 0, amount);
    }

}
