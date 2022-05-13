using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistBehaviour : MonoBehaviour {

    private GameObject player;
    
    private List<GameObject> objectsHit;
    
    void Start() {
        player = GameObject.Find("Player");

        objectsHit = new List<GameObject>();
    }

    void OnTriggerEnter(Collider other) {
        if (objectsHit.Contains(other.gameObject)) return;
        if (other.gameObject.name == player.name) return;
        
        if (other.gameObject.GetComponent<Health>() != null)
            other.gameObject.GetComponent<Health>().ReduceHealth(player, 1);
        else if (other.transform.parent.gameObject.GetComponent<Health>() != null)
            other.transform.parent.gameObject.GetComponent<Health>().ReduceHealth(player, 1);

        objectsHit.Add(other.gameObject);   
    }

    public void ResetObjectsHit() {
        objectsHit = new List<GameObject>();
    }
}
