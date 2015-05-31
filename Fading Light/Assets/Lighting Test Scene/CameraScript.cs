using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	public Transform t;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(t.position.x, t.position.y, DataBase.cameraZ);
	}
}
