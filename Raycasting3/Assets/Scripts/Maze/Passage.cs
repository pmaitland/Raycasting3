using UnityEngine;

public class Passage : MonoBehaviour
{

    public enum WallState
    {
        CLOSED,
        LOWER_OPEN,
        UPPER_OPEN,
        OPEN,
        WINDOW
    }

    public Texture2D CarpetN;
    public Texture2D CarpetE;
    public Texture2D CarpetS;
    public Texture2D CarpetW;
    public Texture2D CarpetNS;
    public Texture2D CarpetEW;
    public Texture2D CarpetNE;
    public Texture2D CarpetES;
    public Texture2D CarpetSW;
    public Texture2D CarpetNW;
    public Texture2D CarpetNES;
    public Texture2D CarpetESW;
    public Texture2D CarpetNSW;
    public Texture2D CarpetNEW;
    public Texture2D CarpetNESW;

    private Transform _carpet;

    private Transform _maze;

    private Color _colour;

    void Awake()
    {
        _carpet = transform.Find("Maze Pieces").Find("Carpet");
    }

    public void Setup(Transform maze, WallState northState, WallState eastState, WallState southState, WallState westState, Color colour)
    {
        _maze = maze;
        _colour = colour;

        int openNeighbourCount = 0;

        if (northState == WallState.OPEN) { OpenNorth(); openNeighbourCount++; }
        else if (northState == WallState.LOWER_OPEN) { OpenNorthLower(); }
        else if (northState == WallState.UPPER_OPEN) { OpenNorthUpper(); }
        else if (northState == WallState.WINDOW) { PlaceNorthWindow(); }
        else if (Random.Range(0, 100) < 10) { transform.Find("Maze Pieces").Find("Banner-N")?.gameObject.SetActive(true); }

        if (eastState == WallState.OPEN) { OpenEast(); openNeighbourCount++; }
        else if (eastState == WallState.LOWER_OPEN) { OpenEastLower(); }
        else if (eastState == WallState.UPPER_OPEN) { OpenEastUpper(); }
        else if (eastState == WallState.WINDOW) { PlaceEastWindow(); }
        else if (Random.Range(0, 100) < 10) { transform.Find("Maze Pieces").Find("Banner-E")?.gameObject.SetActive(true); }

        if (southState == WallState.OPEN) { OpenSouth(); openNeighbourCount++; }
        else if (southState == WallState.LOWER_OPEN) { OpenSouthLower(); }
        else if (southState == WallState.UPPER_OPEN) { OpenSouthUpper(); }
        else if (southState == WallState.WINDOW) { PlaceSouthWindow(); }
        else if (Random.Range(0, 100) < 10) { transform.Find("Maze Pieces").Find("Banner-S")?.gameObject.SetActive(true); }

        if (westState == WallState.OPEN) { OpenWest(); openNeighbourCount++; }
        else if (westState == WallState.LOWER_OPEN) { OpenWestLower(); }
        else if (westState == WallState.UPPER_OPEN) { OpenWestUpper(); }
        else if (westState == WallState.WINDOW) { PlaceWestWindow(); }
        else if (Random.Range(0, 100) < 10) { transform.Find("Maze Pieces").Find("Banner-W")?.gameObject.SetActive(true); }


        if (openNeighbourCount == 1)
        {
            if (northState == WallState.OPEN) { PlaceCarpet(CarpetN); }
            else if (eastState == WallState.OPEN) { PlaceCarpet(CarpetE); }
            else if (southState == WallState.OPEN) { PlaceCarpet(CarpetS); }
            else if (westState == WallState.OPEN) { PlaceCarpet(CarpetW); }
        }
        else if (openNeighbourCount == 2)
        {
            if (northState == WallState.OPEN)
            {
                if (eastState == WallState.OPEN) { PlaceCarpet(CarpetNE); }
                else if (southState == WallState.OPEN) { PlaceCarpet(CarpetNS); }
                else if (westState == WallState.OPEN) { PlaceCarpet(CarpetNW); }
            }
            else if (eastState == WallState.OPEN)
            {
                if (southState == WallState.OPEN) { PlaceCarpet(CarpetES); }
                else if (westState == WallState.OPEN) { PlaceCarpet(CarpetEW); }
            }
            else if (southState == WallState.OPEN)
            {
                if (westState == WallState.OPEN) { PlaceCarpet(CarpetSW); }
            }
        }
        else if (openNeighbourCount == 3)
        {
            if (northState == WallState.OPEN && eastState == WallState.OPEN && southState == WallState.OPEN) { PlaceCarpet(CarpetNES); }
            else if (eastState == WallState.OPEN && southState == WallState.OPEN && westState == WallState.OPEN) { PlaceCarpet(CarpetESW); }
            else if (northState == WallState.OPEN && southState == WallState.OPEN && westState == WallState.OPEN) { PlaceCarpet(CarpetNSW); }
            else if (northState == WallState.OPEN && eastState == WallState.OPEN && westState == WallState.OPEN) { PlaceCarpet(CarpetNEW); }
            PlaceChandelier();
        }
        else if (openNeighbourCount == 4)
        {
            PlaceCarpet(CarpetNESW);
            PlaceChandelier();
        }
    }

    private void OpenNorth()
    {
        OpenNorthLower();
        OpenNorthUpper();
    }

    private void OpenNorthLower()
    {
        foreach (Transform mazePiece in transform.Find("Maze Pieces"))
        {
            if (mazePiece.name == "Wall-N-L0") mazePiece.gameObject.SetActive(false);
        }
    }

    private void OpenNorthUpper()
    {
        foreach (Transform mazePiece in transform.Find("Maze Pieces"))
        {
            if (mazePiece.name == "Wall-N-L1") mazePiece.gameObject.SetActive(false);
        }
    }

    private void PlaceNorthWindow()
    {
        SetWindowLighting();
        transform.Find("Maze Pieces").Find("Wall-N-L0").gameObject.SetActive(false);
        transform.Find("Maze Pieces").Find("Wall-N-L1")?.gameObject.SetActive(false);
        transform.Find("Maze Pieces").Find("Wall-N-L0-Window")?.gameObject.SetActive(true);
        transform.Find("Maze Pieces").Find("Wall-N-L1-Window")?.gameObject.SetActive(true);
    }

    private void OpenEast()
    {
        OpenEastLower();
        OpenEastUpper();
    }

    private void OpenEastLower()
    {
        foreach (Transform mazePiece in transform.Find("Maze Pieces"))
        {
            if (mazePiece.name == "Wall-E-L0") mazePiece.gameObject.SetActive(false);
        }
    }

    private void OpenEastUpper()
    {
        foreach (Transform mazePiece in transform.Find("Maze Pieces"))
        {
            if (mazePiece.name == "Wall-E-L1") mazePiece.gameObject.SetActive(false);
        }
    }

    private void PlaceEastWindow()
    {
        SetWindowLighting();
        transform.Find("Maze Pieces").Find("Wall-E-L0").gameObject.SetActive(false);
        transform.Find("Maze Pieces").Find("Wall-E-L1")?.gameObject.SetActive(false);
        transform.Find("Maze Pieces").Find("Wall-E-L0-Window")?.gameObject.SetActive(true);
        transform.Find("Maze Pieces").Find("Wall-E-L1-Window")?.gameObject.SetActive(true);
    }

    private void OpenSouth()
    {
        OpenSouthLower();
        OpenSouthUpper();
    }

    private void OpenSouthLower()
    {
        foreach (Transform mazePiece in transform.Find("Maze Pieces"))
        {
            if (mazePiece.name == "Wall-S-L0") mazePiece.gameObject.SetActive(false);
        }
    }

    private void OpenSouthUpper()
    {
        foreach (Transform mazePiece in transform.Find("Maze Pieces"))
        {
            if (mazePiece.name == "Wall-S-L1") mazePiece.gameObject.SetActive(false);
        }
    }

    private void PlaceSouthWindow()
    {
        SetWindowLighting();
        transform.Find("Maze Pieces").Find("Wall-S-L0").gameObject.SetActive(false);
        transform.Find("Maze Pieces").Find("Wall-S-L1")?.gameObject.SetActive(false);
        transform.Find("Maze Pieces").Find("Wall-S-L0-Window")?.gameObject.SetActive(true);
        transform.Find("Maze Pieces").Find("Wall-S-L1-Window")?.gameObject.SetActive(true);
    }

    private void OpenWest()
    {
        OpenWestLower();
        OpenWestUpper();
    }

    private void OpenWestLower()
    {
        foreach (Transform mazePiece in transform.Find("Maze Pieces"))
        {
            if (mazePiece.name == "Wall-W-L0") mazePiece.gameObject.SetActive(false);
        }
    }

    private void OpenWestUpper()
    {
        foreach (Transform mazePiece in transform.Find("Maze Pieces"))
        {
            if (mazePiece.name == "Wall-W-L1") mazePiece.gameObject.SetActive(false);
        }
    }

    private void PlaceWestWindow()
    {
        SetWindowLighting();
        transform.Find("Maze Pieces").Find("Wall-W-L0").gameObject.SetActive(false);
        transform.Find("Maze Pieces").Find("Wall-W-L1")?.gameObject.SetActive(false);
        transform.Find("Maze Pieces").Find("Wall-W-L0-Window")?.gameObject.SetActive(true);
        transform.Find("Maze Pieces").Find("Wall-W-L1-Window")?.gameObject.SetActive(true);
    }

    private void PlaceCarpet(Texture2D texture)
    {
        if (_carpet != null)
        {
            _carpet.GetComponent<MeshRenderer>().material.mainTexture = texture;
            _carpet.GetComponent<MeshRenderer>().material.color = _colour;
        }
    }

    private void PlaceChandelier()
    {
        if (transform.Find("Maze Pieces").Find("Chandelier") != null)
        {
            transform.Find("Maze Pieces").Find("Chandelier").gameObject.SetActive(true);
            _maze.GetComponent<MazeGenerator>().AddToUpperLightSources(transform.Find("Maze Pieces").Find("Chandelier").gameObject, Lighting.Type.TORCH_0);
        }
    }

    private void SetWindowLighting()
    {
        if (transform.Find("Maze Pieces").Find("Ceiling") != null)
        {
            _maze.GetComponent<MazeGenerator>().AddToLowerLightSources(transform.Find("Maze Pieces").Find("Ceiling").gameObject, Lighting.Type.LIGHT_SPELL_0);
        }
        _maze.GetComponent<MazeGenerator>().AddToUpperLightSources(transform.Find("Maze Pieces").Find("Floor").gameObject, Lighting.Type.LIGHT_SPELL_0);
    }

}
