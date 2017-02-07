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
	//------------------------------------------------
	[SerializeField]
	private GameObject _canvas;
	public GameObject Canvas
	{
		get { return _canvas; }
	}
}
