using UnityEngine;

public class Bobbing : MonoBehaviour
{
    public float WalkingBobbingSpeed = 14f;
    public float BobbingAmount = 0.05f;

    private PlayerBehaviour _player;

    private float _defaultPosY = 0;
    private float _timer = 0;

    void Start()
    {
        _defaultPosY = transform.localPosition.y;
        _player = GameObject.Find("Player").GetComponent<PlayerBehaviour>();
    }

    void Update()
    {
        if (Mathf.Abs(_player.Horizontal) > 0.1f || Mathf.Abs(_player.Vertical) > 0.1f)
        {
            _timer += Time.deltaTime * WalkingBobbingSpeed;
            transform.localPosition = new Vector3(transform.localPosition.x, _defaultPosY + Mathf.Sin(_timer) * BobbingAmount, transform.localPosition.z);
        }
        else
        {
            _timer = 0;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, _defaultPosY, Time.deltaTime * WalkingBobbingSpeed), transform.localPosition.z);
        }
    }
}
