using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SolidWallScript : MonoBehaviour {

	PolygonCollider2D col;
	// Use this for initialization
	void Start () {
		col = GetComponent<PolygonCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//remove this collider from the list of visible collider in database
	void OnBecameInvisible() {

		DataBase.corners.Remove(col);
	}

	//add this collider to the list of visible collider is database
	void OnBecameVisible() {

		DataBase.corners.Add(col);
	}

}
