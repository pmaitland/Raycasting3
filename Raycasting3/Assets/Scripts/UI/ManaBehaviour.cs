using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBehaviour : MonoBehaviour {
    public Sprite manaSprite;
    public Sprite emptyManaSprite;
    public Sprite noManaSprite;

    private PlayerBehaviour playerBehaviour;

    void Start() {
        playerBehaviour = GameObject.Find("Player").GetComponent<PlayerBehaviour>();
    }

    void Update() {
        int playerMaxMana = playerBehaviour.GetMaxMana();
        int playerCurrentMana = playerBehaviour.GetCurrentMana();

        foreach (Transform mana in transform) {
            Image manaImage = mana.GetComponent<Image>();
            int manaNumber = int.Parse(mana.name.Split(' ')[1]);

            if (manaNumber > playerMaxMana) manaImage.sprite = noManaSprite;
            else if (manaNumber > playerCurrentMana) manaImage.sprite = emptyManaSprite;
            else manaImage.sprite = manaSprite;
        }
    }
}
