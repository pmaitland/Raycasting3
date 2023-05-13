using System.Collections;

using UnityEngine;

public class Hover : MonoBehaviour
{

    public float BobbingSpeed = 14f;
    public float BobbingAmount = 0.05f;

    private float _defaultPosY = 0;
    private float _timer = 0;

    void Start()
    {
        _defaultPosY = transform.localPosition.y;
    }

    void Update()
    {
        _timer += Time.deltaTime * BobbingSpeed;
        transform.localPosition = new Vector3(transform.localPosition.x, _defaultPosY + Mathf.Sin(_timer) * BobbingAmount, transform.localPosition.z);
    }
}
