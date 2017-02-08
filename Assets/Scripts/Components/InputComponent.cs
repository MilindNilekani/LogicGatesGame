using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomTools;

public class InputComponent : VirtualComponent {
	private bool _logicInput;
	private GridHandler.ComponentDirection _direction;
	public GridHandler.ComponentDirection Direction
	{
		get { return _direction; }
	}
	private IVector3 _position;
	public IVector3 Position
	{
		get { return _position; }
	}
	public InputComponent(IVector3 pos, GridHandler.ComponentDirection dir, bool _logicArg)
	{
		_logicInput = _logicArg;
		_direction = dir;
		_position = pos;
	}
	public override void Compute(bool input, out bool output)
	{
		output = input;
		//Debug.Log ("Input wire at " + Position + "Value is " + output);
	}

	public override void Compute(out bool output)
	{
		output = true;
		Debug.Log ("If this statement appears, then everything is wrong with the world");
	}

	public override IVector3 MoveElectron()
	{
		IVector3 _moveDir = IVector3.zero;
		switch (Direction) {
		case GridHandler.ComponentDirection.LEFT:
			_moveDir = new IVector3 (-1, 0, 0);
			break;
		case GridHandler.ComponentDirection.RIGHT:
			_moveDir = new IVector3 (1, 0, 0);
			break;
		case GridHandler.ComponentDirection.FRONT:
			_moveDir = new IVector3 (0, 0, 1);
			break;
		case GridHandler.ComponentDirection.BACK:
			_moveDir = new IVector3 (0, 0, -1);
			break;
		case GridHandler.ComponentDirection.UP:
			_moveDir = new IVector3 (0, 1, 0);
			break;
		case GridHandler.ComponentDirection.DOWN:
			_moveDir = new IVector3 (0, -1, 0);
			break;
		default:
			break;
		}
		IVector3 electronPos = _position + _moveDir;
		if (GameHandler.instance._grid.isOnGrid (electronPos.x, electronPos.y, electronPos.z)) {
			//Debug.Log ("Input "+Position + "," + Direction + "=" + electronPos);
			return electronPos;
		}
		else {
			Debug.Log ("Not on board");
			return null;
		}	
	}
}
