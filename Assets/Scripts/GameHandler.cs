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
	private List<bool> _logicVals = new List<bool> ();
	private bool _wireFlag, _notFlag;

	//Lists
	private List<InputComponent> input=new List<InputComponent>();
	private List<WireComponent> wires=new List<WireComponent>();
	private List<WireComponent> wiresChecked = new List<WireComponent> ();
	private List<NOTComponent> not = new List<NOTComponent> ();
	private List<NOTComponent> notChecked = new List<NOTComponent> ();
	private OutputComponent output;
	private List<IVector3> electronPos = new List<IVector3> ();

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

	private void MoveElectron()
	{
		foreach (InputComponent ic in input) {
			int i = input.IndexOf (ic);
			bool _tempVal;
			ic.Compute (_logicVals [i], out _tempVal);
			_logicVals [i] = _tempVal;
			electronPos [i] = ic.MoveElectron ();
			foreach (InputComponent inp in input) {
				if (electronPos [i] == inp.Position) {
					Debug.LogError ("Inputs cannot be connected to each other");
					return;
				}
			}
		}
		while(wiresChecked.Count!=wires.Count)
		{
			foreach (WireComponent wc in wires) {
				if (!wiresChecked.Contains (wc)) {
					if (electronPos.Contains (wc.Position)) {
						int i = electronPos.IndexOf (wc.Position);
						bool _tempVal;
						wc.Compute (_logicVals [i], out _tempVal);
						_logicVals [i] = _tempVal;
						electronPos [i] = wc.MoveElectron ();
						wiresChecked.Add (wc);
					} else {
						_wireFlag = true;
					}
				}
			}
			if (_wireFlag) {
				Debug.LogError ("Disconnected or backflow of wires");
				return;
			}
		}
		while(notChecked.Count!=not.Count)
		{
			foreach (NOTComponent nc in not) {
				if (!notChecked.Contains (nc)) {
					if (electronPos.Contains (nc.Position)) {
						int i = electronPos.IndexOf (nc.Position);
						bool _tempVal;
						nc.Compute (_logicVals [i], out _tempVal);
						_logicVals [i] = _tempVal;
						electronPos [i] = nc.MoveElectron ();
						notChecked.Add (nc);
					} else {
						_notFlag = true;
					}
				}
			}
			if (_notFlag) {
				Debug.LogError ("Not gate not connected or backflow of gate");
				return;
			}
		}
		if (output != null) {
			if (electronPos.Contains (output.Position)) {
				Debug.Log ("Final!" + output.MoveElectron ());
				bool tempVal;
				output.Compute (_logicVals [0], out tempVal);
				Debug.Log ("Final val" + tempVal);
			} else {
				Debug.LogError ("Output gate not connected");
				return;
			}
				
		}
	}
	

	// Update is called once per frame
	void Update () {
		
	}
}
