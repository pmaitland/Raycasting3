using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
	public float turnSpeed = 4.0f;
	public float moveSpeed = 2.0f;
	public float gravity = 10.0f;
	public GameObject projectilePrefab;

	private CharacterController controller;

	private GameBehaviour gameController;

	void Start() 
	{
		controller = GetComponent<CharacterController>();

		gameController = GameObject.Find("Controller").GetComponent<GameBehaviour>();

		Cursor.lockState = CursorLockMode.Locked;
	}
	
	void Update()
	{
	    float y = Input.GetAxis("Mouse X") * turnSpeed;
	    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + y, 0);

	    float horizontal = Input.GetAxis("Horizontal") * moveSpeed;
		float vertical = Input.GetAxis("Vertical") * moveSpeed;
        controller.Move(transform.rotation * (Vector3.right * horizontal + Vector3.forward * vertical + Vector3.down * gravity) * Time.deltaTime);

		gameController.ActivateMinimapCell(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
		gameController.MovePlayerMinimapCell(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));

		if (Input.GetMouseButtonDown(0)) {
			Vector3 projectilePosition = transform.position;
			projectilePosition += transform.forward * (controller.radius + projectilePrefab.GetComponent<SphereCollider>().radius);
			if (vertical < 0) projectilePosition += transform.forward * 0.1f;
			projectilePosition -= transform.up * 0.25f;
			Instantiate(projectilePrefab, projectilePosition, transform.rotation);
		}

		if (Input.GetKey("escape")) {
            Application.Quit();
        }
	}

	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.transform.parent.name.Contains("Hidden Door")) {
			if (Input.GetKey("e")) hit.transform.parent.GetComponent<HiddenDoorBehaviour>().Open();
		} else if (hit.transform.name.Contains("Face 1")) {
			if (Input.GetKey("e")) hit.transform.parent.parent.GetComponent<DoorBehaviour>().Open(true);
		} else if (hit.transform.name.Contains("Face 2")) {
			if (Input.GetKey("e")) hit.transform.parent.parent.GetComponent<DoorBehaviour>().Open(false);
		}
	}

	public void SetPosition(Vector3 position)
	{
		transform.position = position;
	}
}
