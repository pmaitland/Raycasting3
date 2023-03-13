using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passage : MonoBehaviour {

    public enum WallState {
        CLOSED,
        LOWER_OPEN,
        UPPER_OPEN,
        OPEN,
        WINDOW
    }

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

    Transform carpet;

    Transform maze;

    void Awake() {
        carpet = transform.Find("Maze Pieces").Find("Carpet");

        int random;
        foreach (Transform mazePiece in transform.Find("Maze Pieces")) {
            if (mazePiece.name.Contains("Wall") && mazePiece.name.Contains("L0") && !mazePiece.name.Contains("Window")) {
                random = Random.Range(0, 100);
                if (random >=  0 && random < 10) {
                    mazePiece.GetComponent<MeshRenderer>().material.mainTexture = banner;
                }
            }
        }
    }

    public void Setup(Transform maze, WallState northState, WallState eastState, WallState southState, WallState westState) {
        this.maze = maze;

        int openNeighbourCount = 0;

        if (northState == WallState.OPEN) { OpenNorth(); openNeighbourCount++; }
        else if (northState == WallState.LOWER_OPEN) { OpenNorthLower(); }
        else if (northState == WallState.UPPER_OPEN) { OpenNorthUpper(); }
        else if (northState == WallState.WINDOW) { PlaceNorthWindow(); }

        if (eastState == WallState.OPEN) { OpenEast(); openNeighbourCount++; }
        else if (eastState == WallState.LOWER_OPEN) { OpenEastLower(); }
        else if (eastState == WallState.UPPER_OPEN) { OpenEastUpper(); }
        else if (eastState == WallState.WINDOW) { PlaceEastWindow(); }

        if (southState == WallState.OPEN) { OpenSouth(); openNeighbourCount++; }
        else if (southState == WallState.LOWER_OPEN) { OpenSouthLower(); }
        else if (southState == WallState.UPPER_OPEN) { OpenSouthUpper(); }
        else if (southState == WallState.WINDOW) { PlaceSouthWindow(); }

        if (westState == WallState.OPEN) { OpenWest(); openNeighbourCount++; }
        else if (westState == WallState.LOWER_OPEN) { OpenWestLower(); }
        else if (westState == WallState.UPPER_OPEN) { OpenWestUpper(); }
        else if (westState == WallState.WINDOW) { PlaceWestWindow(); }


        if (openNeighbourCount == 1) {
            if (northState == WallState.OPEN) { PlaceCarpet(carpetN); }
            else if (eastState == WallState.OPEN) { PlaceCarpet(carpetE); }
            else if (southState == WallState.OPEN) { PlaceCarpet(carpetS); }
            else if (westState == WallState.OPEN) { PlaceCarpet(carpetW); }
        } else if (openNeighbourCount == 2) {
            if (northState == WallState.OPEN) {
                if (eastState == WallState.OPEN) { PlaceCarpet(carpetNE); }
                else if (southState == WallState.OPEN) { PlaceCarpet(carpetNS); }
                else if (westState == WallState.OPEN) { PlaceCarpet(carpetNW); }
            } else if (eastState == WallState.OPEN) {
                if (southState == WallState.OPEN) { PlaceCarpet(carpetES); }
                else if (westState == WallState.OPEN) { PlaceCarpet(carpetEW); }
            } else if (southState == WallState.OPEN) {
                if (westState == WallState.OPEN) { PlaceCarpet(carpetSW); }
            }
        } else if (openNeighbourCount == 3) {
            if (northState == WallState.OPEN && eastState == WallState.OPEN && southState == WallState.OPEN) { PlaceCarpet(carpetNES); }
            else if (eastState == WallState.OPEN && southState == WallState.OPEN && westState == WallState.OPEN) { PlaceCarpet(carpetESW); }
            else if (northState == WallState.OPEN && southState == WallState.OPEN && westState == WallState.OPEN) { PlaceCarpet(carpetNSW); }
            else if (northState == WallState.OPEN && eastState == WallState.OPEN && westState == WallState.OPEN) { PlaceCarpet(carpetNEW); }
            PlaceChandelier();
        } else if (openNeighbourCount == 4) {
            PlaceCarpet(carpetNESW);
            PlaceChandelier();
        }
    }

    private void OpenNorth() {
        OpenNorthLower();
        OpenNorthUpper();
    }

    private void OpenNorthLower() {
        foreach (Transform mazePiece in transform.Find("Maze Pieces")) {
            if (mazePiece.name == "Wall-N-L0") mazePiece.gameObject.SetActive(false);
        }
    }

    private void OpenNorthUpper() {
        foreach (Transform mazePiece in transform.Find("Maze Pieces")) {
            if (mazePiece.name == "Wall-N-L1") mazePiece.gameObject.SetActive(false);
        }
    }

    private void PlaceNorthWindow() {
        PlaceWindow();
        transform.Find("Maze Pieces").Find("Wall-N-L0").gameObject.SetActive(false);
        transform.Find("Maze Pieces").Find("Wall-N-L1").gameObject.SetActive(false);
        transform.Find("Maze Pieces").Find("Wall-N-L0-Window").gameObject.SetActive(true);
        transform.Find("Maze Pieces").Find("Wall-N-L1-Window").gameObject.SetActive(true);
    }

    private void OpenEast() {
        OpenEastLower();
        OpenEastUpper();
    }

    private void OpenEastLower() {
        foreach (Transform mazePiece in transform.Find("Maze Pieces")) {
            if (mazePiece.name == "Wall-E-L0") mazePiece.gameObject.SetActive(false);
        }
    }

    private void OpenEastUpper() {
        foreach (Transform mazePiece in transform.Find("Maze Pieces")) {
            if (mazePiece.name == "Wall-E-L1") mazePiece.gameObject.SetActive(false);
        }
    }

    private void PlaceEastWindow() {
        PlaceWindow();
        transform.Find("Maze Pieces").Find("Wall-E-L0").gameObject.SetActive(false);
        transform.Find("Maze Pieces").Find("Wall-E-L1").gameObject.SetActive(false);
        transform.Find("Maze Pieces").Find("Wall-E-L0-Window").gameObject.SetActive(true);
        transform.Find("Maze Pieces").Find("Wall-E-L1-Window").gameObject.SetActive(true);
    }

    private void OpenSouth() {
        OpenSouthLower();
        OpenSouthUpper();
    }

    private void OpenSouthLower() {
        foreach (Transform mazePiece in transform.Find("Maze Pieces")) {
            if (mazePiece.name == "Wall-S-L0") mazePiece.gameObject.SetActive(false);
        }
    }

    private void OpenSouthUpper() {
        foreach (Transform mazePiece in transform.Find("Maze Pieces")) {
            if (mazePiece.name == "Wall-S-L1") mazePiece.gameObject.SetActive(false);
        }
    }

    private void PlaceSouthWindow() {
        PlaceWindow();
        transform.Find("Maze Pieces").Find("Wall-S-L0").gameObject.SetActive(false);
        transform.Find("Maze Pieces").Find("Wall-S-L1").gameObject.SetActive(false);
        transform.Find("Maze Pieces").Find("Wall-S-L0-Window").gameObject.SetActive(true);
        transform.Find("Maze Pieces").Find("Wall-S-L1-Window").gameObject.SetActive(true);
    }

    private void OpenWest() {
        OpenWestLower();
        OpenWestUpper();
    }

    private void OpenWestLower() {
        foreach (Transform mazePiece in transform.Find("Maze Pieces")) {
            if (mazePiece.name == "Wall-W-L0") mazePiece.gameObject.SetActive(false);
        }
    }

    private void OpenWestUpper() {
        foreach (Transform mazePiece in transform.Find("Maze Pieces")) {
            if (mazePiece.name == "Wall-W-L1") mazePiece.gameObject.SetActive(false);
        }
    }

    private void PlaceWestWindow() {
        PlaceWindow();
        transform.Find("Maze Pieces").Find("Wall-W-L0").gameObject.SetActive(false);
        transform.Find("Maze Pieces").Find("Wall-W-L1").gameObject.SetActive(false);
        transform.Find("Maze Pieces").Find("Wall-W-L0-Window").gameObject.SetActive(true);
        transform.Find("Maze Pieces").Find("Wall-W-L1-Window").gameObject.SetActive(true);
    }

    private void PlaceCarpet(Texture2D texture) {
        carpet.GetComponent<MeshRenderer>().material.mainTexture = texture;
    }

    private void PlaceChandelier() {
        transform.Find("Maze Pieces").Find("Chandelier").gameObject.SetActive(true);
        maze.GetComponent<MazeGenerator>().AddToUpperLightSources(transform.Find("Maze Pieces").Find("Chandelier").gameObject, LightingType.TORCH_0);
    }

    private void PlaceWindow() {
        maze.GetComponent<MazeGenerator>().AddToLowerLightSources(transform.Find("Maze Pieces").Find("Ceiling").gameObject, LightingType.LIGHT_SPELL_0);
        maze.GetComponent<MazeGenerator>().AddToUpperLightSources(transform.Find("Maze Pieces").Find("Floor").gameObject, LightingType.LIGHT_SPELL_0);
    }

}
