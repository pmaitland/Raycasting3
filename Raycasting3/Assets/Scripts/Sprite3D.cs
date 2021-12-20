using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprite3D : MonoBehaviour
{
    public List<Sprite> sprites;

    private Transform player;
    private Transform body;
    private Transform sprite;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        body = transform.parent.Find("Body");
        sprite = transform.parent.Find("Sprite");
    }

    void Update()
    {
        float angle = Vector3.SignedAngle(body.position - player.position, body.forward, Vector3.up) + 180 + ((360 / sprites.Count) / 2);
        if (angle > 360) angle -= 360;
        int spriteIndex = (int) (angle / 360 * sprites.Count);
        sprite.GetComponent<MeshRenderer>().material.mainTexture = sprites[spriteIndex].texture;
    }
}
