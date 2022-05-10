using UnityEngine;
using UnityEngine.UI;

public class HandsBehaviour : MonoBehaviour {

    private GameObject hands;
    private GameObject rightHand;
    private GameObject rightSpell;
    private GameObject leftHand;
    private GameObject leftSpell;

    private Vector3 handsShownPosition;
    private Vector3 handsHiddenPosition;

    private GameObject player;

    public Sprite handSprite;
    public Sprite handPreparedSprite;
    public Sprite handCastingSprite;

    public Sprite noSpellSprite;
    public Sprite lightSpellSprite;
    public Sprite fireballSpellSprite;
    public Sprite healSpellSprite;
    public Sprite manaHealSpellSprite;

    void Start() {
        hands = GameObject.Find("Hands");
        rightHand = GameObject.Find("Right Hand");
        rightSpell = GameObject.Find("Right Spell");
        leftHand = GameObject.Find("Left Hand");
        leftSpell = GameObject.Find("Left Spell");

        handsShownPosition = hands.transform.position;
        handsHiddenPosition = new Vector3(
            handsShownPosition.x, 
            -handsShownPosition.y,
            handsShownPosition.z
        );

        player = GameObject.Find("Player");
    }

    void Update() {
        LightingType lightingType = player.GetComponent<PlayerBehaviour>().GetLighting();
        rightHand.GetComponent<Image>().color = Lighting.GetColor(lightingType);
        leftHand.GetComponent<Image>().color = Lighting.GetColor(lightingType);
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
                if (chosenHand == Hand.RIGHT) rightSpell.GetComponent<Image>().sprite = noSpellSprite;
                else if (chosenHand == Hand.LEFT) leftSpell.GetComponent<Image>().sprite = noSpellSprite;
                break;
            case HandState.PREPARED:
                hand.GetComponent<Image>().sprite = handPreparedSprite;
                if (chosenHand == Hand.RIGHT) {
                    switch (player.GetComponent<PlayerBehaviour>().GetCurrentRightSpell()) {
                        case Spell.LIGHT:
                            rightSpell.GetComponent<Image>().sprite = lightSpellSprite;
                            break;
                        case Spell.FIREBALL:
                            rightSpell.GetComponent<Image>().sprite = fireballSpellSprite;
                            break;
                        case Spell.HEAL:
                            rightSpell.GetComponent<Image>().sprite = healSpellSprite;
                            break;
                        case Spell.MANA_HEAL:
                            rightSpell.GetComponent<Image>().sprite = manaHealSpellSprite;
                            break;
                        default:
                            rightSpell.GetComponent<Image>().sprite = noSpellSprite;
                            break;
                    }
                } else if (chosenHand == Hand.LEFT) {
                    switch (player.GetComponent<PlayerBehaviour>().GetCurrentLeftSpell()) {
                        case Spell.LIGHT:
                            leftSpell.GetComponent<Image>().sprite = lightSpellSprite;
                            break;
                        case Spell.FIREBALL:
                            leftSpell.GetComponent<Image>().sprite = fireballSpellSprite;
                            break;
                        case Spell.HEAL:
                            leftSpell.GetComponent<Image>().sprite = healSpellSprite;
                            break;
                        case Spell.MANA_HEAL:
                            leftSpell.GetComponent<Image>().sprite = manaHealSpellSprite;
                            break;
                        default:
                            leftSpell.GetComponent<Image>().sprite = noSpellSprite;
                            break;
                    }
                }
                break;
            case HandState.CASTING:
                hand.GetComponent<Image>().sprite = handCastingSprite;
                if (chosenHand == Hand.RIGHT) rightSpell.GetComponent<Image>().sprite = noSpellSprite;
                else if (chosenHand == Hand.LEFT) leftSpell.GetComponent<Image>().sprite = noSpellSprite;
                break;
            default:
                break;
        }
    }

    public void HideHands() {
        if (hands.transform.position.y > handsHiddenPosition.y) {
            hands.transform.position = new Vector3(
                hands.transform.position.x,
                hands.transform.position.y - 10f,
                hands.transform.position.z
            );
        }
    }

    public void ShowHands() {
        if (hands.transform.position.y < handsShownPosition.y) {
            hands.transform.position = new Vector3(
                hands.transform.position.x,
                hands.transform.position.y + 10f,
                hands.transform.position.z
            );
        }
    }

}
