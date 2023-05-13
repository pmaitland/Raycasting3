using UnityEngine;

public class ChestBehaviour : MonoBehaviour
{

    public GameObject[] PossibleItems;

    private Transform _lid;
    private Transform _hinge;

    private float _rotation = 0.0f;

    private bool _open = false;
    private bool _opening = false;

    void Start()
    {
        _lid = transform.Find("Lid");
        _hinge = transform.Find("Hinge");
    }

    void Update()
    {
        float angle = 5;
        if (_opening)
        {
            _lid.RotateAround(_hinge.position, Vector3.left, angle);
            _rotation += angle;
            if (_rotation >= 135.0f)
            {
                _open = true;
                _opening = false;
                SpawnItem();
            }
        }
    }

    public void Open()
    {
        if (!_open) _opening = true;
    }

    private void SpawnItem()
    {
        GameObject chosenItem = PossibleItems[Random.Range(0, PossibleItems.Length)];
        GameObject item = Instantiate(chosenItem, transform.position, chosenItem.transform.rotation);
        item.transform.parent = transform;
    }
}
