using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

	public float turnSpeed = 4.0f;
	public float moveSpeed = 2.0f;
	public float gravity = 10.0f;
	public GameObject fireballPrefab;

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

	private Spell currentLeftSpell = Spell.NONE;
	private Spell currentRightSpell = Spell.NONE;

	private LightingType lighting = LightingType.DARKNESS;

	void Start() {
		playerCamera = GetComponent<Camera>();
		controller = GetComponent<CharacterController>();

		gameController = GameObject.Find("Controller").GetComponent<GameBehaviour>();
		hands = GameObject.Find("Hands").GetComponent<HandsBehaviour>();
		health = GetComponent<Health>();

		Cursor.lockState = CursorLockMode.Locked;
	}
	
	void Update() {
		if (Input.GetKey("escape")) Application.Quit();

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
		bool switchedRightSpell = false;
		if (Input.GetKey("1")) {
			if (Input.GetMouseButton(0)) {
				currentLeftSpell = Spell.NONE;
				if (!leftShootOnCooldown) hands.ChangeHandSprite(Hand.LEFT, HandState.NORMAL);
				switchedLeftSpell = true;
			}
			if (Input.GetMouseButton(1)) {
				currentRightSpell = Spell.NONE;
				if (!rightShootOnCooldown) hands.ChangeHandSprite(Hand.RIGHT, HandState.NORMAL);
				switchedRightSpell = true;
			}
		} else if (Input.GetKey("2")) {
			if (Input.GetMouseButton(0)) {
				currentLeftSpell = Spell.LIGHT;
				if (!leftShootOnCooldown) hands.ChangeHandSprite(Hand.LEFT, HandState.PREPARED);
				switchedLeftSpell = true;
			}
			if (Input.GetMouseButton(1)) {
				currentRightSpell = Spell.LIGHT;
				if (!rightShootOnCooldown) hands.ChangeHandSprite(Hand.RIGHT, HandState.PREPARED);
				switchedRightSpell = true;
			}
		} else if (Input.GetKey("3")) {
			if (Input.GetMouseButton(0)) {
				currentLeftSpell = Spell.FIREBALL;
				if (!leftShootOnCooldown) hands.ChangeHandSprite(Hand.LEFT, HandState.PREPARED);
				switchedLeftSpell = true;
			}
			if (Input.GetMouseButton(1)) {
				currentRightSpell = Spell.FIREBALL;
				if (!rightShootOnCooldown) hands.ChangeHandSprite(Hand.RIGHT, HandState.PREPARED);
				switchedRightSpell = true;
			}
		}

		if (currentLeftSpell == Spell.LIGHT && currentRightSpell == Spell.LIGHT) {
			lighting = LightingType.LIGHT_SPELL_0;
		} else if (currentLeftSpell == Spell.LIGHT || currentRightSpell == Spell.LIGHT){
			lighting = LightingType.LIGHT_SPELL_1;
		} else if (currentLeftSpell == Spell.FIREBALL && currentRightSpell == Spell.FIREBALL) {
			lighting = LightingType.FIREBALL_SPELL_0;
		} else if (currentLeftSpell == Spell.FIREBALL || currentRightSpell == Spell.FIREBALL){
			lighting = LightingType.FIREBALL_SPELL_1;
		} else {
			lighting = LightingType.DARKNESS;
		}

		if (rightShootOnCooldown) currentRightShootCooldown += Time.deltaTime;
		if (currentRightShootCooldown >= shootRecoveryTime) {
			rightShootOnCooldown = false;
			currentRightShootCooldown = 0.0f;
			if (currentRightSpell != Spell.NONE) hands.ChangeHandSprite(Hand.RIGHT, HandState.PREPARED);
			else hands.ChangeHandSprite(Hand.RIGHT, HandState.NORMAL);
		}
		if (currentRightSpell == Spell.FIREBALL && Input.GetMouseButton(1) && !switchedRightSpell && !rightShootOnCooldown) {
			ShootRightFireball();
			hands.ChangeHandSprite(Hand.RIGHT, HandState.CASTING);
			rightShootOnCooldown = true;
		}

		if (leftShootOnCooldown) currentLeftShootCooldown += Time.deltaTime;
		if (currentLeftShootCooldown >= shootRecoveryTime) {
			leftShootOnCooldown = false;
			currentLeftShootCooldown = 0.0f;
			if (currentLeftSpell != Spell.NONE) hands.ChangeHandSprite(Hand.LEFT, HandState.PREPARED);
			else hands.ChangeHandSprite(Hand.LEFT, HandState.NORMAL);
		}
		if (currentLeftSpell == Spell.FIREBALL && Input.GetMouseButton(0) && !switchedLeftSpell && !leftShootOnCooldown) {
			ShootLeftFireball();
			hands.ChangeHandSprite(Hand.LEFT, HandState.CASTING);
			leftShootOnCooldown = true;
		}
	}

	void OnControllerColliderHit(ControllerColliderHit hit)	{
		if (hit.transform.parent != null) {
			if (hit.transform.parent.name.Contains("Hidden Door")) {
				if (Input.GetKey("e")) hit.transform.parent.GetComponent<HiddenDoorBehaviour>().Open();
			} else if (hit.transform.name.Contains("Face 1")) {
				if (Input.GetKey("e")) hit.transform.parent.parent.GetComponent<DoorBehaviour>().Open(true);
			} else if (hit.transform.name.Contains("Face 2")) {
				if (Input.GetKey("e")) hit.transform.parent.parent.GetComponent<DoorBehaviour>().Open(false);
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
