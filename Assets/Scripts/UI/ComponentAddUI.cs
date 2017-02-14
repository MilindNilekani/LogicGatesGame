using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CustomTools;

public class ComponentAddUI : MonoBehaviour, IBeginDragHandler,IDragHandler,IEndDragHandler {
	private List<GridHandler.GridSpot> _sib = new List<GridHandler.GridSpot> ();
	private GridHandler.GridSpot sib1, sib2;
	private GridHandler _grid;
	private IVector3 eldest;
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

			if ((_typeOfItem == GridHandler.SpotType.OR_CENTRE || _typeOfItem==GridHandler.SpotType.AND_CENTRE) && _grid.isOnGrid (pos.x, pos.y, pos.z - 1) && _grid.isOnGrid (pos.x, pos.y, pos.z + 1)) {
				sib1 = _grid.GetGridSpot (pos.x, pos.y, pos.z - 1);
				sib2 = _grid.GetGridSpot (pos.x, pos.y, pos.z + 1);
				_sib.Add (sib1);
				_sib.Add (sib2);

				GridHandler.GridSpot eld;
				eld = _grid.GetGridSpot (pos.x, pos.y, pos.z);
				_grid.SetSpotToType (_typeOfItem, pos.x, pos.y, pos.z);
				_grid.AttachComponent (pos.x, pos.y, pos.z);
				_grid.SetSiblingsForComponent (pos.x, pos.y, pos.z, _sib, GridHandler.ComponentDirection.FRONT,GridHandler.ComponentDirection.BACK);
				_grid.SetEldest (sib1.Position.x, sib1.Position.y, sib1.Position.z, eld.Position);
				_grid.SetEldest (sib2.Position.x, sib2.Position.y, sib2.Position.z, eld.Position);

			} else if(_typeOfItem!=GridHandler.SpotType.OR_CENTRE && _typeOfItem!=GridHandler.SpotType.AND_CENTRE){
				if (_grid.GetComponentType (pos.x, pos.y, pos.z) == GridHandler.SpotType.OR_CENTRE || _grid.GetComponentType(pos.x,pos.y,pos.z)==GridHandler.SpotType.AND_CENTRE) {
					List<GridHandler.GridSpot> sib = _grid.GetSiblings (pos.x, pos.y, pos.z);
					foreach (GridHandler.GridSpot s in sib) {
						_grid.SetSpotToType (GridHandler.SpotType.EMPTY, s.Position.x, s.Position.y, s.Position.z);
						_grid.AttachComponent (s.Position.x, s.Position.y, s.Position.z);
					}
				} else if (_grid.GetComponentType (pos.x, pos.y, pos.z) == GridHandler.SpotType.OR_LEFT || _grid.GetComponentType (pos.x, pos.y, pos.z) == GridHandler.SpotType.OR_RIGHT || _grid.GetComponentType (pos.x, pos.y, pos.z) == GridHandler.SpotType.AND_LEFT || _grid.GetComponentType (pos.x, pos.y, pos.z) == GridHandler.SpotType.AND_RIGHT) {
					eldest=_grid.GetEldest (pos.x, pos.y, pos.z);
					List<GridHandler.GridSpot> sib = _grid.GetSiblings (eldest.x, eldest.y, eldest.z);
					foreach (GridHandler.GridSpot s in sib) {
						_grid.SetSpotToType (GridHandler.SpotType.EMPTY, s.Position.x, s.Position.y, s.Position.z);
						_grid.AttachComponent (s.Position.x, s.Position.y, s.Position.z);
					}
					_grid.SetSpotToType (GridHandler.SpotType.EMPTY, eldest.x, eldest.y, eldest.z);
					_grid.AttachComponent (eldest.x, eldest.y, eldest.z);
				}
				_grid.SetSpotToType (_typeOfItem, pos.x, pos.y, pos.z);
				_grid.AttachComponent (pos.x, pos.y, pos.z);
			}

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
