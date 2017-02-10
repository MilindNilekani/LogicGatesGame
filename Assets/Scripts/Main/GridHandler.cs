using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomTools;
using CustomTools.ForwardEvents;

public class GridHandler : MonoBehaviour {
	
	private GridSpot[,,] _grid;

	private Dictionary<GameObject, IVector3> _spotToPos;

	//Generate empty grid block
	void Awake () {
		_spotToPos = new Dictionary<GameObject, IVector3> ();
		_grid = new GridSpot[ConstantHandler.Instance.GridLength, ConstantHandler.Instance.GridWidth, ConstantHandler.Instance.GridHeight];

		for (int i = 0; i < ConstantHandler.Instance.GridLength; i++) {
			for (int j = 0; j < ConstantHandler.Instance.GridWidth; j++) {
				for (int k = 0; k < ConstantHandler.Instance.GridHeight; k++) {
					Vector3 pos=new Vector3(i,j,k);
					IVector3 ipos=new IVector3(i,j,k);
					GameObject obj=Instantiate (PrefabHandler.Instance.EmptyGridSphere, pos, Quaternion.identity) as GameObject;
					obj.AddComponent<GridListenerHover> ();
					obj.GetComponent<GridListenerHover> ().Position = ipos;
					_grid [i, j, k] = new GridSpot (obj,new IVector3(i,j,k));
					_spotToPos.Add (obj, ipos);
				}
			}
		}
	}

	//Return the gameobject at position
	public bool GetSpot(IVector3 position, out GameObject spot)
	{
		if (isOnGrid (position.x, position.y, position.z)) {
			spot = _grid [position.x, position.y, position.z].obj;
			return true;
		}
		Debug.LogWarning ("No spot at" + position);
		spot = null;
		return false;
	}

	public bool GetComponentObject(IVector3 position, out GameObject spot)
	{
		if (isOnGrid (position.x, position.y, position.z)) {
			spot = _grid [position.x, position.y, position.z].Component;
			return true;
		}
		Debug.LogWarning ("No spot at" + position);
		spot = null;
		return false;
	}

	//Change type of spot at X,Y,Z
	public void SetSpotToType(SpotType newType, int X, int Y, int Z)
	{
		if (isOnGrid (X, Y, Z)) {
			_grid [X, Y, Z].type = newType;
			//Debug.Log ("Type changed to " + newType + " at " + new IVector3 (X, Y, Z));
		} else {
			Debug.LogError ("OMG Off the grid error");
		}
	}

	public ComponentDirection GetComponentDirection(int X, int Y,int Z)
	{
		if (isOnGrid (X, Y, Z)) {
			//Debug.Log (_grid [X, Y, Z].Direction);
			return _grid [X, Y, Z].Direction;
		}
		return ComponentDirection.LEFT;
	}

	public void SetComponentDirection(ComponentDirection dir, int X, int Y, int Z)
	{
		if (isOnGrid (X, Y, Z)) {
			_grid [X, Y, Z].Direction = dir;
			//Debug.Log ("Direction changed to " + dir + " at " + new IVector3 (X, Y, Z));
		} else {
			Debug.LogError ("OMG Off the grid error");
		}
	}

	//Returns type of spot at X,Y,Z
	public SpotType GetComponentType(int X, int Y, int Z)
	{
		if (isOnGrid(X, Y, Z))
			return _grid [X, Y, Z].type;
		return SpotType.EMPTY;
	}

	//Checks if spot exists and returns position for that gameobject
	public bool GetCoordinates(GameObject spot, out IVector3 position)
	{
		if(_spotToPos.TryGetValue(spot, out position))
			return true;
		Debug.LogWarning("Could not find " + spot + " in the spot dictionary!");
		return false;
	}

	//Checks if X,Y,Z lies on grid
	public bool isOnGrid(int X, int Y, int Z)
	{
		return  (0<= X && X < ConstantHandler.Instance.GridLength && 0 <= Y && Y < ConstantHandler.Instance.GridWidth && 0 <= Z && Z < ConstantHandler.Instance.GridHeight);
	}

	public void SetSiblingsForComponent(int X, int Y, int Z, List<GridSpot> sib, ComponentDirection dir1, ComponentDirection dir2)
	{
		_grid [X, Y, Z].Siblings.Clear ();
		Debug.Log (sib.Count);
		SetComponentDirection (dir1, sib [0].Position.x, sib [0].Position.y, sib [0].Position.z);
		SetComponentDirection (dir2, sib [1].Position.x, sib [1].Position.y, sib [1].Position.z);
		SetSpotToType (SpotType.OR_LEFT, sib [0].Position.x, sib [0].Position.y, sib [0].Position.z);
		SetSpotToType (SpotType.OR_RIGHT, sib [1].Position.x, sib [1].Position.y, sib [1].Position.z);
		AttachComponent (sib [0].Position.x, sib [0].Position.y, sib [0].Position.z);
		AttachComponent (sib [1].Position.x, sib [1].Position.y, sib [1].Position.z);
		foreach (GridSpot s in sib) {
			if (isOnGrid (s.Position.x, s.Position.y, s.Position.z)) {
				_grid [X, Y, Z].Siblings.Add (s);
			}
			//Debug.Log (_grid [X, Y, Z].Siblings.Count);
		}
	}

	public void SetEldest(int X, int Y, int Z, IVector3 eld)
	{
		if (isOnGrid (X, Y, Z))
			_grid [X, Y, Z].Eldest = eld;
	}

	public IVector3 GetEldest(int X, int Y, int Z)
	{
		return _grid [X, Y, Z].Eldest;
	}

	public List<GridSpot> GetSiblings(int X, int Y, int Z)
	{
		if (isOnGrid (X, Y, Z))
			return _grid [X, Y, Z].Siblings;
		return null;
	}

	public GridSpot GetGridSpot(int X, int Y, int Z)
	{
		return _grid [X, Y, Z];
	}
		
	//Attaches a component to the gameobject at X,Y,Z
	public void AttachComponent(int X, int Y, int Z)
	{
		if (isOnGrid (X, Y, Z))
			_grid [X, Y, Z].AttachComponent ();
	}

	//Types of components
	public enum SpotType
	{
		EMPTY, WIRE, INPUT, OUTPUT, NOT, OR_LEFT, OR_CENTRE, OR_RIGHT, SPLITTER
	}
	//OR sides are basically wires while OR centre is basically the OR gate

	public enum ComponentDirection
	{
		RIGHT, LEFT, FRONT, BACK, UP, DOWN
	}

	//Gridspot struct
	public struct GridSpot
	{
		//---------------Private variables-------------------------
		private ComponentDirection _direction; //Find direction the component is facing or flow
		public ComponentDirection Direction
		{
			set { _direction = value; }
			get { return _direction; }
		}
		private IVector3 _position; //Position for each gridspot
		public IVector3 Position
		{
			set { _position = value; }
			get { return _position; }
		}
		private GameObject _componentAttached; //Component attached to a particular gridspot
		public GameObject Component
		{
			get { return _componentAttached; }
		}

		private GameObject _obj; //Empty grid spot
		public GameObject obj
		{
			get { return _obj; }
		}
		private SpotType _type; //Type of component on gridspot 
		public SpotType type
		{
			set { _type = value; }
			get { return _type; }
		}
		private List<GridSpot> _siblings;
		public List<GridSpot> Siblings
		{
			set { _siblings=value;	}
			get { return _siblings; }
		}
		private IVector3 _eldest;
		public IVector3 Eldest
		{
			set { _eldest = value; }
			get { return _eldest; }
		}
		private ComponentDirection _secondaryDirection;
		public ComponentDirection SecondaryDirection
		{
			get { return _secondaryDirection; }
		}
		//----------------------------------------------------------

		//Constructor
		public GridSpot(GameObject obj, IVector3 pos)
		{
			_direction=ComponentDirection.LEFT;
			_position=pos;
			_componentAttached=null;
			_obj=obj;
			_type=SpotType.EMPTY;
			_siblings=new List<GridSpot>();
			_eldest=pos;
			_secondaryDirection=ComponentDirection.FRONT;
		}

		//----------------------Methods--------------------------------
		//Attach hovering to attach component
		private void AddHoveringForMod(GameObject _object)
		{
			_object.AddComponent<GridListenerHover> ();
			_object.GetComponent<GridListenerHover> ().Position = _position;
		}

		private void SetDirectionOfComponent(ComponentDirection dir)
		{
			_direction = dir;
		}

		//To allow changing of rotation of component
		private void AddRotationOnClick(GameObject _object)
		{
			_object.AddComponent<RotateOnClick> ();
			_object.GetComponent<RotateOnClick> ().Position = _position;
			_object.GetComponent<RotateOnClick> ().Direction = _direction;
		}

		//Clear everything at that spot
		public void ClearComponent()
		{
			if (_componentAttached != null) {
				Destroy (_componentAttached);
				_siblings.Clear ();
				_obj.GetComponent<Renderer> ().enabled = false;
			}
		}

		private void AddRotationAndSiblingPosChangeOnClick(GameObject _object)
		{
			_object.AddComponent<RotAndSiblingPosChange> ();
			_object.GetComponent<RotAndSiblingPosChange> ().Position = _position;
			_object.GetComponent<RotAndSiblingPosChange> ().Direction = _direction;
			_object.GetComponent<RotAndSiblingPosChange> ().Siblings = _siblings;
		}

		private void AddSplitterRotationOnClick(GameObject _object)
		{
			_object.AddComponent<SplitterRotateOnClick> ();
			_object.GetComponent<SplitterRotateOnClick> ().Position = _position;
			_object.GetComponent<SplitterRotateOnClick> ().Direction = _direction;
			_object.GetComponent<SplitterRotateOnClick> ().SecondaryDirection = _secondaryDirection;
		}

		private void AddSiblingOnClick(GameObject _object)
		{
			_object.AddComponent<SiblingsDir> ();
			_object.GetComponent<SiblingsDir> ().Direction = _direction;
		}

		//Attahc component at that spot
		public void AttachComponent()
		{
			ClearComponent ();
			switch (_type) {
			case SpotType.EMPTY:
				_obj.GetComponent<Renderer> ().enabled = true;
				break;
			case SpotType.WIRE:
				_obj.GetComponent<Renderer> ().enabled = false;
				_componentAttached = Instantiate (PrefabHandler.Instance.Wire, obj.transform.position, PrefabHandler.Instance.Wire.transform.rotation) as GameObject;
				AddHoveringForMod (_componentAttached);
				AddRotationOnClick (_componentAttached);
				break;
			case SpotType.INPUT:
				_obj.GetComponent<Renderer> ().enabled = false;
				_componentAttached = Instantiate (PrefabHandler.Instance.Input, obj.transform.position, PrefabHandler.Instance.Input.transform.rotation) as GameObject;
				AddHoveringForMod (_componentAttached);
				AddRotationOnClick (_componentAttached);
				break;
			case SpotType.OUTPUT:
				_obj.GetComponent<Renderer> ().enabled = false;
				_componentAttached = Instantiate (PrefabHandler.Instance.Output, obj.transform.position, PrefabHandler.Instance.Output.transform.rotation) as GameObject;
				AddHoveringForMod (_componentAttached);
				AddRotationOnClick (_componentAttached);
				break;
			case SpotType.NOT:
				_obj.GetComponent<Renderer> ().enabled = false;
				_componentAttached = Instantiate (PrefabHandler.Instance.NotGate, obj.transform.position, PrefabHandler.Instance.NotGate.transform.rotation) as GameObject;
				AddHoveringForMod (_componentAttached);
				AddRotationOnClick (_componentAttached);
				break;
			case SpotType.OR_CENTRE:
				_obj.GetComponent<Renderer> ().enabled = false;
				_componentAttached = Instantiate (PrefabHandler.Instance.ORGateCentre, obj.transform.position, PrefabHandler.Instance.ORGateCentre.transform.rotation) as GameObject;
				AddHoveringForMod (_componentAttached);
				AddRotationAndSiblingPosChangeOnClick (_componentAttached);
				break;
			case SpotType.OR_LEFT:
				_obj.GetComponent<Renderer> ().enabled = false;
				_componentAttached = Instantiate (PrefabHandler.Instance.ORGateLeft, obj.transform.position, PrefabHandler.Instance.ORGateLeft.transform.rotation) as GameObject;
				AddHoveringForMod (_componentAttached);
				AddSiblingOnClick (_componentAttached);
				break;
			case SpotType.OR_RIGHT:
				_obj.GetComponent<Renderer> ().enabled = false;
				_componentAttached = Instantiate (PrefabHandler.Instance.ORGateRight, obj.transform.position, PrefabHandler.Instance.ORGateRight.transform.rotation) as GameObject;
				AddHoveringForMod (_componentAttached);
				AddSiblingOnClick (_componentAttached);
				break;
			case SpotType.SPLITTER:
				_obj.GetComponent<Renderer> ().enabled = false;
				_componentAttached = Instantiate (PrefabHandler.Instance.Splitter, obj.transform.position, PrefabHandler.Instance.Splitter.transform.rotation) as GameObject;
				AddHoveringForMod (_componentAttached);
				AddSplitterRotationOnClick (_componentAttached);
				break;
			}
		}
		//---------------------------------------------------------------------
	}
}
