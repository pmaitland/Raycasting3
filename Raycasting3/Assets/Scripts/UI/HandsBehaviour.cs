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

    private PlayerBehaviour playerBehaviour;
    private GameBehaviour gameBehaviour;

    public Sprite handSprite;
    public Sprite handPreparedSprite;
    public Sprite handCastingSprite;
    public Sprite handPunchingSprite;

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

        playerBehaviour = GameObject.Find("Player").GetComponent<PlayerBehaviour>();
        gameBehaviour = GameObject.Find("Controller").GetComponent<GameBehaviour>();
    }

    void Update() {
        LightingType? lightingType = gameBehaviour.GetMazeCell(playerBehaviour.transform.position.x, playerBehaviour.transform.position.z)?.GetCurrentLightingLower();
        rightHand.GetComponent<Image>().color = Lighting.GetColor(lightingType);
        leftHand.GetComponent<Image>().color = Lighting.GetColor(lightingType);
    }

    public void ChangeHandSprite(Hand hand, HandState state) {
        if (hand == Hand.RIGHT) {
            rightHand.GetComponent<Image>().sprite = GetHandSprite(state);
            if (state == HandState.PREPARED)
                rightSpell.GetComponent<Image>().sprite = GetSpellSprite(playerBehaviour.GetCurrentRightSpell());
            else
                rightSpell.GetComponent<Image>().sprite = noSpellSprite;
        } else if (hand == Hand.LEFT) {
            leftHand.GetComponent<Image>().sprite = GetHandSprite(state);
            if (state == HandState.PREPARED)
                leftSpell.GetComponent<Image>().sprite = GetSpellSprite(playerBehaviour.GetCurrentLeftSpell());
            else
                leftSpell.GetComponent<Image>().sprite = noSpellSprite;
        }
    }

    public Sprite GetHandSprite(HandState state) {
        switch (state) {
            case HandState.PREPARED:
                return handPreparedSprite;
            case HandState.CASTING:
                return handCastingSprite;
            case HandState.PUNCHING:
                return handPunchingSprite;
            default:
                return handSprite;
        }
    }

    public Sprite GetSpellSprite(Spell spell) {
        switch (spell) {
            case Spell.LIGHT:
                return lightSpellSprite;
            case Spell.FIREBALL:
                return fireballSpellSprite;
            case Spell.HEAL:
                return healSpellSprite;
            case Spell.MANA_HEAL:
                return manaHealSpellSprite;
            default:
                return noSpellSprite;
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
