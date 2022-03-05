using UnityEngine;
using UnityEngine.UI;

public class HandsBehaviour : MonoBehaviour {

    private GameObject rightHand;
    private GameObject leftHand;

    public Sprite handSprite;
    public Sprite handCastingSprite;

    void Start() {
        rightHand = GameObject.Find("Right Hand");
        leftHand = GameObject.Find("Left Hand");
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
