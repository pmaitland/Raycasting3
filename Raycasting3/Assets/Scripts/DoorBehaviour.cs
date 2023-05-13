using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{

    private Transform _door1;
    private Transform _door2;

    private float _rotation = 0.0f;

    private bool _open = false;
    private bool _opening = false;
    private bool _openInOppositeDirection;

    void Start()
    {
        _door1 = transform.Find("Door 1");
        _door2 = transform.Find("Door 2");
    }

    void Update()
    {
        float angle = 5;
        if (_opening)
        {
            if (_openInOppositeDirection)
            {
                _door1.RotateAround(_door1.Find("Hinge").position, -Vector3.up, angle);
                _door2.RotateAround(_door2.Find("Hinge").position, Vector3.up, angle);
            }
            else
            {
                _door1.RotateAround(_door1.Find("Hinge").position, Vector3.up, angle);
                _door2.RotateAround(_door2.Find("Hinge").position, -Vector3.up, angle);
            }
            _rotation += angle;
            if (_rotation >= 75.0f)
            {
                _open = true;
                _opening = false;
            }
        }
    }

    public void Open(bool oppositeDirection)
    {
        if (!_open) _opening = true;
        _openInOppositeDirection = oppositeDirection;
    }
}
