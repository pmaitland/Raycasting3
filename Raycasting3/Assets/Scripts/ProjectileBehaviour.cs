using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{

    public float MoveSpeed = 4.0f;

    public GameObject Creator { private get; set; }

    void Update()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * MoveSpeed;

        if (Mathf.Abs(transform.position.x) > 100 || Mathf.Abs(transform.position.z) > 100)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == Creator.name) { return; }
        other.GetComponentInParent<Health>()?.ReduceHealth(Creator, 1);
        Destroy(gameObject);
    }

}
