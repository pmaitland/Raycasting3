using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

	public float turnSpeed = 4.0f;
	public float moveSpeed = 2.0f;
	public float gravity = 10.0f;
	public GameObject projectilePrefab;

	private float shootRecoveryTime = 1.0f;
	private bool rightShootOnCooldown = false;
	private bool leftShootOnCooldown = false;
	private float currentRightShootCooldown = 0.0f;
	private float currentLeftShootCooldown = 0.0f;

	private Camera camera;
	private CharacterController controller;

	private GameBehaviour gameController;
	private HandsBehaviour hands;
	private Health health;

	void Start() {
		camera = GetComponent<Camera>();
		controller = GetComponent<CharacterController>();

		gameController = GameObject.Find("Controller").GetComponent<GameBehaviour>();
		hands = GameObject.Find("Hands").GetComponent<HandsBehaviour>();
		health = GetComponent<Health>();

		Cursor.lockState = CursorLockMode.Locked;
	}
	
	void Update() {
		if (Input.GetKey("escape")) Application.Quit();

		if (health.GetCurrentHealth() <= 0) {
			Vector3 cameraPosition = camera.transform.position;
			Vector3 killerPosition = health.GetKiller().transform.position;
			Vector3 lookPosition = new Vector3(killerPosition.x, cameraPosition.y, killerPosition.z);
			camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, Quaternion.LookRotation(lookPosition - cameraPosition), Time.deltaTime);

			if (transform.position.y > 0.05) transform.position = new Vector3(transform.position.x, transform.position.y - 0.01f, transform.position.z);
			return;
		}

	    float y = Input.GetAxis("Mouse X") * turnSpeed;
	    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + y, 0);

	    float horizontal = Input.GetAxis("Horizontal") * moveSpeed;
		float vertical = Input.GetAxis("Vertical") * moveSpeed;
        controller.Move(transform.rotation * (Vector3.right * horizontal + Vector3.forward * vertical + Vector3.down * gravity) * Time.deltaTime);

		gameController.ActivateMinimapCell(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
		gameController.MovePlayerMinimapCell(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));

		if (rightShootOnCooldown) currentRightShootCooldown += Time.deltaTime;
		if (currentRightShootCooldown >= shootRecoveryTime) {
			rightShootOnCooldown = false;
			currentRightShootCooldown = 0.0f;
			hands.ChangeHandSprite(Hand.RIGHT, HandState.NORMAL);
		}
		if (Input.GetMouseButton(1) && !rightShootOnCooldown) {
			ShootRightProjectile();
			hands.ChangeHandSprite(Hand.RIGHT, HandState.CASTING);
			rightShootOnCooldown = true;
		}

		if (leftShootOnCooldown) currentLeftShootCooldown += Time.deltaTime;
		if (currentLeftShootCooldown >= shootRecoveryTime) {
			leftShootOnCooldown = false;
			currentLeftShootCooldown = 0.0f;
			hands.ChangeHandSprite(Hand.LEFT, HandState.NORMAL);
		}
		if (Input.GetMouseButton(0) && !leftShootOnCooldown) {
			ShootLeftProjectile();
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

	public void SetPosition(Vector3 position) {
		transform.position = position;
	}

	private void ShootRightProjectile() {
		GameObject projectile = ShootProjectile();
		projectile.transform.position += transform.right * 0.15f;
	}

	private void ShootLeftProjectile() {
		GameObject projectile = ShootProjectile();
		projectile.transform.position -= transform.right * 0.15f;
	}

	private GameObject ShootProjectile() {
		float vertical = Input.GetAxis("Vertical") * moveSpeed;
		Vector3 projectilePosition = transform.position;
		projectilePosition += transform.forward * (controller.radius + projectilePrefab.GetComponent<SphereCollider>().radius);
		if (vertical < 0) projectilePosition += transform.forward * 0.1f;
		projectilePosition -= transform.up * 0.25f;
		GameObject projectile = Instantiate(projectilePrefab, projectilePosition, transform.rotation);
		projectile.GetComponent<ProjectileBehaviour>().SetCreator(gameObject);
		return projectile;
	}

}
