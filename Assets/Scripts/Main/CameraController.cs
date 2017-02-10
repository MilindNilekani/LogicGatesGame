using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	private float mHdg = 0F;
	private float mPitch = 0F;

	Vector3 newPos= new Vector3(2,2,2);
	bool flag=false;
	
	// Update is called once per frame
	void Update()
	{
		RotateCamera ();
		MoveCamera ();
	}

	private void RotateCamera()
	{
		Vector3 origin = Camera.main.transform.eulerAngles;
		Vector3 destination = origin;

		//detect rotation amount if ALT is being held and the Right mouse button is down
		if (Input.GetMouseButton(1))
		{
			destination.x -= Input.GetAxis("Mouse Y") * ConstantHandler.Instance.MouseSensitivityX;
			destination.y += Input.GetAxis("Mouse X") * ConstantHandler.Instance.MouseSensitivityY;
		}

		//if a change in position is detected perform the necessary update
		if (destination != origin)
		{
			Camera.main.transform.eulerAngles = Vector3.MoveTowards(origin, destination, Time.deltaTime * ConstantHandler.Instance.MouseSensitivityX);
		}
	}

	private void MoveCamera()
	{
		float xpos = Input.mousePosition.x;
		float ypos = Input.mousePosition.y;
		Vector3 movement = new Vector3(0, 0, 0);

		//Move the GameObject
		if (Input.GetKey("a"))
		{
			movement.x -= ConstantHandler.Instance.MoveSpeed;
		}
		if (Input.GetKey("s"))
		{
			movement.z -= ConstantHandler.Instance.MoveSpeed;

		}
		if (Input.GetKey("d"))
		{
			movement.x += ConstantHandler.Instance.MoveSpeed;
		}
		if (Input.GetKey("w"))
		{

			movement.z += ConstantHandler.Instance.MoveSpeed;
		}

		movement = Camera.main.transform.TransformDirection(movement);
		movement.y = 0;
		//away from ground movement
		movement.y -= ConstantHandler.Instance.MoveSpeed * Input.GetAxis("Mouse ScrollWheel");

		//calculate desired camera position based on received input
		Vector3 origin = Camera.main.transform.position;
		Vector3 destination = origin;
		destination.x += movement.x;
		destination.y += movement.y;
		destination.z += movement.z;

		//if a change in position is detected perform the necessary update
		if (destination != origin)
		{
			Camera.main.transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * ConstantHandler.Instance.MoveSpeed);
		}
	}
}
