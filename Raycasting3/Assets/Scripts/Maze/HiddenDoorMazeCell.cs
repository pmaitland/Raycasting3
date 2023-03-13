using UnityEngine;

public class HiddenDoorMazeCell : MazeCell {

    private GameObject hiddenDoor;

    public HiddenDoorMazeCell(int x, int y) : base(x, y, MazeCellType.HIDDEN_DOOR, Color.clear) { }

    public void SetHiddenDoor(GameObject hiddenDoor) {
        this.hiddenDoor = hiddenDoor;
    }

    public bool IsDoorOpen() {
        return hiddenDoor.GetComponent<HiddenDoorBehaviour>().IsOpen();
    }

}