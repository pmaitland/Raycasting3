using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public float turnSpeed = 4.0f;
	public float moveSpeed = 2.0f;

	void Start () 
	{
		Cursor.lockState = CursorLockMode.Locked;
	}
	
	void Update ()
	{
	    MouseAiming();
	    KeyboardMovement();
	}
	
	void MouseAiming ()
	{
	    float y = Input.GetAxis("Mouse X") * turnSpeed;
	    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + y, 0);
	}
	
	void KeyboardMovement ()
	{
	    Vector3 dir = new Vector3(0, 0, 0);
	    dir.x = Input.GetAxis("Horizontal");
	    dir.z = Input.GetAxis("Vertical");
	    transform.Translate(dir * moveSpeed * Time.deltaTime);
	}
}
