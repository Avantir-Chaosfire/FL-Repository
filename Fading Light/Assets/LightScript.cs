using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LightScript : MonoBehaviour {


	private List<RaycastHit2D> rayHits;
	private RaycastHit2D[] rayHitsArray;
	public GameObject light;
	public GameObject prefab;//used for debugging
	public float coneWidth;
	public float coneWidthMax;
	public float coneWidthMin;
	public float coneDistance;
	public float coneDistanceMax;
	public float coneDistanceMin;
	public float percentPerScroll;
	// Use this for initialization
	void Start () {
		rayHits = new List<RaycastHit2D> ();

		light.AddComponent(typeof(MeshRenderer));
		MeshFilter filter = light.AddComponent(typeof(MeshFilter)) as MeshFilter;
		MeshRenderer r = light.GetComponent<MeshRenderer>();
		r.material.color = Color.white;

		coneWidth = (coneWidthMax - coneWidthMin) / 2;
		coneWidthMax = 220f;
		coneWidthMin = 50f;
		coneDistance = (coneDistanceMax - coneDistanceMin) / 2;
		coneDistanceMax = 10f;
		coneDistanceMin = 4f;
		percentPerScroll = 0.25f;
	}
	
	// Update is called once per frame
	void Update () {
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

		light.transform.position = transform.position;
		float angle1 = normalizeAngle (transform.rotation.eulerAngles.z - coneWidth/2);
		float angle2 = normalizeAngle (transform.rotation.eulerAngles.z + coneWidth/2);

		rayHitsArray = drawRays(transform.position, angle1, angle2, coneDistance);
		sortRayHits (rayHitsArray, angle2+0.01f);
		//afterSortCheck(rayHitsArray, angle2); doesnt work :(

		Vector2[] thePoints = new Vector2[rayHitsArray.Length+1];
		thePoints[0] = light.transform.position;
		for(int n = 1; n < rayHitsArray.Length+1; n ++)
		{
			thePoints[n] = rayHitsArray[n-1].point;
		}

		if(Input.GetMouseButtonDown(0))
		{
			Debug.ClearDeveloperConsole();
			Debug.Log("StartingAngle = " + angle2);
			check ();
		}

		debugLines ();
		toLocalSpace(thePoints, light.transform.position);
		MeshFilter filter = light.GetComponent<MeshFilter> ();
		filter.mesh.Clear ();
		filter.mesh = createMesh (thePoints);
	}

	//angles goes COUNTER-CLOCKWISE from start to end, angles should be normalized before using
	RaycastHit2D[] drawRays(Vector2 origin, float angleStart, float angleEnd, float distance)
	{
		//get all polygonCollider2d corners on screen (stored in DataBase)
		//draw a ray toward the angles and all corners withing the angles
		rayHits.Clear();//clear previous frame's rayHits

		Vector2 directionStart = convertToPoint(angleStart);
		Vector2 directionEnd = convertToPoint(angleEnd);

		RaycastHit2D a = Physics2D.Raycast(origin, directionStart, distance);
		RaycastHit2D b = Physics2D.Raycast(origin, directionEnd, distance);
		a.centroid = origin;
		b.centroid = origin;
		if(!a.collider) a.point = (directionStart.normalized*distance) + origin;
		if(!b.collider) b.point = (directionEnd.normalized*distance) + origin;
		rayHits.Add(a);
		rayHits.Add(b);

		float smoothness = 7f;//draw a ray every 5 degree
		float newAngle = angleStart+smoothness;
		float amount = coneWidth / smoothness;
		while(newAngle < angleStart+amount*smoothness)
		{
			Vector2 hurdur = convertToPoint(newAngle);
			
			RaycastHit2D derp = Physics2D.Raycast(origin, hurdur, distance);
			derp.centroid = origin;
			if(!derp.collider) derp.point = (hurdur.normalized*distance) + origin;
			rayHits.Add(derp);

			newAngle += smoothness;
		}

		for(int n = 0; n < DataBase.corners.Count; n ++)
		{
			for(int i = 0; i < DataBase.corners[n].points.Length; i ++)
			{
				Vector2 temp = toWorldSpace(DataBase.corners[n], i);
				Vector2 direction = new Vector2(temp.x - origin.x, temp.y - origin.y);
				float dirAngle = convertToAngle(direction);

				//if the corner is withing the light cone, cast a ray toward it
				if(isBetween(angleStart, angleEnd, dirAngle))
				{
					//cast 2 rays offset by 0.0001f so the ray will keep going if it hits a corner
					Vector2 offSet1 = convertToPoint(dirAngle + 0.01f);
					Vector2 offSet2 = convertToPoint(dirAngle - 0.01f);
					RaycastHit2D c = Physics2D.Raycast(origin, offSet1, distance);
					RaycastHit2D d = Physics2D.Raycast(origin, offSet2, distance);
					c.centroid = origin;
					d.centroid = origin;

					//if both miss then this thing is out of range
					if(!c.collider && !d.collider)
					{
						//dont bother doing anything
					}
					//if both offsets hit, just use one ray(reduce triangles)
					else if(c.collider == d.collider)
					{
						RaycastHit2D e = Physics2D.Raycast(origin, direction, distance);
						e.centroid = origin;
						rayHits.Add (e);
					}
					else//only one of the offsets must have missed(cant both miss unless out of range)
					{
						if(!c.collider) 
							c.point = offSet1.normalized*distance + origin;
						if(!d.collider)
							d.point = offSet2.normalized*distance + origin;

						rayHits.Add (c);
						rayHits.Add (d);
					}
				}
			}
		}
		return rayHits.ToArray();
	}

	///////////////////////CALCULATION FUNCTIONS/////////////////////////////
	/////////////////////////////////////////////////////////////////////////

	//return a angle between 0 and 359
	static float normalizeAngle(float angle)
	{
		float a = angle % 360;
		if (a < 0)
			a += 360;
		return a;
	}

	//calculates the angle of a line given origin and end point
	static float calcAngle(Vector2 orig, Vector2 end)
	{
		Vector2 e = end - orig;
		return normalizeAngle(Mathf.Atan2(e.y, e.x)*Mathf.Rad2Deg);
	}
	static float calcAngle(RaycastHit2D a)
	{
		return calcAngle(a.centroid, a.point);
	}

	//return true if angle a is between angle start and end (counter-clockwise from start to end)
	static bool isBetween(float start, float end, float a)
	{
		float barriar = 360f;
		float barriar2 = 0f;
		if (start < end)//end not cross barriar
			return (a > start && a < end);
		else//end crosses barriar
			return (a > start && a <= barriar) || (a >= barriar2 && a < end);
	}
	
	//returns the world points equvalent of the local point in polygoncollider2d
	Vector2 toWorldSpace(PolygonCollider2D c, int index)
	{
		Vector2 temp = c.transform.position;
		return temp + c.points[index];
	}
	
	//converts an array or world points to local point centered around origin
	void toLocalSpace(Vector2[] worldPoints, Vector2 origin)
	{
		for(int n = 0; n < worldPoints.Length; n++)
		{
			worldPoints[n] = worldPoints[n] - origin;
		}
	}
	
	float convertToAngle(Vector2 v)
	{
		return normalizeAngle(Mathf.Atan2(v.y, v.x)*Mathf.Rad2Deg);
	}
	Vector2 convertToPoint(float a)
	{
		return new Vector2 (Mathf.Cos (Mathf.Deg2Rad * a), Mathf.Sin (Mathf.Deg2Rad * a));
	}

	///////////////////////HELPER CLASSES and SORTING FUNCTION///////////////
	/////////////////////////////////////////////////////////////////////////

	//use this ONLY to sort RaycastHit2D!!!
	private class AngleSorter : IComparer
	{
		float startingAngle;
		public AngleSorter(float s)
		{
			startingAngle = s;
		}
		int IComparer.Compare(object a, object b)
		{
			RaycastHit2D hit1 = (RaycastHit2D)a;
			RaycastHit2D hit2 = (RaycastHit2D)b;
			if (isBetween (startingAngle, calcAngle(hit2), calcAngle(hit1)))
				return 1;
			else if (isBetween (startingAngle, calcAngle(hit1), calcAngle(hit2)))
				return -1;
			else
				return 0;
		}
	}

	//sort the raycasts by their angle, starting from angleStart (CLOCKWISE)
	void sortRayHits(RaycastHit2D[] hits, float angleStart)
	{
		AngleSorter sort = new AngleSorter(angleStart);
		Array.Sort(hits, sort);
	}

	//shitty hack to fix a shitty bug that should not be happening
	void afterSortCheck(RaycastHit2D[] hits, float angleStart)
	{
		if(calcAngle(hits[hits.Length-1]) < angleStart + 0.01f && calcAngle(hits[hits.Length-1]) > angleStart - 0.01f )
		{
			Debug.Log("IT HAPPENED !!!!");
			RaycastHit2D temp = hits[hits.Length-1];
			Array.Copy(hits, 0, hits, 1, hits.Length - 1);
			hits[hits.Length-1] = temp;
		}
	}

	///////////////////////DEBUGGING FUNCTIONS///////////////////////////////
	/////////////////////////////////////////////////////////////////////////

	//create a mesh that represent where the light hits
	Mesh createMesh(Vector2[] points)
	{
		Triangulator t = new Triangulator (points);
		
		// Use the triangulator to get indices for creating triangles
		int[] indices = t.Triangulate();
		
		// Create the Vector3 vertices
		Vector3[] vertices = new Vector3[points.Length];
		for (int i=0; i<vertices.Length; i++) {
			vertices[i] = new Vector3(points[i].x, points[i].y, 0);
		}
		
		// Create the mesh
		Mesh msh = new Mesh();
		msh.vertices = vertices;
		msh.triangles = indices;
		msh.RecalculateNormals();
		msh.RecalculateBounds();
		
		return msh;
	}	

	//print angle of rayHitList
	void check()
	{
		for(int i=0; i<rayHitsArray.Length; i++) 
		{
			Debug.Log(calcAngle(rayHitsArray[i]));
		}
	}

	//uses world space, so call this before converting to local space
	void debugLines()
	{
		for(int n = 0; n < rayHitsArray.Length; n++)
		{
			Vector2 direction = new Vector2 (rayHitsArray[n].point.x - rayHitsArray[n].centroid.x,
			                                 rayHitsArray[n].point.y - rayHitsArray[n].centroid.y);
			if(n == 0 || n == rayHitsArray.Length-1)
				Debug.DrawRay(rayHitsArray[n].centroid, direction, Color.yellow);
			else
				Debug.DrawRay(rayHitsArray[n].centroid, direction, Color.white);

			GameObject test = Instantiate (prefab);
			test.transform.position = rayHitsArray[n].point;
		}
	}
	/////////////////////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////////////////////
}
