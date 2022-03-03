using UnityEngine;

public class DoorBehaviour : MonoBehaviour {

    private Transform door1;
    private Transform door2;
    private float speed = 200.0f;
    private float rotation = 0.0f;
    private bool open = false;
    private bool opening = false;
    private bool openInOppositeDirection;

    void Start() {
        door1 = transform.Find("Door 1");
        door2 = transform.Find("Door 2");
    }

    void Update() {
        float angle = speed * Time.deltaTime;
        if (opening) {
            if (openInOppositeDirection) {
                door1.RotateAround(door1.Find("Hinge").position, -Vector3.up, angle);
                door2.RotateAround(door2.Find("Hinge").position, Vector3.up, angle);
            } else {
                door1.RotateAround(door1.Find("Hinge").position, Vector3.up, angle);
                door2.RotateAround(door2.Find("Hinge").position, -Vector3.up, angle);
            }
            rotation += angle;
            if (rotation >= 85.0f) {
                open = true;
                opening = false;
            }
        }
    }

    public void Open(bool oppositeDirection) {
        if (!open) opening = true;
        openInOppositeDirection = oppositeDirection;
    }
}
