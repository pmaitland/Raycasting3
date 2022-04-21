using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestBehaviour : MonoBehaviour {

    public GameObject[] possibleItems;

    private Transform lid;
    private Transform hinge;

    private float rotation = 0.0f;

    private bool open = false;
    private bool opening = false;

    void Start() {
        lid = transform.Find("Lid");
        hinge = transform.Find("Hinge");
    }

    void Update() {
        float angle = 5;
        if (opening) {
            lid.RotateAround(hinge.position, Vector3.left, angle);
            rotation += angle;
            if (rotation >= 135.0f) {
                open = true;
                opening = false;
                SpawnItem();
            }
        }
    }

    public void Open() {
        if (!open) opening = true;
    }

    private void SpawnItem() {
        GameObject chosenItem = possibleItems[Random.Range(0, possibleItems.Length)];
        GameObject item = Instantiate(chosenItem, transform.position, chosenItem.transform.rotation);
        item.transform.parent = transform;
    }
}
