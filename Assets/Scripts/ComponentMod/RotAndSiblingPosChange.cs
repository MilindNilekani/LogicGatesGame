using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomTools;
using CustomTools.ForwardEvents;

public class RotAndSiblingPosChange : MonoBehaviour {
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

	private List<GridHandler.GridSpot> _siblings;
	public List<GridHandler.GridSpot> Siblings
	{
		set { _siblings = value; }
		get { return _siblings; }
	}

	private List<GridHandler.GridSpot> _tempSib=new List<GridHandler.GridSpot>();
	private GridHandler.GridSpot sib1,sib2;
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
			//Rotate OR centre
			_object.transform.Rotate (90, 0, 0);
			//Change direction of OR centre
			if (_rotClicks == 0)
				_direction = GridHandler.ComponentDirection.DOWN;
			else if (_rotClicks == 1)
				_direction = GridHandler.ComponentDirection.RIGHT;
			else if (_rotClicks == 2)
				_direction = GridHandler.ComponentDirection.UP;
			else
				_direction = GridHandler.ComponentDirection.LEFT;
			
			//Change position direction and orientation of siblings
			sib1 = GameHandler.instance._grid.GetGridSpot (_pos.x, _pos.y, _pos.z - 1);
			sib2 = GameHandler.instance._grid.GetGridSpot (_pos.x, _pos.y, _pos.z + 1);
			foreach (GridHandler.GridSpot t in _tempSib) {
				GameHandler.instance._grid.SetSpotToType (GridHandler.SpotType.EMPTY, t.Position.x, t.Position.y, t.Position.z);
				GameHandler.instance._grid.AttachComponent (t.Position.x, t.Position.y, t.Position.z);
			}
			_tempSib.Clear ();
			_tempSib.Add (sib1);
			_tempSib.Add (sib2);
			_siblings = _tempSib;

			GridHandler.GridSpot eld;
			eld = GameHandler.instance._grid.GetGridSpot (_pos.x, _pos.y, _pos.z);
			GameHandler.instance._grid.SetSiblingsForComponent (_pos.x, _pos.y, _pos.z, _tempSib, GridHandler.ComponentDirection.FRONT,GridHandler.ComponentDirection.BACK);
			GameObject obj1, obj2;
			if (GameHandler.instance._grid.GetComponentObject (sib1.Position, out obj1) && GameHandler.instance._grid.GetComponentObject (sib2.Position, out obj2)) {
				obj1.transform.eulerAngles = new Vector3 (-180, 0, 0);
				obj2.transform.eulerAngles = new Vector3 (-180, 180, 0);
				obj1.GetComponent<SiblingsDir> ().Direction = GridHandler.ComponentDirection.FRONT;
				obj2.GetComponent<SiblingsDir> ().Direction = GridHandler.ComponentDirection.BACK;
			}
			GameHandler.instance._grid.SetEldest (sib1.Position.x, sib1.Position.y, sib1.Position.z, eld.Position);
			GameHandler.instance._grid.SetEldest (sib2.Position.x, sib2.Position.y, sib2.Position.z, eld.Position);
			_rotClicks++;
			//-----------------------------
		} else if (_rotClicks < 8)
		{
			_direction = GridHandler.ComponentDirection.LEFT;
			if (GameHandler.instance._grid.isOnGrid (_pos.x, _pos.y + 1, _pos.z) && GameHandler.instance._grid.isOnGrid (_pos.x, _pos.y - 1, _pos.z) && GameHandler.instance._grid.isOnGrid (_pos.x, _pos.y, _pos.z + 1) && GameHandler.instance._grid.isOnGrid (_pos.x, _pos.y, _pos.z - 1)) {
				_object.transform.Rotate (0, 0, 90);
				if (_rotClicks == 4) {
					//left go up, right go down
					sib1 = GameHandler.instance._grid.GetGridSpot (_pos.x, _pos.y + 1, _pos.z);
					sib2 = GameHandler.instance._grid.GetGridSpot (_pos.x, _pos.y - 1, _pos.z);
					foreach (GridHandler.GridSpot t in _tempSib) {
						GameHandler.instance._grid.SetSpotToType (GridHandler.SpotType.EMPTY, t.Position.x, t.Position.y, t.Position.z);
						GameHandler.instance._grid.AttachComponent (t.Position.x, t.Position.y, t.Position.z);
					}
					_tempSib.Clear ();
					_tempSib.Add (sib1);
					_tempSib.Add (sib2);
					_siblings = _tempSib;

					GridHandler.GridSpot eld;
					eld = GameHandler.instance._grid.GetGridSpot (_pos.x, _pos.y, _pos.z);
					GameHandler.instance._grid.SetSiblingsForComponent (_pos.x, _pos.y, _pos.z, _tempSib, GridHandler.ComponentDirection.DOWN, GridHandler.ComponentDirection.UP);
					GameObject obj1, obj2;
					if (GameHandler.instance._grid.GetComponentObject (sib1.Position, out obj1) && GameHandler.instance._grid.GetComponentObject (sib2.Position, out obj2)) {
						obj1.transform.eulerAngles = new Vector3 (-90, 0, 0);
						obj2.transform.eulerAngles = new Vector3 (90, 180, 0);
						obj1.GetComponent<SiblingsDir> ().Direction = GridHandler.ComponentDirection.DOWN;
						obj2.GetComponent<SiblingsDir> ().Direction = GridHandler.ComponentDirection.UP;
					}
					GameHandler.instance._grid.SetEldest (sib1.Position.x, sib1.Position.y, sib1.Position.z, eld.Position);
					GameHandler.instance._grid.SetEldest (sib2.Position.x, sib2.Position.y, sib2.Position.z, eld.Position);
				} else if (_rotClicks == 5) {
					//Right and left switch
					sib1 = GameHandler.instance._grid.GetGridSpot (_pos.x, _pos.y, _pos.z + 1);
					sib2 = GameHandler.instance._grid.GetGridSpot (_pos.x, _pos.y, _pos.z - 1);
					foreach (GridHandler.GridSpot t in _tempSib) {
						GameHandler.instance._grid.SetSpotToType (GridHandler.SpotType.EMPTY, t.Position.x, t.Position.y, t.Position.z);
						GameHandler.instance._grid.AttachComponent (t.Position.x, t.Position.y, t.Position.z);
					}
					_tempSib.Clear ();
					_tempSib.Add (sib1);
					_tempSib.Add (sib2);
					_siblings = _tempSib;

					GridHandler.GridSpot eld;
					eld = GameHandler.instance._grid.GetGridSpot (_pos.x, _pos.y, _pos.z);
					GameHandler.instance._grid.SetSiblingsForComponent (_pos.x, _pos.y, _pos.z, _tempSib, GridHandler.ComponentDirection.BACK, GridHandler.ComponentDirection.FRONT);
					GameObject obj1, obj2;
					if (GameHandler.instance._grid.GetComponentObject (sib1.Position, out obj1) && GameHandler.instance._grid.GetComponentObject (sib2.Position, out obj2)) {
						obj1.transform.eulerAngles = new Vector3 (-180, 180, 0);
						obj2.transform.eulerAngles = new Vector3 (-180, 0, 0);
						obj1.GetComponent<SiblingsDir> ().Direction = GridHandler.ComponentDirection.BACK;
						obj2.GetComponent<SiblingsDir> ().Direction = GridHandler.ComponentDirection.FRONT;
					}
					GameHandler.instance._grid.SetEldest (sib1.Position.x, sib1.Position.y, sib1.Position.z, eld.Position);
					GameHandler.instance._grid.SetEldest (sib2.Position.x, sib2.Position.y, sib2.Position.z, eld.Position);
				} else if (_rotClicks == 6) {
					//Right go up, left go down
					sib1 = GameHandler.instance._grid.GetGridSpot (_pos.x, _pos.y - 1, _pos.z);
					sib2 = GameHandler.instance._grid.GetGridSpot (_pos.x, _pos.y + 1, _pos.z);
					foreach (GridHandler.GridSpot t in _tempSib) {
						GameHandler.instance._grid.SetSpotToType (GridHandler.SpotType.EMPTY, t.Position.x, t.Position.y, t.Position.z);
						GameHandler.instance._grid.AttachComponent (t.Position.x, t.Position.y, t.Position.z);
					}
					_tempSib.Clear ();
					_tempSib.Add (sib1);
					_tempSib.Add (sib2);
					_siblings = _tempSib;

					GridHandler.GridSpot eld;
					eld = GameHandler.instance._grid.GetGridSpot (_pos.x, _pos.y, _pos.z);
					GameHandler.instance._grid.SetSiblingsForComponent (_pos.x, _pos.y, _pos.z, _tempSib, GridHandler.ComponentDirection.UP, GridHandler.ComponentDirection.DOWN);
					GameObject obj1, obj2;
					if (GameHandler.instance._grid.GetComponentObject (sib1.Position, out obj1) && GameHandler.instance._grid.GetComponentObject (sib2.Position, out obj2)) {
						obj1.transform.eulerAngles = new Vector3 (90, 180, 0);
						obj2.transform.eulerAngles = new Vector3 (-90, 0, 0);
						obj1.GetComponent<SiblingsDir> ().Direction = GridHandler.ComponentDirection.UP;
						obj2.GetComponent<SiblingsDir> ().Direction = GridHandler.ComponentDirection.DOWN;
					}
					GameHandler.instance._grid.SetEldest (sib1.Position.x, sib1.Position.y, sib1.Position.z, eld.Position);
					GameHandler.instance._grid.SetEldest (sib2.Position.x, sib2.Position.y, sib2.Position.z, eld.Position);
				} else {
					sib1 = GameHandler.instance._grid.GetGridSpot (_pos.x, _pos.y, _pos.z - 1);
					sib2 = GameHandler.instance._grid.GetGridSpot (_pos.x, _pos.y, _pos.z + 1);
					foreach (GridHandler.GridSpot t in _tempSib) {
						GameHandler.instance._grid.SetSpotToType (GridHandler.SpotType.EMPTY, t.Position.x, t.Position.y, t.Position.z);
						GameHandler.instance._grid.AttachComponent (t.Position.x, t.Position.y, t.Position.z);
					}
					_tempSib.Clear ();
					_tempSib.Add (sib1);
					_tempSib.Add (sib2);
					_siblings = _tempSib;

					GridHandler.GridSpot eld;
					eld = GameHandler.instance._grid.GetGridSpot (_pos.x, _pos.y, _pos.z);
					GameHandler.instance._grid.SetSiblingsForComponent (_pos.x, _pos.y, _pos.z, _tempSib, GridHandler.ComponentDirection.FRONT, GridHandler.ComponentDirection.BACK);
					GameObject obj1, obj2;
					if (GameHandler.instance._grid.GetComponentObject (sib1.Position, out obj1) && GameHandler.instance._grid.GetComponentObject (sib2.Position, out obj2)) {
						obj1.transform.eulerAngles = new Vector3 (-180, 0, 0);
						obj2.transform.eulerAngles = new Vector3 (-180, 180, 0);
						obj1.GetComponent<SiblingsDir> ().Direction = GridHandler.ComponentDirection.FRONT;
						obj2.GetComponent<SiblingsDir> ().Direction = GridHandler.ComponentDirection.BACK;
					}
					GameHandler.instance._grid.SetEldest (sib1.Position.x, sib1.Position.y, sib1.Position.z, eld.Position);
					GameHandler.instance._grid.SetEldest (sib2.Position.x, sib2.Position.y, sib2.Position.z, eld.Position);
					//Left left, right right
				}
				_rotClicks++;
			} else {
				_rotClicks += 4;
			}
		} else if (_rotClicks < 12)
		{
			if (GameHandler.instance._grid.isOnGrid (_pos.x, _pos.y, _pos.z - 1) && GameHandler.instance._grid.isOnGrid (_pos.x + 1, _pos.y, _pos.z) && GameHandler.instance._grid.isOnGrid (_pos.x, _pos.y, _pos.z + 1) && GameHandler.instance._grid.isOnGrid (_pos.x - 1, _pos.y, _pos.z)) {
				_object.transform.Rotate (0, 90, 0);
				if (_rotClicks == 8) {
					_direction = GridHandler.ComponentDirection.BACK;
					sib1 = GameHandler.instance._grid.GetGridSpot (_pos.x + 1, _pos.y, _pos.z);
					sib2 = GameHandler.instance._grid.GetGridSpot (_pos.x - 1, _pos.y, _pos.z);
					foreach (GridHandler.GridSpot t in _tempSib) {
						GameHandler.instance._grid.SetSpotToType (GridHandler.SpotType.EMPTY, t.Position.x, t.Position.y, t.Position.z);
						GameHandler.instance._grid.AttachComponent (t.Position.x, t.Position.y, t.Position.z);
					}
					_tempSib.Clear ();
					_tempSib.Add (sib1);
					_tempSib.Add (sib2);
					_siblings = _tempSib;

					GridHandler.GridSpot eld;
					eld = GameHandler.instance._grid.GetGridSpot (_pos.x, _pos.y, _pos.z);
					GameHandler.instance._grid.SetSiblingsForComponent (_pos.x, _pos.y, _pos.z, _tempSib, GridHandler.ComponentDirection.LEFT, GridHandler.ComponentDirection.RIGHT);
					GameObject obj1, obj2;
					if (GameHandler.instance._grid.GetComponentObject (sib1.Position, out obj1) && GameHandler.instance._grid.GetComponentObject (sib2.Position, out obj2)) {
						obj1.transform.eulerAngles = new Vector3 (-180, -90, 0);
						obj2.transform.eulerAngles = new Vector3 (-180, 90, 0);
						obj1.GetComponent<SiblingsDir> ().Direction = GridHandler.ComponentDirection.LEFT;
						obj2.GetComponent<SiblingsDir> ().Direction = GridHandler.ComponentDirection.RIGHT;
					}
					GameHandler.instance._grid.SetEldest (sib1.Position.x, sib1.Position.y, sib1.Position.z, eld.Position);
					GameHandler.instance._grid.SetEldest (sib2.Position.x, sib2.Position.y, sib2.Position.z, eld.Position);
				} else if (_rotClicks == 9) {
					_direction = GridHandler.ComponentDirection.RIGHT;
					sib1 = GameHandler.instance._grid.GetGridSpot (_pos.x, _pos.y, _pos.z + 1);
					sib2 = GameHandler.instance._grid.GetGridSpot (_pos.x, _pos.y, _pos.z - 1);
					foreach (GridHandler.GridSpot t in _tempSib) {
						GameHandler.instance._grid.SetSpotToType (GridHandler.SpotType.EMPTY, t.Position.x, t.Position.y, t.Position.z);
						GameHandler.instance._grid.AttachComponent (t.Position.x, t.Position.y, t.Position.z);
					}
					_tempSib.Clear ();
					_tempSib.Add (sib1);
					_tempSib.Add (sib2);
					_siblings = _tempSib;

					GridHandler.GridSpot eld;
					eld = GameHandler.instance._grid.GetGridSpot (_pos.x, _pos.y, _pos.z);
					GameHandler.instance._grid.SetSiblingsForComponent (_pos.x, _pos.y, _pos.z, _tempSib, GridHandler.ComponentDirection.BACK, GridHandler.ComponentDirection.FRONT);
					GameObject obj1, obj2;
					if (GameHandler.instance._grid.GetComponentObject (sib1.Position, out obj1) && GameHandler.instance._grid.GetComponentObject (sib2.Position, out obj2)) {
						obj1.transform.eulerAngles = new Vector3 (-180, 180, 0);
						obj2.transform.eulerAngles = new Vector3 (-180, 0, 0);
						obj1.GetComponent<SiblingsDir> ().Direction = GridHandler.ComponentDirection.BACK;
						obj2.GetComponent<SiblingsDir> ().Direction = GridHandler.ComponentDirection.FRONT;
					}
					GameHandler.instance._grid.SetEldest (sib1.Position.x, sib1.Position.y, sib1.Position.z, eld.Position);
					GameHandler.instance._grid.SetEldest (sib2.Position.x, sib2.Position.y, sib2.Position.z, eld.Position);
				} else if (_rotClicks == 10) {
					_direction = GridHandler.ComponentDirection.FRONT;
					sib1 = GameHandler.instance._grid.GetGridSpot (_pos.x - 1, _pos.y, _pos.z);
					sib2 = GameHandler.instance._grid.GetGridSpot (_pos.x + 1, _pos.y, _pos.z);
					foreach (GridHandler.GridSpot t in _tempSib) {
						GameHandler.instance._grid.SetSpotToType (GridHandler.SpotType.EMPTY, t.Position.x, t.Position.y, t.Position.z);
						GameHandler.instance._grid.AttachComponent (t.Position.x, t.Position.y, t.Position.z);
					}
					_tempSib.Clear ();
					_tempSib.Add (sib1);
					_tempSib.Add (sib2);
					_siblings = _tempSib;

					GridHandler.GridSpot eld;
					eld = GameHandler.instance._grid.GetGridSpot (_pos.x, _pos.y, _pos.z);
					GameHandler.instance._grid.SetSiblingsForComponent (_pos.x, _pos.y, _pos.z, _tempSib, GridHandler.ComponentDirection.RIGHT, GridHandler.ComponentDirection.LEFT);
					GameObject obj1, obj2;
					if (GameHandler.instance._grid.GetComponentObject (sib1.Position, out obj1) && GameHandler.instance._grid.GetComponentObject (sib2.Position, out obj2)) {
						obj1.transform.eulerAngles = new Vector3 (-180, 90, 0);
						obj2.transform.eulerAngles = new Vector3 (-180, -90, 0);
						obj1.GetComponent<SiblingsDir> ().Direction = GridHandler.ComponentDirection.RIGHT;
						obj2.GetComponent<SiblingsDir> ().Direction = GridHandler.ComponentDirection.LEFT;
					}
					GameHandler.instance._grid.SetEldest (sib1.Position.x, sib1.Position.y, sib1.Position.z, eld.Position);
					GameHandler.instance._grid.SetEldest (sib2.Position.x, sib2.Position.y, sib2.Position.z, eld.Position);
				} else {
					_direction = GridHandler.ComponentDirection.LEFT;
					sib1 = GameHandler.instance._grid.GetGridSpot (_pos.x, _pos.y, _pos.z - 1);
					sib2 = GameHandler.instance._grid.GetGridSpot (_pos.x, _pos.y, _pos.z + 1);
					foreach (GridHandler.GridSpot t in _tempSib) {
						GameHandler.instance._grid.SetSpotToType (GridHandler.SpotType.EMPTY, t.Position.x, t.Position.y, t.Position.z);
						GameHandler.instance._grid.AttachComponent (t.Position.x, t.Position.y, t.Position.z);
					}
					_tempSib.Clear ();
					_tempSib.Add (sib1);
					_tempSib.Add (sib2);
					_siblings = _tempSib;

					GridHandler.GridSpot eld;
					eld = GameHandler.instance._grid.GetGridSpot (_pos.x, _pos.y, _pos.z);
					GameHandler.instance._grid.SetSiblingsForComponent (_pos.x, _pos.y, _pos.z, _tempSib, GridHandler.ComponentDirection.FRONT,GridHandler.ComponentDirection.BACK);
					GameObject obj1, obj2;
					if (GameHandler.instance._grid.GetComponentObject (sib1.Position, out obj1) && GameHandler.instance._grid.GetComponentObject (sib2.Position, out obj2)) {
						obj1.transform.eulerAngles = new Vector3 (-180, 0, 0);
						obj2.transform.eulerAngles = new Vector3 (-180, 180, 0);
						obj1.GetComponent<SiblingsDir> ().Direction = GridHandler.ComponentDirection.FRONT;
						obj2.GetComponent<SiblingsDir> ().Direction = GridHandler.ComponentDirection.BACK;
					}
					GameHandler.instance._grid.SetEldest (sib1.Position.x, sib1.Position.y, sib1.Position.z, eld.Position);
					GameHandler.instance._grid.SetEldest (sib2.Position.x, sib2.Position.y, sib2.Position.z, eld.Position);
				}
				_rotClicks++;
			} else {
				_rotClicks += 4;
			}
			//Left and right move counterwise position
		}else {
			_rotClicks = 0;
		}
		//Debug.Log(_direction);
	}
}
