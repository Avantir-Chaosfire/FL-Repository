using UnityEngine;
using System.Collections;

public class SetRenderQueue : MonoBehaviour {

	public int rq = 2000;//geometry
	// Use this for initialization
	void Start () {
		SpriteRenderer r = GetComponent<SpriteRenderer> ();
		r.material.renderQueue = rq;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
