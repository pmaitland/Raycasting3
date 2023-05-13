using UnityEngine;

public class SpriteBehaviour : MonoBehaviour
{

    public Sprite[] Sprites;
    public Sprite DestroyedSprite;

    public bool UseLighting;

    private Transform _player;
    private Transform _body;
    private SpriteRenderer _spriteRenderer;
    private Health _health;

    private bool _usingDestroyedSprite;

    private GameBehaviour _gameBehaviour;

    void Start()
    {
        _player = GameObject.FindWithTag("Player").transform;

        _body = transform.parent.Find("Body");
        if (_body == null) _body = transform;

        _spriteRenderer = GetComponent<SpriteRenderer>();

        _health = GetComponentInParent<Health>();

        _usingDestroyedSprite = false;

        _gameBehaviour = GameObject.Find("Controller").GetComponent<GameBehaviour>();
    }

    void Update()
    {
        if (!_usingDestroyedSprite)
        {
            if (_health != null && _health.CurrentHealth <= 0)
            {
                _spriteRenderer.sprite = DestroyedSprite;
                _usingDestroyedSprite = true;
            }
            else
            {
                float angle = Vector3.SignedAngle(_body.position - _player.position, _body.forward, Vector3.up) + 180 + (360 / Sprites.Length / 2);
                if (angle > 360) angle -= 360;
                int spriteIndex = (int)(angle / 360 * Sprites.Length);
                _spriteRenderer.sprite = Sprites[spriteIndex];
            }
        }

        transform.forward = new Vector3(_player.forward.x, transform.forward.y, _player.forward.z);

        if (UseLighting && !_gameBehaviour.GeneratingMaze)
        {
            Lighting.Type? lightingType = _gameBehaviour.GetMazeCell(transform.position.x, transform.position.z)?.LightingLower;
            _spriteRenderer.material.color = Lighting.GetColor(lightingType);
        }
        else
        {
            _spriteRenderer.material.color = Color.white;
        }
    }
}
