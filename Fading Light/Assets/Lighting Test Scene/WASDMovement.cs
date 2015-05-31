using UnityEngine;
using System.Collections;

public class WASDMovement : MonoBehaviour {

	public float moveSpeed;
	// Use this for initialization
	void Start () {
		moveSpeed = 0.05f;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey("w"))
		{
			transform.position = new Vector3(transform.position.x, transform.position.y + moveSpeed, transform.position.z);
		}
		if(Input.GetKey("a"))
		{
			transform.position = new Vector3(transform.position.x - moveSpeed, transform.position.y, transform.position.z);
		}
		if(Input.GetKey("s"))
		{
			transform.position = new Vector3(transform.position.x, transform.position.y - moveSpeed, transform.position.z);
		}
		if(Input.GetKey("d"))
		{
			transform.position = new Vector3(transform.position.x + moveSpeed, transform.position.y, transform.position.z);
		}
	}
}
