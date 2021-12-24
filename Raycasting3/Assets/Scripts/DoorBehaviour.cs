using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    private float openY;
    private bool open = false;
    private bool opening = false;

    void Start()
    {
        openY = transform.position.y + 0.75f;
    }

    void Update()
    {
        if (opening) transform.position += new Vector3(0, 0.005f, 0);
        if (transform.position.y >= openY) {
            opening = false;
            open = true;
        }
    }

    public void Open()
    {
        if (!open) opening = true;
    }
}
