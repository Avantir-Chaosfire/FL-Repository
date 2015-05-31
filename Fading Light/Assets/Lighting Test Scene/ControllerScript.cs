using UnityEngine;
using System.Collections;

public class ControllerScript : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		//if (Input.GetKeyDown ("space"))
		//	debug ();
		//testing sccessful, SolidWallScript correctly does what its suppose to do (scene view camera must also not be on collider)
	}

	void debug()
	{
		for(int n = 0; n < DataBase.corners.Count; n ++)
		{
			for(int i = 0; i < DataBase.corners[n].points.Length; i ++)
			{
				Debug.Log(DataBase.corners[n].points[i].x + "," + DataBase.corners[n].points[i].y);
			}
		}
	}
}
