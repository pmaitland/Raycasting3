using UnityEngine;
using UnityEngine.UI;

public class HandsBehaviour : MonoBehaviour {

    private GameObject rightHand;
    private GameObject leftHand;

    private GameObject player;
    private GameBehaviour gameBehaviour;

    public Sprite handSprite;
    public Sprite handCastingSprite;

    void Start() {
        rightHand = GameObject.Find("Right Hand");
        leftHand = GameObject.Find("Left Hand");

        player = GameObject.Find("Player");
        gameBehaviour = GameObject.Find("Controller").GetComponent<GameBehaviour>();
    }

    void Update() {
        LightingType lightingType = gameBehaviour.GetMazeCell(player.transform.position.x, player.transform.position.z).GetLighting();
        rightHand.GetComponent<Image>().color = LightingColor.GetLightingColor(lightingType);
        leftHand.GetComponent<Image>().color = LightingColor.GetLightingColor(lightingType);
    }

    public void ChangeHandSprite(Hand chosenHand, HandState newState) {
        GameObject hand = rightHand;

        switch (chosenHand) {
            case Hand.RIGHT:
                hand = rightHand;
                break;
            case Hand.LEFT:
                hand = leftHand;
                break;
            default:
                break;
        }

        switch (newState) {
            case HandState.NORMAL:
                hand.GetComponent<Image>().sprite = handSprite;
                break;
            case HandState.CASTING:
                hand.GetComponent<Image>().sprite = handCastingSprite;
                break;
            default:
                break;
        }
    }

}
