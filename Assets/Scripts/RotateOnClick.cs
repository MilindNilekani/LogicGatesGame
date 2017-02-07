using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomTools;
using CustomTools.ForwardEvents;

public class RotateOnClick : MonoBehaviour {
	private IVector3  _pos;
	public IVector3 Position
	{
		set { _pos = value; }
		get { return _pos; }
	}

	private GridHandler.ComponentDirection _direction = GridHandler.ComponentDirection.LEFT;
	public GridHandler.ComponentDirection Direction
	{
		set { _direction = value; }
		get { return _direction; }
	}
	private int _rotClicks=0;
	// Use this for initialization
	void Start () {
		ForwardTouch ft = gameObject.ForceGetComponent<ForwardTouch> ();
		ft.Clicked += RotateComponent;
	}

	//Rotate component
	private void RotateComponent(GameObject _object)
	{
		if (_rotClicks < 4) {
			_object.transform.Rotate (90, 0, 0);
			if (_rotClicks == 0)
				_direction = GridHandler.ComponentDirection.DOWN;
			else if (_rotClicks == 1)
				_direction = GridHandler.ComponentDirection.RIGHT;
			else if (_rotClicks == 2)
				_direction = GridHandler.ComponentDirection.UP;
			else
				_direction = GridHandler.ComponentDirection.LEFT;
			_rotClicks++;
		} else if (_rotClicks < 8) {
			_object.transform.Rotate (0, 0, 90);
			_direction = GridHandler.ComponentDirection.LEFT;
			_rotClicks++;
		} else if (_rotClicks < 12) {
			_object.transform.Rotate (0, 90, 0);
			if (_rotClicks == 8)
				_direction = GridHandler.ComponentDirection.BACK;
			else if (_rotClicks == 9)
				_direction = GridHandler.ComponentDirection.RIGHT;
			else if (_rotClicks == 10)
				_direction = GridHandler.ComponentDirection.FRONT;
			else
				_direction = GridHandler.ComponentDirection.LEFT;
			_rotClicks++;
		}else {
			_rotClicks = 0;
		}
		Debug.Log(_direction);
	}
}
