using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float distance = 100f;
		RaycastHit2D testingRay = Physics2D.Raycast(transform.position, convertToPoint(transform.rotation.eulerAngles.z), distance, (1 << LayerMask.NameToLayer("SolidWallLayer")));
		testingRay.fraction = transform.rotation.eulerAngles.z;
		testingRay.distance = distance;
		testingRay.centroid = transform.position;
		if (testingRay.collider)
		{
			testingRay.point = testingRay.collider.gameObject.GetComponent<SolidWallScript> ().getRayExit(testingRay);
			//testingRay.point = testingRay.collider.gameObject.GetComponent<SolidWallScript> ().secondClosestIntersection (testingRay);
		}
		else
		{
			testingRay.point = convertToPoint(transform.rotation.eulerAngles.z).normalized*distance + (Vector2)transform.position;
		}
		Debug.DrawRay (transform.position,testingRay.point-(Vector2)transform.position,Color.yellow);
	}

	Vector2 convertToPoint(float a)
	{
		return new Vector2 (Mathf.Cos (Mathf.Deg2Rad * a), Mathf.Sin (Mathf.Deg2Rad * a));
	}
}
