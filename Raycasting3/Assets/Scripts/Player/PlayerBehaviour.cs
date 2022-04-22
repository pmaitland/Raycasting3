using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

	public GameObject fireballPrefab;
	public GameObject ballOfLightPrefab;

	public float turnSpeed = 4.0f;
	public float moveSpeed = 2.0f;
	public float gravity = 10.0f;

	private float shootRecoveryTime = 1.0f;
	private bool rightShootOnCooldown = false;
	private bool leftShootOnCooldown = false;
	private float currentRightShootCooldown = 0.0f;
	private float currentLeftShootCooldown = 0.0f;

	private Camera playerCamera;
	private CharacterController controller;

	private GameBehaviour gameController;
	private HandsBehaviour hands;
	private Health health;

	private const int MAX_MAX_MANA = 20;
    private int maxMana = 5;
    private int currentMana = 5;

	private Spell[] availableLeftHandSpells = { Spell.NONE, Spell.LIGHT, Spell.FIREBALL };
	private int currentLeftSpellIndex = 0;
	private Spell currentLeftSpell = Spell.NONE;

	private Spell[] availableRightHandSpells = { Spell.NONE, Spell.LIGHT, Spell.FIREBALL };
	private int currentRightSpellIndex = 0;
	private Spell currentRightSpell = Spell.NONE;

	private LightingType lighting = LightingType.DARKNESS;

	private string pauseKey = "escape";
	private string interactKey = "space";
	private string changeLeftHandKey = "q";
	private string changeRightHandKey = "e";

	void Start() {
		playerCamera = GetComponent<Camera>();
		controller = GetComponent<CharacterController>();

		gameController = GameObject.Find("Controller").GetComponent<GameBehaviour>();
		hands = GameObject.Find("Hands").GetComponent<HandsBehaviour>();
		health = GetComponent<Health>();

		Cursor.lockState = CursorLockMode.Locked;
	}
	
	void Update() {
		if (Input.GetKey(pauseKey)) Application.Quit();

		if (health.GetCurrentHealth() <= 0) {
			Vector3 cameraPosition = playerCamera.transform.position;
			Vector3 killerPosition = health.GetKiller().transform.position;
			Vector3 lookPosition = new Vector3(killerPosition.x, cameraPosition.y, killerPosition.z);
			playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, Quaternion.LookRotation(lookPosition - cameraPosition), Time.deltaTime);

			if (transform.position.y > 0.05) transform.position = new Vector3(transform.position.x, transform.position.y - 0.01f, transform.position.z);

			lighting = LightingType.DARKNESS;
			hands.ChangeHandSprite(Hand.LEFT, HandState.NORMAL);
			hands.ChangeHandSprite(Hand.RIGHT, HandState.NORMAL);
			hands.HideHands();
			return;
		}

	    float y = Input.GetAxis("Mouse X") * turnSpeed;
	    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + y, 0);

	    float horizontal = Input.GetAxis("Horizontal") * moveSpeed;
		float vertical = Input.GetAxis("Vertical") * moveSpeed;
        controller.Move(transform.rotation * (Vector3.right * horizontal + Vector3.forward * vertical + Vector3.down * gravity) * Time.deltaTime);

		gameController.ActivateMinimapCell(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
		gameController.MovePlayerMinimapCell(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));

		bool switchedLeftSpell = false;
		if (Input.GetKeyDown(changeLeftHandKey)) {
			currentLeftSpellIndex = (currentLeftSpellIndex + 1) % availableLeftHandSpells.Length;
			currentLeftSpell = availableLeftHandSpells[currentLeftSpellIndex];
			if (!leftShootOnCooldown) {
				switch (currentLeftSpell) {
					case Spell.LIGHT:
					case Spell.FIREBALL:
						hands.ChangeHandSprite(Hand.LEFT, HandState.PREPARED);
						break;
					case Spell.NONE:
					default:
						hands.ChangeHandSprite(Hand.LEFT, HandState.NORMAL);
						break;
				}
			}
			switchedLeftSpell = true;
		}

		bool switchedRightSpell = false;
		if (Input.GetKeyDown(changeRightHandKey)) {
			currentRightSpellIndex = (currentRightSpellIndex + 1) % availableRightHandSpells.Length;
			currentRightSpell = availableRightHandSpells[currentRightSpellIndex];
			if (!rightShootOnCooldown) {
				switch (currentRightSpell) {
					case Spell.LIGHT:
					case Spell.FIREBALL:
						hands.ChangeHandSprite(Hand.RIGHT, HandState.PREPARED);
						break;
					case Spell.NONE:
					default:
						hands.ChangeHandSprite(Hand.RIGHT, HandState.NORMAL);
						break;
				}
			}
			switchedRightSpell = true;
		}

		if (currentLeftSpell == Spell.LIGHT && currentRightSpell == Spell.LIGHT) {
			lighting = LightingType.LIGHT_SPELL_2;
		} else if (currentLeftSpell == Spell.LIGHT || currentRightSpell == Spell.LIGHT){
			lighting = LightingType.LIGHT_SPELL_3;
		} else if (currentLeftSpell == Spell.FIREBALL && currentRightSpell == Spell.FIREBALL) {
			lighting = LightingType.FIREBALL_SPELL_0;
		} else if (currentLeftSpell == Spell.FIREBALL || currentRightSpell == Spell.FIREBALL){
			lighting = LightingType.FIREBALL_SPELL_1;
		} else {
			lighting = LightingType.DARKNESS;
		}

		if (leftShootOnCooldown) currentLeftShootCooldown += Time.deltaTime;
		if (currentLeftShootCooldown >= shootRecoveryTime) {
			leftShootOnCooldown = false;
			currentLeftShootCooldown = 0.0f;
			if (currentLeftSpell != Spell.NONE) hands.ChangeHandSprite(Hand.LEFT, HandState.PREPARED);
			else hands.ChangeHandSprite(Hand.LEFT, HandState.NORMAL);
		}
		
		if (rightShootOnCooldown) currentRightShootCooldown += Time.deltaTime;
		if (currentRightShootCooldown >= shootRecoveryTime) {
			rightShootOnCooldown = false;
			currentRightShootCooldown = 0.0f;
			if (currentRightSpell != Spell.NONE) hands.ChangeHandSprite(Hand.RIGHT, HandState.PREPARED);
			else hands.ChangeHandSprite(Hand.RIGHT, HandState.NORMAL);
		}

		if (Input.GetMouseButton(0) && !switchedLeftSpell && !leftShootOnCooldown && currentMana >= 1) {
			if (currentLeftSpell == Spell.LIGHT) {
				GameObject ballOfLight = Instantiate(ballOfLightPrefab, transform.position, transform.rotation);
				gameController.AddLightSource(ballOfLight, LightingType.LIGHT_SPELL_0);
				hands.ChangeHandSprite(Hand.LEFT, HandState.CASTING);
				leftShootOnCooldown = true;
				currentMana -= 1;
			} else if (currentLeftSpell == Spell.FIREBALL) {
				ShootLeftFireball();
				hands.ChangeHandSprite(Hand.LEFT, HandState.CASTING);
				leftShootOnCooldown = true;
				currentMana -= 1;
			}
		}	

		if (Input.GetMouseButton(1) && !switchedRightSpell && !rightShootOnCooldown && currentMana >= 1) {
			if (currentRightSpell == Spell.LIGHT) {
				GameObject ballOfLight = Instantiate(ballOfLightPrefab, transform.position, transform.rotation);
				gameController.AddLightSource(ballOfLight, LightingType.LIGHT_SPELL_0);
				hands.ChangeHandSprite(Hand.RIGHT, HandState.CASTING);
				rightShootOnCooldown = true;
				currentMana -= 1;
			} else if (currentRightSpell == Spell.FIREBALL) {
				ShootRightFireball();
				hands.ChangeHandSprite(Hand.RIGHT, HandState.CASTING);
				rightShootOnCooldown = true;
				currentMana -= 1;
			}
		}
	}

	void OnTriggerStay(Collider other) {
		if (other.gameObject.GetComponent<HealthPickup>() != null) {
			if (health.GetCurrentHealth() < health.GetMaxHealth()) {
				health.IncreaseHealth(other.gameObject.GetComponent<HealthPickup>().GetHealingAmount());
				Destroy(other.gameObject);
			}
		} else if (other.gameObject.GetComponent<HeartContainer>() != null) {
			if (health.GetMaxHealth() < health.GetMaxMaxHealth()) {
				health.IncreaseMaxHealth(other.gameObject.GetComponent<HeartContainer>().GetMaxHealthIncreaseAmount());
				Destroy(other.gameObject);
			}
		} else if (other.gameObject.GetComponent<ManaPickup>() != null) {
			if (currentMana < maxMana) {
				currentMana += other.gameObject.GetComponent<ManaPickup>().GetManaAmount();
				if (currentMana > maxMana) currentMana = maxMana;
				Destroy(other.gameObject);
			}
		} else if (other.gameObject.GetComponent<ManaContainer>() != null) {
			if (maxMana < MAX_MAX_MANA) {
				maxMana += other.gameObject.GetComponent<ManaContainer>().GetMaxManaIncreaseAmount();
				if (maxMana > MAX_MAX_MANA) maxMana = MAX_MAX_MANA;
				currentMana += other.gameObject.GetComponent<ManaContainer>().GetMaxManaIncreaseAmount();
				if (currentMana > maxMana) currentMana = maxMana;
				Destroy(other.gameObject);
			}
		} 

		if (!Input.GetKey(interactKey)) return;

		if (other.transform.parent != null) {

			if (other.transform.parent.name.Contains("Hidden Door")) {
				other.transform.parent.GetComponent<HiddenDoorBehaviour>().Open();
			}

			else if (other.transform.parent.parent != null) {

				if (other.transform.name.Contains("Face 1")) {
					other.transform.parent.parent.GetComponent<DoorBehaviour>().Open(true);
				} else if (other.transform.name.Contains("Face 2")) {
					other.transform.parent.parent.GetComponent<DoorBehaviour>().Open(false);
				}

				else if (other.transform.parent.parent.parent != null) {
					if (other.transform.parent.parent.parent.name.Contains("Chest")) {
						other.transform.parent.parent.parent.GetComponent<ChestBehaviour>().Open();
					}
				}
			}
		}
	}

	public LightingType GetLighting() {
		return lighting;
	}

	public Spell GetCurrentLeftSpell() {
		return currentLeftSpell;
	}

	public Spell GetCurrentRightSpell() {
		return currentRightSpell;
	}

	public int GetCurrentMana() {
		return currentMana;
	}

	public int GetMaxMana() {
		return maxMana;
	}

	public void SetPosition(Vector3 position) {
		transform.position = position;
	}

	private void ShootRightFireball() {
		GameObject fireball = ShootFireball();
		fireball.transform.position += transform.right * 0.15f;
	}

	private void ShootLeftFireball() {
		GameObject fireball = ShootFireball();
		fireball.transform.position -= transform.right * 0.15f;
	}

	private GameObject ShootFireball() {
		float vertical = Input.GetAxis("Vertical") * moveSpeed;
		Vector3 fireballPosition = transform.position;
		fireballPosition += transform.forward * (controller.radius + fireballPrefab.GetComponent<SphereCollider>().radius);
		if (vertical < 0) fireballPosition += transform.forward * 0.1f;
		fireballPosition -= transform.up * 0.25f;
		GameObject fireball = Instantiate(fireballPrefab, fireballPosition, transform.rotation);
		fireball.GetComponent<ProjectileBehaviour>().SetCreator(gameObject);
		gameController.AddLightSource(fireball, LightingType.FIREBALL_SPELL_0);
		return fireball;
	}

}
