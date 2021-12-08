using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprite3D : MonoBehaviour
{
    public List<Sprite> sprites;

    private Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        float angle = Vector3.SignedAngle(transform.position - player.position, transform.forward, Vector3.up) + 180 + ((360 / sprites.Count) / 2);
        if (angle > 360) angle -= 360;
        int spriteIndex = (int) (angle / 360 * sprites.Count);
        transform.parent.transform.Find("Mesh").GetComponent<MeshRenderer>().material.mainTexture = sprites[spriteIndex].texture;
    }
}
