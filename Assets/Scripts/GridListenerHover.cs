using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomTools;

public class GridListenerHover : MonoBehaviour {
	private IVector3 _position;
	public IVector3 Position {
		set { _position = value; }
		get { return _position; }
	}
	private bool _goToOGcolor;

	void OnMouseEnter()
	{
		if (ConstantHandler.Instance.ComponentDragged)
			_goToOGcolor = false;
	}

	void OnMouseOver()
	{
		if (ConstantHandler.Instance.ComponentDragged) {
			GetComponent<Renderer> ().material.color = Color.red;
			ConstantHandler.Instance.ComponentAdded = true;
			ConstantHandler.Instance.PositionComponentAdded = _position;
		}
	}

	void OnMouseExit()
	{
		_goToOGcolor = true;
		ConstantHandler.Instance.ComponentAdded = false;
	}

	void Start () {
		_goToOGcolor = true;
	}

	void Update () {
		if (_goToOGcolor)
			GetComponent<Renderer> ().material.color = Color.white;
	}
}
