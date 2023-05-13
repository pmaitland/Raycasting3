using UnityEngine;

public class PauseScreenBehaviour : MonoBehaviour
{

    private GameObject _pauseScreen;

    void Start()
    {
        _pauseScreen = GameObject.Find("Pause Screen");
        _pauseScreen.SetActive(false);
    }

    public void Pause()
    {
        _pauseScreen.SetActive(true);
    }

    public void Unpause()
    {
        _pauseScreen.SetActive(false);
    }

}
