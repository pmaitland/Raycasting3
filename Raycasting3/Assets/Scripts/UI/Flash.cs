using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flash : MonoBehaviour {

    Image image;
    
    void Start() {
        image = GetComponent<Image>();
    }

    void Update() {
        if (image.color.a > 0) {
            image.color -= new Color(0, 0, 0, 0.01f);
        }
    }

    private void DoFlash(Color color) {
        image.color = color;
        image.color -= new Color(0, 0, 0, 0.75f);
    }

    public void Health() {
        DoFlash(Color.green);
    }

    public void Mana() {
        DoFlash(Color.blue);
    }

    public void Damage() {
        DoFlash(Color.red);
    }

    public void Pickup() {
        DoFlash(Color.yellow);
    }
}
