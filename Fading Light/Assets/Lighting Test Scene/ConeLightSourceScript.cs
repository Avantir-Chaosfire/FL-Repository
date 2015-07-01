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

	public int numberOfLights = 5;//number of lights this light source emits
	private GameObject[] emits;
	private float radius = 0.2f;
	// Use this for initialization
	void Start () {
		percentPerScroll = 0.1f;
		coneWidth = 359f;
		coneWidthMax = 359f;
		coneWidthMin = 60f;
		coneDistance = 7f;
		coneDistanceMax = 13f;
		coneDistanceMin = 7f;

		emits = new GameObject[numberOfLights];
		for(int n = 0; n < numberOfLights; n++)
		{
			GameObject light = new GameObject();
			light.AddComponent<LightScript>();
			light.GetComponent<LightScript>().color = DataBase.getLightColor(numberOfLights);
			light.transform.parent = transform;
			emits[n] = light;
		}
		emits[0].transform.localPosition = new Vector2(0f,0f);
		for(int n = 1; n < numberOfLights; n++)
		{
			float angle = 360f*((float)n/(numberOfLights-1));
			Vector2 point = convertToPoint(angle)*radius;
			emits[n].transform.localPosition = point;
		}
	}
	
	// Update is called once per frame
	void Update () {
		mouseScroll();
		updateLightVariables();
	}

	Vector2 convertToPoint(float a)
	{
		return new Vector2 (Mathf.Cos (Mathf.Deg2Rad * a), Mathf.Sin (Mathf.Deg2Rad * a));
	}

	void updateLightVariables()
	{
		for(int n = 0; n < numberOfLights; n ++)
		{
			float percentage = (float)(n+1f)/(numberOfLights);
			emits[n].GetComponent<LightScript>().coneWidth = coneWidth;
			emits[n].GetComponent<LightScript>().coneDistance = coneDistance/1.7f + percentage*(coneDistance-coneDistance/1.7f);
		}
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
