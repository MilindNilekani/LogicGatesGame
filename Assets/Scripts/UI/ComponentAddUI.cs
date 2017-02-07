using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CustomTools;

public class ComponentAddUI : MonoBehaviour, IBeginDragHandler,IDragHandler,IEndDragHandler {

	private GridHandler _grid;
	public GridHandler.SpotType _typeOfItem;
	public void OnBeginDrag(PointerEventData data)
	{
		GameObject i=Instantiate (gameObject, gameObject.GetComponent<RectTransform>().position, transform.rotation) as GameObject;
		i.transform.SetParent (PrefabHandler.Instance.Canvas.transform, false);
		i.GetComponent<RectTransform>().position=gameObject.GetComponent<RectTransform> ().position;
	}

	public void OnDrag(PointerEventData data)
	{
		ConstantHandler.Instance.ComponentDragged = true;
		gameObject.GetComponent<RectTransform> ().position = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
	}

	public void OnEndDrag(PointerEventData data)
	{
		ConstantHandler.Instance.ComponentDragged = false;
		if (ConstantHandler.Instance.ComponentAdded) {
			IVector3 pos = ConstantHandler.Instance.PositionComponentAdded;
			_grid.SetSpotToType (_typeOfItem, pos.x, pos.y, pos.z);
			_grid.AttachComponent (pos.x,pos.y,pos.z);
			ConstantHandler.Instance.PositionComponentAdded = IVector3.zero;
			ConstantHandler.Instance.ComponentAdded = false;
		}
		Destroy (gameObject);
	}

	void Start()
	{
		_grid = GameObject.Find ("GridGenerator").GetComponent<GridHandler> ();
	}
}
