using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreenBehaviour : MonoBehaviour {

    private GameObject pauseScreen;

    void Start() {
        pauseScreen = GameObject.Find("Pause Screen");
        pauseScreen.SetActive(false);
    }

    public void Pause() {
        pauseScreen.SetActive(true);
    }

    public void Unpause() {
        pauseScreen.SetActive(false);
    }

}
