using UnityEngine;

public class HiddenDoorBehaviour : MonoBehaviour
{

    private float _openY;
    private bool _open = false;
    private bool _opening = false;

    void Start()
    {
        _openY = transform.position.y - 1f;
    }

    void Update()
    {
        if (_opening) { transform.position -= new Vector3(0, 0.005f, 0); }
        if (transform.position.y <= _openY)
        {
            _opening = false;
            _open = true;
            transform.position = new Vector3(transform.position.x, _openY, transform.position.z);
        }
    }

    public void Open()
    {
        if (!_open) _opening = true;
    }

    public bool IsOpen()
    {
        return _open || _opening;
    }
}
