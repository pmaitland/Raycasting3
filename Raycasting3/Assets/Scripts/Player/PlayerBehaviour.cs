using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{

    public GameObject FireballPrefab;
    public GameObject BallOfLightPrefab;

    public float TurnSpeed = 4.0f;
    public float MoveSpeed = 2.0f;
    public float Gravity = 10.0f;

    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }

    private readonly float _castRecoveryTime = 1.0f;
    private bool _rightCastOnCooldown = false;
    private bool _leftCastOnCooldown = false;
    private float _currentRightCastCooldown = 0.0f;
    private float _currentLeftCastCooldown = 0.0f;

    private CharacterController _controller;

    private GameObject _rightFist;
    private GameObject _leftFist;

    private GameBehaviour _gameController;
    private HandsBehaviour _hands;
    private Health _health;
    private Flash _flash;

    private const int MAX_MAX_MANA = 20;
    public int MaxMana { get; private set; } = 5;
    public int CurrentMana { get; private set; } = 5;

    private readonly Spell[] _availableLeftHandSpells = { Spell.NONE, Spell.LIGHT, Spell.FIREBALL, Spell.HEAL, Spell.MANA_HEAL };
    private int _currentLeftSpellIndex = 0;
    public Spell CurrentLeftSpell { get; private set; } = Spell.NONE;

    private readonly Spell[] _availableRightHandSpells = { Spell.NONE, Spell.LIGHT, Spell.FIREBALL, Spell.HEAL, Spell.MANA_HEAL };
    private int _currentRightSpellIndex = 0;
    public Spell CurrentRightSpell { get; private set; } = Spell.NONE;

    public Lighting.Type CurrentLighting { get; private set; } = Lighting.Type.DARKNESS;

    private bool _interactingWithLadder = false;

    private readonly string _interactKey = "space";
    private readonly string _changeLeftHandKey = "q";
    private readonly string _changeRightHandKey = "e";

    void Start()
    {
        _controller = GetComponent<CharacterController>();

        _rightFist = GameObject.Find("Right Fist");
        _leftFist = GameObject.Find("Left Fist");

        _gameController = GameObject.Find("Controller").GetComponent<GameBehaviour>();
        _hands = GameObject.Find("Hands").GetComponent<HandsBehaviour>();
        _health = GetComponent<Health>();
        _flash = GameObject.Find("Flash").GetComponent<Flash>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (_gameController.Paused) { return; }

        if (_interactingWithLadder)
        {
            _gameController.GenerateMaze();
            _interactingWithLadder = false;
        }

        if (_health.CurrentHealth <= 0)
        {
            Vector3 killerPosition = _health.Killer.transform.position;
            Vector3 lookPosition = new(killerPosition.x, transform.position.y, killerPosition.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPosition - transform.position), Time.deltaTime);

            if (transform.position.y > 0.05) transform.position = new Vector3(transform.position.x, transform.position.y - 0.01f, transform.position.z);

            CurrentLighting = Lighting.Type.DARKNESS;
            _hands.ChangeHandSprite(Hand.LEFT, HandState.NORMAL);
            _hands.ChangeHandSprite(Hand.RIGHT, HandState.NORMAL);
            _hands.HideHands();

            Horizontal = 0;
            Vertical = 0;

            return;
        }

        float y = Input.GetAxis("Mouse X") * TurnSpeed;
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + y, 0);

        Horizontal = Input.GetAxis("Horizontal") * MoveSpeed;
        Vertical = Input.GetAxis("Vertical") * MoveSpeed;
        _controller.Move(transform.rotation * (Vector3.right * Horizontal + Vector3.forward * Vertical + Vector3.down * Gravity) * Time.deltaTime);

        _gameController.ActivateMinimapCell(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        _gameController.MovePlayerMinimapCell(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));

        bool switchedLeftSpell = false;
        if (Input.GetKeyDown(_changeLeftHandKey))
        {
            _currentLeftSpellIndex = (_currentLeftSpellIndex + 1) % _availableLeftHandSpells.Length;
            CurrentLeftSpell = _availableLeftHandSpells[_currentLeftSpellIndex];
            if (!_leftCastOnCooldown)
            {
                switch (CurrentLeftSpell)
                {
                    case Spell.LIGHT:
                    case Spell.FIREBALL:
                    case Spell.HEAL:
                    case Spell.MANA_HEAL:
                        _hands.ChangeHandSprite(Hand.LEFT, HandState.PREPARED);
                        break;
                    case Spell.NONE:
                    default:
                        _hands.ChangeHandSprite(Hand.LEFT, HandState.NORMAL);
                        break;
                }
            }
            switchedLeftSpell = true;
        }

        bool switchedRightSpell = false;
        if (Input.GetKeyDown(_changeRightHandKey))
        {
            _currentRightSpellIndex = (_currentRightSpellIndex + 1) % _availableRightHandSpells.Length;
            CurrentRightSpell = _availableRightHandSpells[_currentRightSpellIndex];
            if (!_rightCastOnCooldown)
            {
                switch (CurrentRightSpell)
                {
                    case Spell.LIGHT:
                    case Spell.FIREBALL:
                    case Spell.HEAL:
                    case Spell.MANA_HEAL:
                        _hands.ChangeHandSprite(Hand.RIGHT, HandState.PREPARED);
                        break;
                    case Spell.NONE:
                    default:
                        _hands.ChangeHandSprite(Hand.RIGHT, HandState.NORMAL);
                        break;
                }
            }
            switchedRightSpell = true;
        }

        if (CurrentLeftSpell == Spell.LIGHT && CurrentRightSpell == Spell.LIGHT)
        {
            CurrentLighting = Lighting.Type.LIGHT_SPELL_2;
        }
        else if (CurrentLeftSpell == Spell.LIGHT || CurrentRightSpell == Spell.LIGHT)
        {
            CurrentLighting = Lighting.Type.LIGHT_SPELL_3;
        }
        else if (CurrentLeftSpell == Spell.FIREBALL && CurrentRightSpell == Spell.FIREBALL)
        {
            CurrentLighting = Lighting.Type.FIREBALL_SPELL_0;
        }
        else if (CurrentLeftSpell == Spell.FIREBALL || CurrentRightSpell == Spell.FIREBALL)
        {
            CurrentLighting = Lighting.Type.FIREBALL_SPELL_1;
        }
        else
        {
            CurrentLighting = Lighting.Type.DARKNESS;
        }

        if (_leftCastOnCooldown) _currentLeftCastCooldown += Time.deltaTime;
        if (_currentLeftCastCooldown >= _castRecoveryTime)
        {
            _leftCastOnCooldown = false;
            _currentLeftCastCooldown = 0.0f;
            _leftFist.GetComponent<BoxCollider>().enabled = false;
            _leftFist.GetComponent<FistBehaviour>().ResetObjectsHit();
            if (CurrentLeftSpell != Spell.NONE) _hands.ChangeHandSprite(Hand.LEFT, HandState.PREPARED);
            else _hands.ChangeHandSprite(Hand.LEFT, HandState.NORMAL);
        }

        if (_rightCastOnCooldown) _currentRightCastCooldown += Time.deltaTime;
        if (_currentRightCastCooldown >= _castRecoveryTime)
        {
            _rightCastOnCooldown = false;
            _currentRightCastCooldown = 0.0f;
            _rightFist.GetComponent<BoxCollider>().enabled = false;
            _rightFist.GetComponent<FistBehaviour>().ResetObjectsHit();
            if (CurrentRightSpell != Spell.NONE) _hands.ChangeHandSprite(Hand.RIGHT, HandState.PREPARED);
            else _hands.ChangeHandSprite(Hand.RIGHT, HandState.NORMAL);
        }

        if (Input.GetMouseButton(0) && !switchedLeftSpell && !_leftCastOnCooldown)
        {
            if (CurrentMana > 0)
            {
                if (CurrentLeftSpell == Spell.LIGHT)
                {
                    GameObject ballOfLight = Instantiate(BallOfLightPrefab, transform.position, transform.rotation);
                    _gameController.AddLowerLightSource(ballOfLight, Lighting.Type.LIGHT_SPELL_0);
                    _hands.ChangeHandSprite(Hand.LEFT, HandState.CASTING);
                    _leftCastOnCooldown = true;
                    CurrentMana -= 1;
                }
                else if (CurrentLeftSpell == Spell.FIREBALL)
                {
                    ShootLeftFireball();
                    _hands.ChangeHandSprite(Hand.LEFT, HandState.CASTING);
                    _leftCastOnCooldown = true;
                    CurrentMana -= 1;
                }
                else if (CurrentLeftSpell == Spell.HEAL && _health.CurrentHealth < _health.MaxHealth)
                {
                    _health.IncreaseHealth(1);
                    _hands.ChangeHandSprite(Hand.LEFT, HandState.CASTING);
                    _leftCastOnCooldown = true;
                    CurrentMana -= 1;
                    _flash.Health();
                }
            }

            if (CurrentLeftSpell == Spell.NONE)
            {
                _hands.ChangeHandSprite(Hand.LEFT, HandState.PUNCHING);
                _leftFist.GetComponent<BoxCollider>().enabled = true;
                _leftCastOnCooldown = true;
            }
            else if (CurrentLeftSpell == Spell.MANA_HEAL && CurrentMana < MaxMana)
            {
                CurrentMana += 1;
                _hands.ChangeHandSprite(Hand.LEFT, HandState.CASTING);
                _leftCastOnCooldown = true;
                _health.ReduceHealth(gameObject, 1);
            }
        }

        if (Input.GetMouseButton(1) && !switchedRightSpell && !_rightCastOnCooldown)
        {
            if (CurrentMana > 0)
            {
                if (CurrentRightSpell == Spell.LIGHT)
                {
                    GameObject ballOfLight = Instantiate(BallOfLightPrefab, transform.position, transform.rotation);
                    _gameController.AddLowerLightSource(ballOfLight, Lighting.Type.LIGHT_SPELL_0);
                    _hands.ChangeHandSprite(Hand.RIGHT, HandState.CASTING);
                    _rightCastOnCooldown = true;
                    CurrentMana -= 1;
                }
                else if (CurrentRightSpell == Spell.FIREBALL)
                {
                    ShootRightFireball();
                    _hands.ChangeHandSprite(Hand.RIGHT, HandState.CASTING);
                    _rightCastOnCooldown = true;
                    CurrentMana -= 1;
                }
                else if (CurrentRightSpell == Spell.HEAL && _health.CurrentHealth < _health.MaxHealth)
                {
                    _health.IncreaseHealth(1);
                    _hands.ChangeHandSprite(Hand.RIGHT, HandState.CASTING);
                    _rightCastOnCooldown = true;
                    CurrentMana -= 1;
                    _flash.Health();
                }
            }

            if (CurrentRightSpell == Spell.NONE)
            {
                _hands.ChangeHandSprite(Hand.RIGHT, HandState.PUNCHING);
                _rightFist.GetComponent<BoxCollider>().enabled = true;
                _rightCastOnCooldown = true;
            }
            else if (CurrentRightSpell == Spell.MANA_HEAL && CurrentMana < MaxMana)
            {
                CurrentMana += 1;
                _hands.ChangeHandSprite(Hand.RIGHT, HandState.CASTING);
                _rightCastOnCooldown = true;
                _health.ReduceHealth(gameObject, 1);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<HealthPickup>() != null)
        {
            if (_health.CurrentHealth < _health.MaxHealth)
            {
                _health.IncreaseHealth(other.gameObject.GetComponent<HealthPickup>().HealingAmount);
                Destroy(other.gameObject);
                _flash.Health();
            }
        }
        else if (other.gameObject.GetComponent<HeartContainer>() != null)
        {
            if (_health.MaxHealth < _health.MAX_MAX_HEALTH)
            {
                _health.IncreaseMaxHealth(other.gameObject.GetComponent<HeartContainer>().MaxHealthIncreaseAmount);
                Destroy(other.gameObject);
                _flash.Health();
            }
        }
        else if (other.gameObject.GetComponent<ManaPickup>() != null)
        {
            if (CurrentMana < MaxMana)
            {
                CurrentMana += other.gameObject.GetComponent<ManaPickup>().ManaAmount;
                if (CurrentMana > MaxMana) CurrentMana = MaxMana;
                Destroy(other.gameObject);
                _flash.Mana();
            }
        }
        else if (other.gameObject.GetComponent<ManaContainer>() != null)
        {
            if (MaxMana < MAX_MAX_MANA)
            {
                MaxMana += other.gameObject.GetComponent<ManaContainer>().MaxManaIncreaseAmount;
                if (MaxMana > MAX_MAX_MANA) MaxMana = MAX_MAX_MANA;
                CurrentMana += other.gameObject.GetComponent<ManaContainer>().MaxManaIncreaseAmount;
                if (CurrentMana > MaxMana) CurrentMana = MaxMana;
                Destroy(other.gameObject);
                _flash.Mana();
            }
        }

        if (!Input.GetKey(_interactKey)) return;

        if (other.transform.name.Contains("Ladder"))
        {
            _interactingWithLadder = true;
        }
        else if (other.transform.parent != null)
        {
            if (other.transform.parent.name.Contains("Hidden Door"))
            {
                other.transform.parent.GetComponent<HiddenDoorBehaviour>().Open();
            }
            else if (other.transform.parent.parent != null)
            {
                if (other.transform.name.Contains("Face 1"))
                {
                    other.transform.parent.parent.GetComponent<DoorBehaviour>().Open(true);
                }
                else if (other.transform.name.Contains("Face 2"))
                {
                    other.transform.parent.parent.GetComponent<DoorBehaviour>().Open(false);
                }
                else if (other.transform.parent.parent.parent != null)
                {
                    if (other.transform.parent.parent.parent.name.Contains("Chest"))
                    {
                        other.transform.parent.parent.parent.GetComponent<ChestBehaviour>().Open();
                    }
                }
            }
        }
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    private void ShootRightFireball()
    {
        GameObject fireball = ShootFireball();
        fireball.transform.position += transform.right * 0.15f;
    }

    private void ShootLeftFireball()
    {
        GameObject fireball = ShootFireball();
        fireball.transform.position -= transform.right * 0.15f;
    }

    private GameObject ShootFireball()
    {
        float vertical = Input.GetAxis("Vertical") * MoveSpeed;
        Vector3 fireballPosition = transform.position;
        fireballPosition += transform.forward * (_controller.radius + FireballPrefab.GetComponent<SphereCollider>().radius);
        if (vertical < 0) fireballPosition += transform.forward * 0.1f;
        fireballPosition -= transform.up * 0.25f;
        GameObject fireball = Instantiate(FireballPrefab, fireballPosition, transform.rotation);
        fireball.GetComponent<ProjectileBehaviour>().Creator = gameObject;
        _gameController.AddTemporaryLowerLightSource(fireball, Lighting.Type.FIREBALL_SPELL_0);
        return fireball;
    }

    public void FlashDamage()
    {
        _flash.Damage();
    }

}
