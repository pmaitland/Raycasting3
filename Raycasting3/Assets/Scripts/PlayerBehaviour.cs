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
		string hitName = hit.transform.parent.name;
		if (hitName.Contains("Door")) {
			if (Input.GetKey("e")) hit.transform.parent.GetComponent<DoorBehaviour>().Open();
		} else if (hitName.Contains("Floor")) {
			gameController.ActivateMinimapCell((int) hit.transform.position.x, (int) hit.transform.position.z);
			gameController.MovePlayerMinimapCell((int) hit.transform.position.x, (int) hit.transform.position.z);
		}
	}

	public void SetPosition(Vector3 position)
	{
		transform.position = position;
	}
}
