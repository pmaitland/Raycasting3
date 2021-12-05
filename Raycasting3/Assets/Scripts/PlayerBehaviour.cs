using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
	private CharacterController controller;
    public float turnSpeed = 4.0f;
	public float moveSpeed = 2.0f;
	public float gravity = 10.0f;

	void Start () 
	{
		controller = GetComponent<CharacterController>();
		Cursor.lockState = CursorLockMode.Locked;
	}
	
	void Update ()
	{
	    float y = Input.GetAxis("Mouse X") * turnSpeed;
	    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + y, 0);

	    float horizontal = Input.GetAxis("Horizontal") * moveSpeed;
		float vertical = Input.GetAxis("Vertical") * moveSpeed;
        controller.Move(transform.rotation * (Vector3.right * horizontal + Vector3.forward * vertical + Vector3.down * gravity) * Time.deltaTime);
	}
}
