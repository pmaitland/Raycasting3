using UnityEngine;

public class DeadEnd : MonoBehaviour
{

    public Texture2D CarpetN;
    public Texture2D CarpetE;
    public Texture2D CarpetS;
    public Texture2D CarpetW;

    private Transform _carpet;
    private Transform _floor;

    void Awake()
    {
        _carpet = transform.Find("Maze Pieces").Find("Carpet");
        _floor = transform.Find("Maze Pieces").Find("Floor");
    }

    public void SetDirectionNorth()
    {
        if (_carpet != null) { _carpet.GetComponent<MeshRenderer>().material.mainTexture = CarpetN; }
        Rotate(180);
    }

    public void SetDirectionEast()
    {
        if (_carpet != null) { _carpet.GetComponent<MeshRenderer>().material.mainTexture = CarpetE; }
        Rotate(270);
    }

    public void SetDirectionSouth()
    {
        if (_carpet != null) { _carpet.GetComponent<MeshRenderer>().material.mainTexture = CarpetS; }
    }

    public void SetDirectionWest()
    {
        if (_carpet != null) { _carpet.GetComponent<MeshRenderer>().material.mainTexture = CarpetW; }
        Rotate(90);
    }

    private void Rotate(float amount)
    {
        transform.Rotate(0, amount, 0);
        _carpet.Rotate(0, 0, amount);
        _floor.Rotate(0, 0, amount);
    }

}
