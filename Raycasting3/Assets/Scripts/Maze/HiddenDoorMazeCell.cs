using UnityEngine;

public class HiddenDoorMazeCell : MazeCell
{

    protected GameObject HiddenDoor { private get; set; }

    public HiddenDoorMazeCell(int x, int y) : base(x, y, Type.HIDDEN_DOOR, Color.clear) { }

    public bool IsDoorOpen()
    {
        return HiddenDoor.GetComponent<HiddenDoorBehaviour>().IsOpen();
    }

}