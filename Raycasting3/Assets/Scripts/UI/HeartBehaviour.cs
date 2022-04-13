using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartBehaviour : MonoBehaviour {

    public Sprite heartSprite;
    public Sprite emptyHeartSprite;
    public Sprite noHeartSprite;

    private Health playerHealth;

    void Start() {
        playerHealth = GameObject.Find("Player").GetComponent<Health>();
    }

    void Update() {
        int playerMaxHealth = playerHealth.GetMaxHealth();
        int playerCurrentHealth = playerHealth.GetCurrentHealth();

        foreach (Transform heart in transform) {
            Image heartImage = heart.GetComponent<Image>();
            int heartNumber = int.Parse(heart.name.Split(' ')[1]);

            if (heartNumber > playerMaxHealth) heartImage.sprite = noHeartSprite;
            else if (heartNumber > playerCurrentHealth) heartImage.sprite = emptyHeartSprite;
            else heartImage.sprite = heartSprite;
        }
    }
}
