using System.Collections.Generic;

using UnityEngine;

public class FistBehaviour : MonoBehaviour
{

    private GameObject _player;

    private List<GameObject> _objectsHit;

    void Start()
    {
        _player = GameObject.Find("Player");
        _objectsHit = new List<GameObject>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (_objectsHit.Contains(other.gameObject)) { return; }
        if (other.gameObject.name == _player.name) { return; }

        if (other.gameObject.GetComponent<Health>() != null)
        {
            other.gameObject.GetComponent<Health>().ReduceHealth(_player, 1);
        }
        else
        {
            other.transform.parent.gameObject.GetComponent<Health>()?.ReduceHealth(_player, 1);
        }

        _objectsHit.Add(other.gameObject);
    }

    public void ResetObjectsHit()
    {
        _objectsHit = new List<GameObject>();
    }
}
