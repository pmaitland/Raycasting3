using UnityEngine;

public class SpriteBehaviour : MonoBehaviour {

    public Sprite[] sprites;
    public Sprite destroyedSprite;

    private Transform player;
    private Transform body;
    private Transform sprite;
    private Health health;

    private bool usingDestroyedSprites;

    void Start() {
        player = GameObject.FindWithTag("Player").transform;

        body = transform.parent.Find("Body");
        if (body == null) body = transform;

        sprite = transform.parent.Find("Sprite");
        sprite.GetComponent<MeshRenderer>().material.mainTexture = sprites[0].texture;

        health = GetComponentInParent<Health>();

        usingDestroyedSprites = false;
    }

    void Update() {
        if (!usingDestroyedSprites) {
            if (health != null && health.GetCurrentHealth() <= 0) {
                sprite.GetComponent<MeshRenderer>().material.mainTexture = destroyedSprite.texture;
                usingDestroyedSprites = true;
            } else {
                float angle = Vector3.SignedAngle(body.position - player.position, body.forward, Vector3.up) + 180 + ((360 / sprites.Length) / 2);
                if (angle > 360) angle -= 360;
                int spriteIndex = (int) (angle / 360 * sprites.Length);
                sprite.GetComponent<MeshRenderer>().material.mainTexture = sprites[spriteIndex].texture;
            }
        }

        transform.forward = new Vector3(player.forward.x, transform.forward.y, player.forward.z);
    }
}
