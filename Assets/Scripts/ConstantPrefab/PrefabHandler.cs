using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomTools;

public class PrefabHandler : Singleton<PrefabHandler> {

	protected PrefabHandler()
	{
	}
	//---------------Components----------------------
	[SerializeField]
	private GameObject _emptyGrid;
	public GameObject EmptyGridSphere
	{
		get { return _emptyGrid; }
	}
	[SerializeField]
	private GameObject _wire;
	public GameObject Wire
	{
		get { return _wire; }
	}
	[SerializeField]
	private GameObject _input;
	public GameObject Input
	{
		get { return _input; }
	}
	[SerializeField]
	private GameObject _output;
	public GameObject Output
	{
		get { return _output; }
	}
	[SerializeField]
	private GameObject _notGate;
	public GameObject NotGate
	{
		get { return _notGate; }
	}
	[SerializeField]
	private GameObject _orGateCentre;
	public GameObject ORGateCentre
	{
		get { return _orGateCentre; }
	}
	[SerializeField]
	private GameObject _gateLeft;
	public GameObject GateLeft
	{
		get { return _gateLeft; }
	}
	[SerializeField]
	private GameObject _gateRight;
	public GameObject GateRight
	{
		get { return _gateRight; }
	}
	[SerializeField]
	private GameObject _splitterPrefab;
	public GameObject Splitter
	{
		get { return _splitterPrefab; }
	}
	[SerializeField]
	private GameObject _andGateCentrePrefab;
	public GameObject ANDGateCentre
	{
		get { return _andGateCentrePrefab; }
	}
	//------------------------------------------------
	[SerializeField]
	private GameObject _canvas;
	public GameObject Canvas
	{
		get { return _canvas; }
	}
}
