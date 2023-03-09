using UnityEngine;

public class SpriteBehaviour : MonoBehaviour {

    public Sprite[] sprites;
    public Sprite destroyedSprite;

    public bool useLighting;

    private Transform player;
    private Transform body;
    private SpriteRenderer spriteRenderer;
    private Health health;

    private bool usingDestroyedSprite;

    private GameBehaviour gameBehaviour;

    void Start() {
        player = GameObject.FindWithTag("Player").transform;

        body = transform.parent.Find("Body");
        if (body == null) body = transform;

        spriteRenderer = GetComponent<SpriteRenderer>();

        health = GetComponentInParent<Health>();

        usingDestroyedSprite = false;

        gameBehaviour = GameObject.Find("Controller").GetComponent<GameBehaviour>();
    }

    void Update() {
        if (!usingDestroyedSprite) {
            if (health != null && health.GetCurrentHealth() <= 0) {
                spriteRenderer.sprite = destroyedSprite;
                usingDestroyedSprite = true;
            } else {
                float angle = Vector3.SignedAngle(body.position - player.position, body.forward, Vector3.up) + 180 + ((360 / sprites.Length) / 2);
                if (angle > 360) angle -= 360;
                int spriteIndex = (int) (angle / 360 * sprites.Length);
                spriteRenderer.sprite = sprites[spriteIndex];
            }
        }

        transform.forward = new Vector3(player.forward.x, transform.forward.y, player.forward.z);

        if (useLighting) {
            LightingType lightingType = gameBehaviour.GetMazeCell(transform.position.x, transform.position.z).GetLightingLower();
            spriteRenderer.material.color = Lighting.GetColor(lightingType);
        } else {
            spriteRenderer.material.color = Color.white;
        }
    }
}
