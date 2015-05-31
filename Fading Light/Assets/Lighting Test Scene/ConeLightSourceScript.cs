using UnityEngine;
using System.Collections;

public class ConeLightSourceScript : MonoBehaviour {

	public float coneWidth;
	public float coneWidthMax;
	public float coneWidthMin;
	public float coneDistance;
	public float coneDistanceMax;
	public float coneDistanceMin;
	public float percentPerScroll;

	private Vector3[] Points;
	private int numberOfLights = 5;//number of lights this light source emits
	private GameObject[] emits;
	public GameObject light1;
	public GameObject light2;
	public GameObject light3;
	public GameObject light4;
	public GameObject light5;
	// Use this for initialization
	void Start () {
		percentPerScroll = 0.1f;
		coneWidth = 359f;
		coneWidthMax = 359f;
		coneWidthMin = 60f;
		coneDistance = 4f;
		coneDistanceMax = 11f;
		coneDistanceMin = 4f;

		Points = new Vector3[numberOfLights];
		Points[0] = new Vector3(0.2f,0.2f,DataBase.lightZ);
		Points[1] = new Vector3(0.2f,-0.2f,DataBase.lightZ);
        Points[2] = new Vector3(-0.2f,-0.2f,DataBase.lightZ);
        Points[3] = new Vector3(-0.2f,0.2f,DataBase.lightZ);
        Points[4] = new Vector3(0f,0f,DataBase.lightZ);
		emits = new GameObject[numberOfLights];
		emits[0] = light1;
		emits[1] = light2;
		emits[2] = light3;
		emits[3] = light4;
		emits[4] = light5;
	}
	
	// Update is called once per frame
	void Update () {
		mouseScroll();
		placeLights();
		updateLightVariables();
	}

	Vector3[] toWorldPoint()
	{
		Vector3[] worldP = new Vector3[numberOfLights];
		for(int n = 0; n < numberOfLights; n ++)
		{
			worldP[n] = new Vector3(Points[n].x + transform.position.x,
			                        Points[n].y + transform.position.y,
			                        DataBase.lightZ);
		}
		return worldP;
	}

	void placeLights()
	{
		Vector3[] worldP = toWorldPoint();
		for(int n = 0; n < numberOfLights; n ++)
		{
			emits[n].gameObject.transform.localPosition = worldP[n];
		}
	}

	void updateLightVariables()
	{
		for(int n = 0; n < numberOfLights-1; n ++)
		{
			emits[n].GetComponent<LightScript>().angle = transform.rotation.eulerAngles.z;
			emits[n].GetComponent<LightScript>().coneWidth = coneWidth;
			emits[n].GetComponent<LightScript>().coneDistance = coneDistance;
		}
		emits[numberOfLights-1].GetComponent<LightScript>().coneWidth = 359f;
		emits[numberOfLights-1].GetComponent<LightScript>().coneDistance = 1f;
	}

	void mouseScroll()
	{
		if(Input.GetAxis("Mouse ScrollWheel") < 0) // backwards
		{
			coneDistance -= percentPerScroll*(coneDistanceMax-coneDistanceMin);
			coneWidth += percentPerScroll*(coneWidthMax-coneWidthMin);
			if(coneWidth > coneWidthMax) coneWidth = coneWidthMax;
			if(coneDistance < coneDistanceMin) coneDistance = coneDistanceMin;
		}
		if(Input.GetAxis("Mouse ScrollWheel") > 0) // forwards
		{
			coneDistance += percentPerScroll*(coneDistanceMax-coneDistanceMin);
			coneWidth -= percentPerScroll*(coneWidthMax-coneWidthMin);
			if(coneWidth < coneWidthMin) coneWidth = coneWidthMin;
			if(coneDistance > coneDistanceMax) coneDistance = coneDistanceMax;
		}
	}
}
