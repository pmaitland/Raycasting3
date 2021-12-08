using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform player;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }
    
    private void LateUpdate()
    {
        transform.forward = new Vector3(player.forward.x, transform.forward.y, player.forward.z);
    }
}
