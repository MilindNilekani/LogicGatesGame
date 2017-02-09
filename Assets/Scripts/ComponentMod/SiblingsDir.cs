using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomTools;

public class SiblingsDir : MonoBehaviour {

	private GridHandler.ComponentDirection _direction;
	public GridHandler.ComponentDirection Direction
	{
		set { _direction = value; }
		get { return _direction; }
	}

}
