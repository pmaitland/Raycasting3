using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEnd : MonoBehaviour {

    public Texture2D carpetN;
    public Texture2D carpetE;
    public Texture2D carpetS;
    public Texture2D carpetW;

    Transform carpet;
    Transform floor;

    void Awake() {
        carpet = transform.Find("Maze Pieces").Find("Carpet");
        floor = transform.Find("Maze Pieces").Find("Floor");
    }

    public void SetDirectionNorth() {
        if (carpet != null) { carpet.GetComponent<MeshRenderer>().material.mainTexture = carpetN; }
        Rotate(180);
    }

    public void SetDirectionEast() {
        if (carpet != null) { carpet.GetComponent<MeshRenderer>().material.mainTexture = carpetE; }
        Rotate(270);
    }

    public void SetDirectionSouth() {
        if (carpet != null) { carpet.GetComponent<MeshRenderer>().material.mainTexture = carpetS; }
    }

    public void SetDirectionWest() {
        if (carpet != null) { carpet.GetComponent<MeshRenderer>().material.mainTexture = carpetW; }
        Rotate(90);
    }

    private void Rotate(float amount) {
        transform.Rotate(0, amount, 0);
        carpet?.Rotate(0, 0, amount);
        floor.Rotate(0, 0, amount);
    }

}
