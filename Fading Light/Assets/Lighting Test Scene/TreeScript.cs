using UnityEngine;
using System.Collections;

[RequireComponent (typeof (SpriteRenderer))]
[RequireComponent (typeof (PolygonCollider2D))]
[RequireComponent (typeof (SolidWallScript))]
public class TreeScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SpriteRenderer renderer = GetComponent<SpriteRenderer> ();
		renderer.sprite = (Sprite)Resources.Load("base_2");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
