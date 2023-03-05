using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobbing : MonoBehaviour {
    public float walkingBobbingSpeed = 14f;
    public float bobbingAmount = 0.05f;
    
    private PlayerBehaviour player;

    float defaultPosY = 0;
    float timer = 0;

    void Start() {
        defaultPosY = transform.localPosition.y;
        player = GameObject.Find("Player").GetComponent<PlayerBehaviour>();
    }

    void Update() {
        if (Mathf.Abs(player.GetHorizontal()) > 0.1f || Mathf.Abs(player.GetVertical()) > 0.1f) {
            timer += Time.deltaTime * walkingBobbingSpeed;
            transform.localPosition = new Vector3(transform.localPosition.x, defaultPosY + Mathf.Sin(timer) * bobbingAmount, transform.localPosition.z);
        } else {
            timer = 0;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, defaultPosY, Time.deltaTime * walkingBobbingSpeed), transform.localPosition.z);
        }
    }
}
