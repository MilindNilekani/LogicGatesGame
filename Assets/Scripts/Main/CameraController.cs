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
		/*float deltaX = Input.GetAxis("Mouse X") * ConstantHandler.Instance.MouseSensitivityX;
		float deltaY = Input.GetAxis("Mouse Y") * ConstantHandler.Instance.MouseSensitivityY;
		if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
		{
			Strafe(deltaX);
			ChangeHeight(deltaY);
		}
		else
		{
			if (Input.GetMouseButton(0))
			{
				MoveForwards(deltaY);
				ChangeHeading(deltaX);
			}
			else if (Input.GetMouseButton(1))
			{
				ChangeHeading(deltaX);
				ChangePitch(-deltaY);
			}
		}*/
	}

	private void MoveForwards(float aVal)
	{
		Vector3 fwd = transform.forward;
		fwd.y = 0;
		fwd.Normalize();
		transform.position += aVal * fwd;
	}

	private void Strafe(float aVal)
	{
		transform.position += aVal * transform.right;
	}

	private void ChangeHeight(float aVal)
	{
		transform.position += aVal * Vector3.up;
	}

	private void ChangeHeading(float aVal)
	{
		mHdg += aVal;
		WrapAngle(ref mHdg);
		transform.localEulerAngles = new Vector3(mPitch, mHdg, 0);
		Camera.main.transform.rotation = Quaternion.Euler (mPitch, mHdg, 0);
	}

	private void ChangePitch(float aVal)
	{
		mPitch += aVal;
		WrapAngle(ref mPitch);
		transform.localEulerAngles = new Vector3(mPitch, mHdg, 0);
		Camera.main.transform.rotation = Quaternion.Euler (mPitch, mHdg, 0);
	}

	private static void WrapAngle(ref float angle)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
	}
}
