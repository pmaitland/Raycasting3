using UnityEngine;
using UnityEngine.UI;

public class HandsBehaviour : MonoBehaviour
{

    private GameObject _hands;
    private GameObject _rightHand;
    private GameObject _rightSpell;
    private GameObject _leftHand;
    private GameObject _leftSpell;

    private Vector3 _handsShownPosition;
    private Vector3 _handsHiddenPosition;

    private PlayerBehaviour _playerBehaviour;
    private GameBehaviour _gameBehaviour;

    public Sprite HandSprite;
    public Sprite HandPreparedSprite;
    public Sprite HandCastingSprite;
    public Sprite HandPunchingSprite;

    public Sprite NoSpellSprite;
    public Sprite LightSpellSprite;
    public Sprite FireballSpellSprite;
    public Sprite HealSpellSprite;
    public Sprite ManaHealSpellSprite;

    void Start()
    {
        _hands = GameObject.Find("Hands");
        _rightHand = GameObject.Find("Right Hand");
        _rightSpell = GameObject.Find("Right Spell");
        _leftHand = GameObject.Find("Left Hand");
        _leftSpell = GameObject.Find("Left Spell");

        _handsShownPosition = _hands.transform.position;
        _handsHiddenPosition = new Vector3(
            _handsShownPosition.x,
            -_handsShownPosition.y,
            _handsShownPosition.z
        );

        _playerBehaviour = GameObject.Find("Player").GetComponent<PlayerBehaviour>();
        _gameBehaviour = GameObject.Find("Controller").GetComponent<GameBehaviour>();
    }

    void Update()
    {
        Lighting.Type? lightingType = _gameBehaviour.GetMazeCell(_playerBehaviour.transform.position.x, _playerBehaviour.transform.position.z)?.GetCurrentLightingLower();
        _rightHand.GetComponent<Image>().color = Lighting.GetColor(lightingType);
        _leftHand.GetComponent<Image>().color = Lighting.GetColor(lightingType);
    }

    public void ChangeHandSprite(Hand hand, HandState state)
    {
        if (hand == Hand.RIGHT)
        {
            _rightHand.GetComponent<Image>().sprite = GetHandSprite(state);
            _rightSpell.GetComponent<Image>().sprite = state == HandState.PREPARED ? GetSpellSprite(_playerBehaviour.CurrentRightSpell) : NoSpellSprite;
        }
        else if (hand == Hand.LEFT)
        {
            _leftHand.GetComponent<Image>().sprite = GetHandSprite(state);
            _leftSpell.GetComponent<Image>().sprite = state == HandState.PREPARED ? GetSpellSprite(_playerBehaviour.CurrentLeftSpell) : NoSpellSprite;
        }
    }

    public Sprite GetHandSprite(HandState state)
    {
        return state switch
        {
            HandState.PREPARED => HandPreparedSprite,
            HandState.CASTING => HandCastingSprite,
            HandState.PUNCHING => HandPunchingSprite,
            _ => HandSprite,
        };
    }

    public Sprite GetSpellSprite(Spell spell)
    {
        return spell switch
        {
            Spell.LIGHT => LightSpellSprite,
            Spell.FIREBALL => FireballSpellSprite,
            Spell.HEAL => HealSpellSprite,
            Spell.MANA_HEAL => ManaHealSpellSprite,
            _ => NoSpellSprite,
        };
    }

    public void HideHands()
    {
        if (_hands.transform.position.y > _handsHiddenPosition.y)
        {
            _hands.transform.position = new Vector3(
                _hands.transform.position.x,
                _hands.transform.position.y - 10f,
                _hands.transform.position.z
            );
        }
    }

    public void ShowHands()
    {
        if (_hands.transform.position.y < _handsShownPosition.y)
        {
            _hands.transform.position = new Vector3(
                _hands.transform.position.x,
                _hands.transform.position.y + 10f,
                _hands.transform.position.z
            );
        }
    }

}
