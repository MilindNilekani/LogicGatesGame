using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomTools;

public class ConstantHandler : Singleton<ConstantHandler> {
	protected ConstantHandler()
	{
	}
	//-------------------Camera Controller-------------------
	[SerializeField]
	private float _mouseSensitivityX=20F;
	public float MouseSensitivityX
	{
		get { return _mouseSensitivityX; }
	}
	[SerializeField]
	private float _mouseSensitivityY=20F;
	public float MouseSensitivityY
	{
		get { return _mouseSensitivityY; }
	}
	[SerializeField]
	private float _moveSpeed=8F;
	public float MoveSpeed
	{
		get { return _moveSpeed; }
	}
	//--------------------------------------------------------

	//-------------------Generate 3D Grid---------------------
	[SerializeField]
	private int _gridLength=5;
	public int GridLength
	{
		get { return _gridLength; }
	}
	[SerializeField]
	private int _gridWidth=5;
	public int GridWidth
	{
		get { return _gridWidth; }
	}
	[SerializeField]
	private int _gridHeight=5;
	public int GridHeight
	{
		get { return _gridHeight; }
	}
	//---------------------------------------------------------

	//-------------------UI Handler----------------------------
	private bool _isUIdragged;
	public bool ComponentDragged
	{
		set { _isUIdragged = value; }
		get { return _isUIdragged; }
	}
	private bool _componentAdded;
	public bool ComponentAdded
	{
		set { _componentAdded = value; }
		get { return _componentAdded; }
	}
	private IVector3 _positionComponentAdded;
	public IVector3 PositionComponentAdded
	{
		set { _positionComponentAdded = value; }
		get { return _positionComponentAdded; }
	}
}
