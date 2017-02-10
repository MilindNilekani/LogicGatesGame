using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomTools;
using CustomTools.ForwardEvents;

public class GameHandler : MonoBehaviour {
	//Object of GridHandler
	public GridHandler _grid;

	//Logic calculation related attributes
	public bool[] _logicInput = new bool[5];
	private List<bool> _logicVals;

	//Lists
	private List<InputComponent> input;
	private List<WireComponent> wires;
	private List<NOTComponent> not;
	private List<ORComponent> or;
	private List<SplitterComponent> splitter;
	private OutputComponent output;
	private List<IVector3> electronPos;

	//Singleton for GameHandler
	public static GameHandler instance = null;

	void Awake()
	{
		//Singleton called at beginning of scene
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
		//Attach to gameobject in the first scene
		DontDestroyOnLoad (gameObject);
	}

	//OnClickSimulation
	public void StartSimulation()
	{
		FindAllComponents ();
	}

	private void FindAllComponents()
	{
		input = new List<InputComponent> ();
		wires = new List<WireComponent> ();
		not = new List<NOTComponent> ();
		or = new List<ORComponent> ();
		splitter = new List<SplitterComponent> ();
		output = null;
		electronPos = new List<IVector3> ();
		_logicVals = new List<bool> ();
		int val = 0;
		for (int i = 0; i < ConstantHandler.Instance.GridLength; i++) {
			for (int j = 0; j < ConstantHandler.Instance.GridWidth; j++) {
				for (int k = 0; k < ConstantHandler.Instance.GridHeight; k++) {
					GameObject spot;
					if (_grid.GetComponentObject (new IVector3 (i, j, k), out spot)) {
						GridHandler.SpotType _type = _grid.GetComponentType (i, j, k);
						switch (_type) {
						case GridHandler.SpotType.INPUT:
							{
								GridHandler.ComponentDirection dir = spot.GetComponent<RotateOnClick> ().Direction;
								input.Add (new InputComponent (new IVector3 (i, j, k), dir, _logicInput[val]));
								_logicVals.Add (_logicInput [val]);
								val++;
								electronPos.Add (new IVector3(i,j,k));
								break;
							}
						case GridHandler.SpotType.WIRE:
							{
								GridHandler.ComponentDirection dir = spot.GetComponent<RotateOnClick> ().Direction;
								wires.Add (new WireComponent (new IVector3 (i, j, k), dir));
								break;
							}
						case GridHandler.SpotType.OUTPUT:
							{
								GridHandler.ComponentDirection dir = spot.GetComponent<RotateOnClick> ().Direction;
								output = new OutputComponent (new IVector3 (i, j, k), dir);
								break;
							}
						case GridHandler.SpotType.NOT:
							{
								GridHandler.ComponentDirection dir = spot.GetComponent<RotateOnClick> ().Direction;
								not.Add (new NOTComponent (new IVector3 (i, j, k), dir));
								break;
							}
						case GridHandler.SpotType.OR_LEFT:
							{
								GridHandler.ComponentDirection dir = spot.GetComponent<SiblingsDir> ().Direction;
								wires.Add (new WireComponent (new IVector3 (i, j, k), dir));
								break;
							}
						case GridHandler.SpotType.OR_RIGHT:
							{
								GridHandler.ComponentDirection dir = spot.GetComponent<SiblingsDir> ().Direction;
								wires.Add (new WireComponent (new IVector3 (i, j, k), dir));
								break;
							}
						case GridHandler.SpotType.OR_CENTRE:
							{
								GridHandler.ComponentDirection dir = spot.GetComponent<RotAndSiblingPosChange>().Direction;
								or.Add (new ORComponent (new IVector3 (i, j, k), dir));
								break;
							}
						case GridHandler.SpotType.SPLITTER:
							{
								GridHandler.ComponentDirection dir = spot.GetComponent<SplitterRotateOnClick> ().Direction;
								GridHandler.ComponentDirection sec = spot.GetComponent<SplitterRotateOnClick> ().SecondaryDirection;
								splitter.Add (new SplitterComponent (new IVector3 (i, j, k), dir,sec));
								break;
							}
						}
					}
				}
			}
		}
		if (input.Count < 1) {
			Debug.LogError ("No inputs detected");
			return;
		}
		if (output == null) {
			Debug.LogError ("No output detected");
			return;
		}
		MoveElectron ();
	}

	private void PerformNotOperation(IVector3 pos, int index)
	{
		foreach (NOTComponent nc in not) {
			if (nc.Position == pos) {
				bool _tempVal;
				nc.Compute (_logicVals [index], out _tempVal);
				_logicVals [index] = _tempVal;
				electronPos [index] = nc.MoveElectron ();
				Debug.Log ("Electron moved through NOT " + electronPos [index] + " with value =" + _logicVals[index]);
				GridHandler.SpotType type = _grid.GetComponentType (electronPos [index].x, electronPos [index].y, electronPos [index].z);
				switch (type) {
				case GridHandler.SpotType.EMPTY:
					Debug.LogError ("Disconnected not gate at " + electronPos [index]);
					break;
				case GridHandler.SpotType.INPUT:
					Debug.LogError ("Not cannot be connected to input wire");
					break;
				case GridHandler.SpotType.NOT:
					PerformNotOperation (electronPos [index], index);
					break;
				case GridHandler.SpotType.WIRE:
					PerformWireOperation (electronPos [index], index);
					break;
				case GridHandler.SpotType.OUTPUT:
					FinalOutput (electronPos [index], index);
					break;
				case GridHandler.SpotType.OR_LEFT:
					PerformWireOperation (electronPos [index], index);
					break;
				case GridHandler.SpotType.OR_RIGHT:
					PerformWireOperation (electronPos [index], index);
					break;
				case GridHandler.SpotType.OR_CENTRE:
					PerformOrOperation (electronPos [index], index);
					break;
				}
			}
		}
	}

	private void PerformWireOperation(IVector3 pos, int index)
	{
		foreach (WireComponent wc in wires) {
			if (wc.Position == pos) {
				bool _tempVal;
				wc.Compute (_logicVals [index], out _tempVal);
				_logicVals [index] = _tempVal;
				electronPos [index] = wc.MoveElectron ();
				Debug.Log ("Electron moved through WIRE " + electronPos [index] + " with value =" + _logicVals[index]);
				GridHandler.SpotType type = _grid.GetComponentType (electronPos [index].x, electronPos [index].y, electronPos [index].z);
				switch (type) {
				case GridHandler.SpotType.EMPTY:
					Debug.LogError ("Disconnected wire at " + electronPos [index]);
					break;
				case GridHandler.SpotType.INPUT:
					Debug.LogError ("Wire cannot be connected to input wire");
					break;
				case GridHandler.SpotType.NOT:
					PerformNotOperation (electronPos [index], index);
					break;
				case GridHandler.SpotType.WIRE:
					PerformWireOperation (electronPos [index], index);
					break;
				case GridHandler.SpotType.OUTPUT:
					FinalOutput (electronPos [index], index);
					break;
				case GridHandler.SpotType.OR_LEFT:
					PerformWireOperation (electronPos [index], index);
					break;
				case GridHandler.SpotType.OR_RIGHT:
					PerformWireOperation (electronPos [index], index);
					break;
				case GridHandler.SpotType.OR_CENTRE:
					PerformOrOperation (electronPos [index], index);
					break;
				}
			}
		}
	}

	private void PerformSplitOperation(IVector3 pos, int index)
	{
		int newIndex;
		foreach (SplitterComponent sc in splitter) {
			if (sc.Position == pos) {
				bool newValAdded = _logicVals [index];
				_logicVals.Add (newValAdded);
				newIndex = _logicVals.Count-1;
				bool _tempVal1, _tempVal2;
				sc.Compute (_logicVals [index], out _tempVal1);
				sc.Compute (_logicVals [newIndex], out _tempVal2);
				_logicVals [index] = _tempVal1;
				_logicVals [newIndex] = _tempVal2;

				IVector3 newPos = sc.MoveSecElectron ();
				electronPos.Add (newPos);
				electronPos [index] = sc.MoveElectron ();
				Debug.Log ("Electron moved through SPLITTER " + electronPos [index] + " with value =" + _logicVals[index] +" and " + electronPos [newIndex] + " with value =" + _logicVals[newIndex]);
				GridHandler.SpotType type1 = _grid.GetComponentType (electronPos [index].x, electronPos [index].y, electronPos [index].z);
				GridHandler.SpotType type2 = _grid.GetComponentType (electronPos [newIndex].x, electronPos [newIndex].y, electronPos [newIndex].z);
				switch (type1) {
				case GridHandler.SpotType.EMPTY:
					Debug.LogError ("Disconnected wire at " + electronPos [index]);
					break;
				case GridHandler.SpotType.INPUT:
					Debug.LogError ("Wire cannot be connected to input wire");
					break;
				case GridHandler.SpotType.NOT:
					PerformNotOperation (electronPos [index], index);
					break;
				case GridHandler.SpotType.WIRE:
					PerformWireOperation (electronPos [index], index);
					break;
				case GridHandler.SpotType.OUTPUT:
					FinalOutput (electronPos [index], index);
					break;
				case GridHandler.SpotType.OR_LEFT:
					PerformWireOperation (electronPos [index], index);
					break;
				case GridHandler.SpotType.OR_RIGHT:
					PerformWireOperation (electronPos [index], index);
					break;
				case GridHandler.SpotType.OR_CENTRE:
					PerformOrOperation (electronPos [index], index);
					break;
				case GridHandler.SpotType.SPLITTER:
					PerformSplitOperation (electronPos [index], index);
					break;
				}
				switch (type2) {
				case GridHandler.SpotType.EMPTY:
					Debug.LogError ("Disconnected wire at " + electronPos [newIndex]);
					break;
				case GridHandler.SpotType.INPUT:
					Debug.LogError ("Wire cannot be connected to input wire");
					break;
				case GridHandler.SpotType.NOT:
					PerformNotOperation (electronPos [newIndex], newIndex);
					break;
				case GridHandler.SpotType.WIRE:
					PerformWireOperation (electronPos [newIndex], newIndex);
					break;
				case GridHandler.SpotType.OUTPUT:
					FinalOutput (electronPos [newIndex], newIndex);
					break;
				case GridHandler.SpotType.OR_LEFT:
					PerformWireOperation (electronPos [newIndex], newIndex);
					break;
				case GridHandler.SpotType.OR_RIGHT:
					PerformWireOperation (electronPos [newIndex], newIndex);
					break;
				case GridHandler.SpotType.OR_CENTRE:
					PerformOrOperation (electronPos [newIndex], newIndex);
					break;
				case GridHandler.SpotType.SPLITTER:
					PerformSplitOperation (electronPos [newIndex], newIndex);
					break;
				}
			}
		}
	}

	private void FinalOutput (IVector3 pos, int index)
	{
		IVector3 final = output.MoveElectron ();
		if (_grid.GetComponentType (final.x, final.y, final.z) != GridHandler.SpotType.EMPTY) {
			Debug.LogError ("Output cannot be connected to anything");
		}
		Debug.Log ("Final!" + final);
		bool tempVal;
		output.Compute (_logicVals [index], out tempVal);
		Debug.Log ("Final val" + tempVal);
	}

	private void PerformOrOperation (IVector3 pos, int index)
	{
		foreach (ORComponent oc in or) {
			if (oc.Position == pos) {
				if (oc.Input.Count == 1) {
					oc.Input.Add (_logicVals [index]);
					bool _tempVal;
					oc.Compute (out _tempVal);
					_logicVals [index] = _tempVal;
					//_logicVals.RemoveAt (index);
					electronPos [index] = oc.MoveElectron ();
					Debug.Log ("Electron moved through OR " + electronPos [index] + " with value =" + _tempVal);
					GridHandler.SpotType type = _grid.GetComponentType (electronPos [index].x, electronPos [index].y, electronPos [index].z);
					switch (type) {
					case GridHandler.SpotType.EMPTY:
						Debug.LogError ("Disconnected input wire at " + electronPos [index]);
						break;
					case GridHandler.SpotType.INPUT:
						Debug.LogError ("Input wires cannot be connected to each other");
						break;
					case GridHandler.SpotType.NOT:
						PerformNotOperation (electronPos [index], index);
						break;
					case GridHandler.SpotType.WIRE:
						PerformWireOperation (electronPos [index], index);
						break;
					case GridHandler.SpotType.OUTPUT:
						FinalOutput (electronPos [index], index);
						break;
					case GridHandler.SpotType.OR_LEFT:
						PerformWireOperation (electronPos [index], index);
						break;
					case GridHandler.SpotType.OR_RIGHT:
						PerformWireOperation (electronPos [index], index);
						break;
					case GridHandler.SpotType.OR_CENTRE:
						PerformOrOperation (electronPos [index], index);
						break;
					}

				} else {
					oc.Input.Add (_logicVals [index]);
				}
			}
		}
	}

	private void MoveElectron()
	{
		foreach (InputComponent ic in input) {
			int i = input.IndexOf (ic);
			bool _tempVal;
			ic.Compute (_logicVals [i], out _tempVal);
			_logicVals [i] = _tempVal;
			electronPos [i] = ic.MoveElectron ();
			Debug.Log ("Electron moved through INPUT " + electronPos [i] + " with value =" + _logicVals[i]);
			GridHandler.SpotType type = _grid.GetComponentType (electronPos [i].x, electronPos [i].y, electronPos [i].z);
			switch (type) {
			case GridHandler.SpotType.EMPTY:
				Debug.LogError ("Disconnected input wire at " + electronPos [i]);
				break;
			case GridHandler.SpotType.INPUT:
				Debug.LogError ("Input wires cannot be connected to each other");
				break;
			case GridHandler.SpotType.NOT:
				PerformNotOperation (electronPos [i], i);
				break;
			case GridHandler.SpotType.WIRE:
				PerformWireOperation (electronPos [i], i);
				break;
			case GridHandler.SpotType.OUTPUT:
				FinalOutput (electronPos [i], i);
				break;
			case GridHandler.SpotType.OR_LEFT:
				PerformWireOperation (electronPos [i], i);
				break;
			case GridHandler.SpotType.OR_RIGHT:
				PerformWireOperation (electronPos [i], i);
				break;
			case GridHandler.SpotType.OR_CENTRE:
				PerformOrOperation (electronPos [i], i);
				break;
			case GridHandler.SpotType.SPLITTER:
				PerformSplitOperation (electronPos [i], i);
				break;
			}
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
